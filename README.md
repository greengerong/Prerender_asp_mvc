Prerender.io middleware for ASP.NET projects
============================================

Are you using backbone, angular, emberjs, etc, but you're unsure about the SEO implications?

Use this filter that prerenders a javascript-rendered page using an external service and returns the HTML to the search engine crawler for SEO.

`Note:` If you are using a `#` in your urls, make sure to change it to `#!`. [View Google's ajax crawling protocol](https://developers.google.com/webmasters/ajax-crawling/docs/getting-started)

`Note:` Make sure you have more than one webserver thread/process running because the prerender service will make a request to your server to render the HTML.

Demo project moved to [Prerender_asp_mvc_demo](https://github.com/greengerong/Prerender_asp_mvc_demo).

##Installing the middleware

1: Do a build of this project and include and reference the DLL in your web application

2: Add the http module to your web.config:

	<httpModules>
		<add name="Prerender" type="Prerender.io.PrerenderModule, Prerender.io, Version=1.0.0.2, Culture=neutral, PublicKeyToken=null"/>
	</httpModules>

3: Sign up to get a [token](https://prerender.io/signup) and add a prerender section to the web.config file containing your token:

	<configSections>
		<section name="prerender" type="Prerender.io.PrerenderConfigSection, Prerender.io, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" />
	</configSections>

	<prerender 
		token="[YOURTOKEN]">
	</prerender>

4: You can add following additional attributes to the prerender section to override or add to the custom settings (see PrerenderModule.cs):

  - prerenderServiceUrl
  - stripApplicationNameFromRequestUrl
  - whitelist
  - blacklist
  - extensionsToIgnore
  - crawlerUserAgents

5: Create a new class called PreApplicationStartCode in the App_Start folder:

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Microsoft.Web.WebPages.OAuth;
    using Demo.Models;
    
    namespace Demo
    {
        public static class PreApplicationStartCode
        {
            private static bool _isStarting;
    
            public static void PreStart()
            {
                if (!_isStarting)
                {
                    _isStarting = true;
    
                    DynamicModuleUtility.RegisterModule(typeof(Prerender.io.PrerenderModule));
                }
            }
        }
    }

6: Add this line to the bottom of the AssemblyInfo.cs file:
```
[assembly: PreApplicationStartMethod(typeof(Demo.PreApplicationStartCode), "PreStart")]
```

7: Build and publish you web application. 

8: To make shure the middleware works correctly, you can create a first request by surfing to `[YOURURL]?_escaped_fragment_=`, then log into your account [Prerender.io](https://prerender.io). If there are errors shown in red, something went wrong. Otherwise, you're good to go!
    
## How it works / Testing
1. Check to make sure we should show a prerendered page
	1. Check if the request is from a crawler (`_escaped_fragment_` or agent string)
	2. Check to make sure we aren't requesting a resource (js, css, etc...)
	3. (optional) Check to make sure the url is in the whitelist
	4. (optional) Check to make sure the url isn't in the blacklist
2. Make a `GET` request to the [prerender service](https://github.com/collectiveip/prerender)(phantomjs server) for the page's prerendered HTML
3. Return that HTML to the crawler

####OR

Mac:
  1. Open the Developer Tools in Chrome (Cmd + Atl + J)
  2. Click the Settings gear in the bottom right corner.
  3. Click "Overrides" on the left side of the settings panel.
  4. Check the "User Agent" checkbox.
  6. Choose "Other..." from the User Agent dropdown.
  7. Type `googlebot` into the input box.
  8. Refresh the page (make sure to keep the developer tools open).

Windows:
  1. Open the Developer Tools in Chrome (Ctrl + shift + i)
  2. Open settings (F1)
  3. Click "Devices" on the left side of the settings panel.
  4. Click "Add custom device..."
  6. Choose a name (eg. Googlebot), screen size and enter the following User agent string: 
	   ```
       Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)
	   ```
  7. Make sure the new device is checked.
  8. You can now choose it from the device dropdown in the Developer Tools screen.

## Using your own prerender service

If you've deployed the prerender service on your own, set the `PRERENDER_SERVICE_URL` environment variable so that this package points there instead. Otherwise, it will default to the service already deployed at `http://service.prerender.io`

	$ export PRERENDER_SERVICE_URL=<new url>

Or on heroku:

	$ heroku config:add PRERENDER_SERVICE_URL=<new url>

As an alternative, you can pass `prerender_service_url` in the options object during initialization of the middleware

## License

The MIT License (MIT)

## TODO:

*	upgrade version.(wait for my local env.)
