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
