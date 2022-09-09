using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SuperMarket_Client
{
    public class BranchActionFilter : ActionFilterAttribute,IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var request = context.HttpContext.Request;
                var branchIdSession = request.Cookies["branchId"];

                if (branchIdSession == null)
                {
                    context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary{
                        { "controller", "Home" },{ "action", "Index" } });
                }
                base.OnActionExecuting(context);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
