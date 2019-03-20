using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HookJester.Hosting
{
    public class LocalActionFilter : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequest request = filterContext.HttpContext.Request;
            IHeaderDictionary headerDict = request.Headers;

            if (headerDict.ContainsKey("X-Forwarded-For"))
                filterContext.Result = new BadRequestObjectResult("");

            base.OnActionExecuting(filterContext);
        }
    }
}
