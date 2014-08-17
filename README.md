Prerender_asp_mvc
=================

Are you using backbone, angular, emberjs, etc, but you're unsure about the SEO implications?

Use this filter that prerenders a javascript-rendered page using an external service and returns the HTML to the search engine crawler for SEO.

`Note:` If you are using a `#` in your urls, make sure to change it to `#!`. [View Google's ajax crawling protocol](https://developers.google.com/webmasters/ajax-crawling/docs/getting-started)

`Note:` Make sure you have more than one webserver thread/process running because the prerender service will make a request to your server to render the HTML.

Demo project moved to [Prerender_asp_mvc_demo](https://github.com/greengerong/Prerender_asp_mvc_demo).

1:Add this http module to your web.config or register it on HttpApplication:

*** web.xml

	<httpModules>
		<add name="Prerender" type="Prerender_asp_mvc.PrerenderModule, Prerender_asp_mvc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"/>
	</httpModules>
   

*** register
   
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
    
                    DynamicModuleUtility.RegisterModule(typeof(Prerender_asp_mvc.PrerenderModule));
                }
            }
        }
    }

    on AssemblyInfo.cs :
    [assembly: PreApplicationStartMethod(typeof(Demo.PreApplicationStartCode), "PreStart")]
    
you can see [PreApplicationStartCode](https://github.com/greengerong/Prerender_asp_mvc/blob/master/Demo/App_Start/PreApplicationStartCode.cs)

#### Please see demo on this repo.


## How it works
1. Check to make sure we should show a prerendered page
	1. Check if the request is from a crawler (`_escaped_fragment_` or agent string)
	2. Check to make sure we aren't requesting a resource (js, css, etc...)
	3. (optional) Check to make sure the url is in the whitelist
	4. (optional) Check to make sure the url isn't in the blacklist
2. Make a `GET` request to the [prerender service](https://github.com/collectiveip/prerender)(phantomjs server) for the page's prerendered HTML
3. Return that HTML to the crawler

## Customization 
 
 you can add config on your web.config by Prerender_asp_mvc.PrerenderConfigSection.

### crawlerUserAgents
example: someproxy,someproxy1

### whitelist

### blacklist


### Using your own prerender service

If you've deployed the prerender service on your own, set the `PRERENDER_SERVICE_URL` environment variable so that this package points there instead. Otherwise, it will default to the service already deployed at `http://prerender.herokuapp.com`

	$ export PRERENDER_SERVICE_URL=<new url>

Or on heroku:

	$ heroku config:add PRERENDER_SERVICE_URL=<new url>

As an alternative, you can pass `prerender_service_url` in the options object during initialization of the middleware



## Testing

If you want to make sure your pages are rendering correctly:

1. Open the Developer Tools in Chrome (Cmd + Atl + J)
2. Click the Settings gear in the bottom right corner.
3. Click "Overrides" on the left side of the settings panel.
4. Check the "User Agent" checkbox.
6. Choose "Other..." from the User Agent dropdown.
7. Type `googlebot` into the input box.
8. Refresh the page (make sure to keep the developer tools open).

## License

The MIT License (MIT)

## TODO:

*	upgrade version.(wait for my local env.)
