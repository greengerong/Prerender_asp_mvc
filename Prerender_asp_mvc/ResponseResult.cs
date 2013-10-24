using System;
using System.Net;

namespace Prerender_asp_mvc
{
    public class ResponseResult
    {
        public HttpStatusCode StatusCode
        {
            private set;
            get;
        }

        public String ResponseBody
        {
            private set;
            get;
        }

        public ResponseResult(HttpStatusCode code, String body)
        {
            StatusCode = code;
            ResponseBody = body;
        }

    }
}