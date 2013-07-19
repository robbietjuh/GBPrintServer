GBPrintServer
=============

At RobbytuProjects we love automation. Automation all the way! This simple solution makes it possible to offer print jobs through HTTP to any bpac compatible Brother label printer.

We use it ourselves in combination with a inventory administration Web site. Just to provide a good example - if you want to label the items you're putting onto your web shop, why not just automate it? We did so. After adding an item to the catalog, the browser makes a simple HTTP request to the PrintServer which then prints the label. Previously, users would have to create a label for each product they had, and manually set variables.

You can use this solution yourself to automate your label distribution. Just make sure to change the path to your label files - they're currently being fetched from a local network path (\\win2k12.ad.local\Programs\...), which you probably don't have set up the same way as we do.

Note that this is indeed a very simple implementation. *Because of that, it wouldn't be a very good idea to use this in a large production environment*. If you're planning to expose this to the Internet, *you might want to tweak the variable processing a bit*. There isn't any validation at the moment (as it's being used in a private environment at this moment and we don't really need it).

Contribution is always welcome. Feel free to fork and let your creativity get loose. We love to see what others make!
