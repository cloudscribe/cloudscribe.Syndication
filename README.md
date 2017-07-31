# cloudscribe.Syndication
a re-useable RSS Feed generator for ASP.NET Core

It was implemented for use in [cloudscribe SimpleContent](https://github.com/joeaudette/cloudscribe.SimpleContent), but could be used by anyone who would like to add [RSS Feed](http://cyber.law.harvard.edu/rss/rss.html) support to their web application.

### Build Status

| Windows  | Linux/Mac |
| ------------- | ------------- |
| [![Build status](https://ci.appveyor.com/api/projects/status/si9j58aa51wel2dv/branch/master?svg=true)](https://ci.appveyor.com/project/joeaudette/cloudscribe-syndication/branch/master)  | [![Build Status](https://travis-ci.org/joeaudette/cloudscribe.Syndication.svg?branch=master)](https://travis-ci.org/joeaudette/cloudscribe.Syndication)  |

[![Join the chat at https://gitter.im/joeaudette/cloudscribe](https://badges.gitter.im/joeaudette/cloudscribe.svg)](https://gitter.im/joeaudette/cloudscribe?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Rationale - why do we need this?

You may notice that in the ASP.NET Core world, ways that we used to generate RSS Feeds don't currently exist. The [System.ServiceModel.Syndication](https://msdn.microsoft.com/en-us/library/system.servicemodel.syndication%28v=vs.110%29.aspx) namespace that can be used to build feeds in the full desktop framework have not yet been ported to .NET Core and I don't know if they have plans to implement that later or not. The other commonly used [Argotic Framework](https://argotic.codeplex.com/) has not been updated since 2008 and is not designed in a way that makes it easy to port. I needed a solution for my SimpleContent project and decided to implement something myself rather than wait for something else that may not be available for a while. I did borrow the model classes from Argotic in order to build cloudscribe.Syndication.


## How to use it in your own application

To use it in your own application, add a reference in the dependencies section of your project.json

    "cloudscribe.Syndication.Web": "1.0.0-*"

Then you need to implement your own [IChannelProvider](https://github.com/joeaudette/cloudscribe.Syndication/blob/master/src/cloudscribe.Syndication/Models/Rss/IChannelProvider.cs) to build the feed content.

Then in the Startup.cs of your appplication, register your implementation with the dependency injection services like this:

    services.AddScoped<cloudscribe.Syndication.Models.Rss.IChannelProvider, yournamespace.YourRssChannelProvider>();
	
You can see [my implementation](https://github.com/joeaudette/cloudscribe.SimpleContent/blob/master/src/cloudscribe.SimpleContent.Syndication/RssChannelProvider.cs) in the cloudscribe.SimpleContent.Syndication project, which should give you a good example of how to implement it from your own content models.

Then it should work at yoursite/api/rss

## What is provided

* RssController which is the endpoint
* Logic for building the rss/xml result from the channel model

## What is not provided or not provided yet

* I included the Atom models from Argotic framework but have not implemented anything for Atom feeds. It should be fairly straightforward to implement something similar to what I have done for RSS but for my purposes RSS is all I really need right now and I'm not sure if I will later implement Atom feeds or not. Some people may want to implement both RSS and Atom but from my reading of best practices it is better to just have one feed. Certainly if someone else wants to implement it and submit a pull request that would be great!
* The demo.Web project in this solution is just a stub, not really a demo yet. I thought to implement a hard coded example of IChannelProvider and use it in a demo app but I have not done that yet and it is a low priority. The best available demo at the moment is my [cloudscribe.SimpleContent project](https://github.com/joeaudette/cloudscribe.SimpleContent).
