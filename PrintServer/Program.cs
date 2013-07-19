using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Web;
using bpac;

namespace PrintServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" ");
            Console.WriteLine("  GBPrintServer 1.1.0");
            Console.WriteLine("  (C) RobbytuProjects 2013");
            Console.WriteLine(" ");

            TcpListener listener = new TcpListener(IPAddress.Any, 8000);
            listener.Start();

            Console.WriteLine("  * Listening for incoming connections on port 8000.");

            while (true)
            {
                try
                {
                    Console.WriteLine(" ");

                    Socket soc = listener.AcceptSocket();
                    Console.WriteLine("  * Accepted incoming connection: {0}", soc.RemoteEndPoint);

                    Stream s = new NetworkStream(soc);
                    StreamReader sr = new StreamReader(s);
                    StreamWriter sw = new StreamWriter(s);

                    Console.WriteLine("  * Reading job information...");

                    bool waitForJob = true;
                    while (waitForJob)
                    {
                        String input = sr.ReadLine();
                        if (input.StartsWith("GET "))
                        {
                            Console.WriteLine("  * Processing job information...");

                            String[] jobParts = input.Split("?".ToCharArray())[1].Split(" ".ToCharArray())[0].Split("&".ToCharArray());
                            IDictionary<string, string> processedParts = new Dictionary<string, string>();

                            foreach (String part in jobParts)
                            {
                                String decodedPart = HttpUtility.UrlDecode(part);
                                processedParts[decodedPart.Split("=".ToCharArray())[0]] = decodedPart.Split("=".ToCharArray())[1];
                            }

                            DocumentClass doc = new DocumentClass();
                            if (doc.Open("\\\\win2k12.ad.local\\Programs\\GBPrintServer\\" + processedParts["label"]) != false)
                            {
                                Console.WriteLine("  * Preparing print job...");

                                foreach (KeyValuePair<string, string> part in processedParts)
                                {
                                    if (part.Key != "label")
                                    {
                                        Console.WriteLine("    -> Setting object value: {0}={1}", part.Key, part.Value);
                                        doc.GetObject(part.Key).Text = part.Value;
                                    }
                                }


                                Console.WriteLine("  * Sending job to printer...");

                                doc.StartPrint("", PrintOptionConstants.bpoNoCut);
                                doc.PrintOut(1, PrintOptionConstants.bpoNoCut);
                                doc.EndPrint();

                                Console.WriteLine("  * Writing response code NOERR...");

                                sw.WriteLine("HTTP/1.1 200 OK");
                                sw.WriteLine("Content-Type: text/html");
                                sw.WriteLine("Content-Length: 3");
                                sw.WriteLine("");
                                sw.Write("noe"); // NO ERROR
                                sw.Flush();

                                Console.WriteLine("  * Closing connection...");

                                soc.Close();
                            }
                            else
                            {
                                Console.WriteLine("    -> E: Could not open label file: are you connected to the Network?");
                                Console.WriteLine("    -> Writing response code ERR...");

                                sw.WriteLine("HTTP/1.1 200 OK");
                                sw.WriteLine("Content-Type: text/html");
                                sw.WriteLine("Content-Length: 3");
                                sw.WriteLine("");
                                sw.Write("err");
                                sw.Flush();

                                Console.WriteLine("  * Closing connection...");

                                soc.Close();
                            }

                            waitForJob = false;
                        }

                        Console.WriteLine(" ");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("    -> Unexpected exception during Connloop.");
                    Console.WriteLine("    -> E: " + e.Message);
                }
            }
        }
    }
}
