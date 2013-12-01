NestDemo
========

This is a simple demo showing how to use [Nest](http://nest.azurewebsites.net/) in a .NET backend. 
The front-end application is built using [Simple.Web](https://github.com/markrendle/Simple.Web) as the 
.NET web framework hosted on [Nowin](https://github.com/Bobris/Nowin) instead of IIS. The UI is built using 
[angularjs](http://angularjs.org/) and [bootstrap](http://getbootstrap.com/).

To use the application you need a copy of the [Northwind database](http://northwinddatabase.codeplex.com/) 
and an installation of [Elasticsearch](elasticsearch.org/). 
When the database and Elasticsearch service is setup update the [app.config](https://github.com/mastoj/NestDemo/blob/master/NestDemo/App.config)
with the correct paths.

If everything is setup correctly all you need to do is to start the NestDemo application and you're good to go.

I have a presentation walking through how to get started with Elasticsearch on Windows and how to get started
with the basic of Nest on [SlideShare](https://www.slideshare.net/mastoj/getting-started-with-elasticsearch-and-net/)
