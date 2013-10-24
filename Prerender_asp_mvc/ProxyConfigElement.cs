using System;
using System.Configuration;

namespace Prerender_asp_mvc
{
    public class ProxyConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("url")]
        public String Url
        {
            get
            {
                return (String)this["url"];
            }
            set
            {
                this["url"] = value;
            }
        }

        [ConfigurationProperty("port", DefaultValue = 80)]
        public int Port
        {
            get
            {
                return (int)this["port"];
            }
            set
            {
                this["port"] = value;
            }
        }
    }
}
