using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HomeFixers.Models;
using System.Security.Cryptography;
using System.Text;
using System.Web.Caching;

namespace HomeFixers.Filters
{
    public class Spam : ActionFilterAttribute
    {
        public static int spamflag = 0;
        public int DelayRequest = 10;
        public string ErrorMessage = "Processing your request. Please refrain from multiple submission.";
        public string RedirectURL;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var cache = filterContext.HttpContext.Cache;
            var originationInfo = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;
            originationInfo += request.UserAgent;
            var targetInfo = request.RawUrl + request.QueryString;

            var hashValue = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(originationInfo + targetInfo)).Select(s => s.ToString("x2")));
            if (cache[hashValue] != null)
            {
               filterContext.Controller.ViewData.ModelState.AddModelError(string.Empty, ErrorMessage);
                spamflag = 1; 
            }
            else
            {
               cache.Add(hashValue, "", null, DateTime.Now.AddSeconds(DelayRequest), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                spamflag = 0;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}