using System;
using System.Collections.Generic;
using System.Configuration;

namespace Prerender.io
{
    public sealed class PrerenderConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("prerenderServiceUrl", DefaultValue = "http://service.prerender.io/")]
        public String PrerenderServiceUrl
        {
            get
            {
                var prerenderServiceUrl = (String)this["prerenderServiceUrl"];
                return prerenderServiceUrl.IsNotBlank() ? prerenderServiceUrl : "http://service.prerender.io/";
            }
            set
            {
                this["prerenderServiceUrl"] = value;
            }
        }

		[ConfigurationProperty("stripApplicationNameFromRequestUrl", DefaultValue = false)]
		public bool StripApplicationNameFromRequestUrl
		{
			get
			{
				return (bool)this["stripApplicationNameFromRequestUrl"];
			}
			set
			{
				this["stripApplicationNameFromRequestUrl"] = value;
			}
		}

        [ConfigurationProperty("whitelist")]
        public String WhitelistString
        {
            get
            {
                return (String)this["whitelist"];
            }
            set
            {
                this["whitelist"] = value;
            }
        }

        public IEnumerable<String> Whitelist
        {
            get
            {
                return WhitelistString.IsBlank() ? null : WhitelistString.Trim().Split(',');
            }
        }

        [ConfigurationProperty("blacklist")]
        public String BlacklistString
        {
            get
            {
                return (String)this["blacklist"];
            }
            set
            {
                this["blacklist"] = value;
            }
        }

        public IEnumerable<String> Blacklist
        {
            get
            {
                return BlacklistString.IsBlank() ? null : BlacklistString.Trim().Split(',');
            }
        }

        [ConfigurationProperty("extensionsToIgnore")]
        public String ExtensionsToIgnoreString
        {
            get
            {
                return (String)this["extensionsToIgnore"];
            }
            set
            {
                this["extensionsToIgnore"] = value;
            }
        }

        public IEnumerable<String> ExtensionsToIgnore
        {
            get
            {
                return ExtensionsToIgnoreString.IsBlank() ? null : ExtensionsToIgnoreString.Trim().Split(',');
            }
        }


        [ConfigurationProperty("crawlerUserAgents")]
        public String CrawlerUserAgentsString
        {
            get
            {
                return (String)this["crawlerUserAgents"];
            }
            set
            {
                this["crawlerUserAgents"] = value;
            }
        }

        public IEnumerable<String> CrawlerUserAgents
        {
            get
            {
                return CrawlerUserAgentsString.IsBlank() ? null : CrawlerUserAgentsString.Trim().Split(',');
            }
        }

        [ConfigurationProperty("Proxy")]
        public ProxyConfigElement Proxy
        {
            get
            {
                return (ProxyConfigElement)this["Proxy"];
            }
            set
            {
                this["Proxy"] = value;
            }
        }

        [ConfigurationProperty("token")]
        public String Token
        {
            get
            {
                return (String)this["token"];
            }
            set
            {
                this["token"] = value;
            }
        }

        /// <summary>
        /// CDNs use the X-host to provide the actual web address,
        /// i.e. if we have the address www.my-address.com which uses the my-traffic-manager.manager.com, 
        /// the X-Host would containt "my-address.com" while the prerender.io would provide my-traffic-manager.manager.com. 
        /// In other words we would have https://my-traffic-manager.com/users indexed instead of http://my-address.com/users
        /// </summary>
        [ConfigurationProperty("useXHostHeader", DefaultValue = false)]
        public bool UseXHostHeader
        {
            get
            {
                return (bool)this["useXHostHeader"];
            }
            set
            {
                this["useXHostHeader"] = value;
            }
        }
    }
}
