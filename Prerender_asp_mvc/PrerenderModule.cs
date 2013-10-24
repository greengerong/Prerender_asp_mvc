using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Prerender_asp_mvc
{
    public class PrerenderModule : IHttpModule
    {
        private PrerenderConfigSection _prerenderConfig;
        private HttpApplication _context;
        private static readonly string PRERENDER_SECTION_KEY = "prerender";
        private static readonly string _Escaped_Fragment = "_escaped_fragment_";

        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            this._context = context;
            _prerenderConfig = ConfigurationManager.GetSection(PRERENDER_SECTION_KEY) as PrerenderConfigSection;

            context.BeginRequest += context_BeginRequest;
        }

        protected void context_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                DoPrerender(_context);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
            }
        }

        private void DoPrerender(HttpApplication context)
        {
            var httpContext = context.Context;
            var request = httpContext.Request;
            var response = httpContext.Response;
            if (ShouldShowPrerenderedPage(request))
            {
                var result = GetPrerenderedPageResponse(request);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    response.Write(result.ResponseBody);
                    response.Flush();
                    context.CompleteRequest();
                }
            }
        }

        private ResponseResult GetPrerenderedPageResponse(HttpRequest request)
        {
            var apiUrl = GetApiUrl(request);
            var webRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            webRequest.Method = "GET";
            webRequest.UserAgent = request.UserAgent;
            SetProxy(webRequest);
            SetNoCache(webRequest);

            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            var reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
            return new ResponseResult(webResponse.StatusCode, reader.ReadToEnd());
        }

        private void SetProxy(HttpWebRequest webRequest)
        {
            if (_prerenderConfig.Proxy != null && _prerenderConfig.Proxy.Url.IsNotBlank())
            {
                webRequest.Proxy = new WebProxy(_prerenderConfig.Proxy.Url, _prerenderConfig.Proxy.Port);
            }
        }

        private static void SetNoCache(HttpWebRequest webRequest)
        {
            webRequest.Headers.Add("Cache-Control", "no-cache");
            webRequest.ContentType = "text/html";
        }

        private String GetApiUrl(HttpRequest request)
        {
            var url = request.Url.AbsoluteUri;
            var prerenderServiceUrl = _prerenderConfig.PrerenderServiceUrl;
            return prerenderServiceUrl.EndsWith("/")
                ? (prerenderServiceUrl + url)
                : string.Format("{0}/{1}", prerenderServiceUrl, url);
        }


        private bool ShouldShowPrerenderedPage(HttpRequest request)
        {
            var useAgent = request.UserAgent;
            var url = request.Url;
            var referer = request.UrlReferrer == null ? string.Empty : request.UrlReferrer.AbsoluteUri;

            if (HasEscapedFragment(request))
            {
                return true;
            }

            if (useAgent.IsBlank())
            {
                return false;
            }

            if (!IsInSearchUserAgent(useAgent))
            {
                return false;
            }


            if (IsInResources(url))
            {
                return false;
            }

            var whiteList = _prerenderConfig.Whitelist;
            if (whiteList != null && !IsInWhiteList(url, whiteList))
            {
                return false;
            }

            var blacklist = _prerenderConfig.Blacklist;
            if (blacklist != null && IsInBlackList(url, referer, blacklist))
            {
                return false;
            }

            return true;

        }

        private bool IsInBlackList(Uri url, string referer, IEnumerable<string> blacklist)
        {
            return blacklist.Any(item =>
            {
                var regex = new Regex(item);
                return regex.IsMatch(url.AbsoluteUri) || (referer.IsNotBlank() && regex.IsMatch(referer));
            });
        }

        private bool IsInWhiteList(Uri url, IEnumerable<string> whiteList)
        {
            return whiteList.Any(item => new Regex(item).IsMatch(url.AbsoluteUri));
        }

        private bool IsInResources(Uri url)
        {
            var extensionsToIgnore = GetExtensionsToIgnore();
            return extensionsToIgnore.Any(item => url.AbsoluteUri.ToLower().Contains(item.ToLower()));
        }

        private IEnumerable<String> GetExtensionsToIgnore()
        {
            var extensionsToIgnore = new List<string>(new[]{".js", ".css", ".less", ".png", ".jpg", ".jpeg",
                ".gif", ".pdf", ".doc", ".txt", ".zip", ".mp3", ".rar", ".exe", ".wmv", ".doc", ".avi", ".ppt", ".mpg",
                ".mpeg", ".tif", ".wav", ".mov", ".psd", ".ai", ".xls", ".mp4", ".m4a", ".swf", ".dat", ".dmg",
                ".iso", ".flv", ".m4v", ".torrent"});
            if (_prerenderConfig.ExtensionsToIgnore.IsNotEmpty())
            {
                extensionsToIgnore.AddRange(_prerenderConfig.ExtensionsToIgnore);
            }
            return extensionsToIgnore;
        }

        private bool IsInSearchUserAgent(string useAgent)
        {
            var crawlerUserAgents = GetCrawlerUserAgents();
            return crawlerUserAgents.Any(item => String.Compare(useAgent, item, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private IEnumerable<String> GetCrawlerUserAgents()
        {
            var crawlerUserAgents = new List<string>(new[]{"googlebot", "yahoo", "bingbot", "baiduspider",
                "facebookexternalhit", "twitterbot"});

            if (_prerenderConfig.CrawlerUserAgents.IsNotEmpty())
            {
                crawlerUserAgents.AddRange(_prerenderConfig.CrawlerUserAgents);
            }
            return crawlerUserAgents;
        }

        private bool HasEscapedFragment(HttpRequest request)
        {
            return request.QueryString.AllKeys.Contains(_Escaped_Fragment);
        }

    }
}
