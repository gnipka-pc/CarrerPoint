using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CareerPoint.Web.Filters;

public sealed class RoleAuthorizeFilter : IAuthorizationFilter
{
    readonly string _roleName;

    public RoleAuthorizeFilter(string roleName)
    {
        _roleName = roleName;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity == null)
        {
            context.Result = new UnauthorizedResult();
        }

        if (!context.HttpContext.User.Claims.Any(c => c.Value == _roleName))
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}
