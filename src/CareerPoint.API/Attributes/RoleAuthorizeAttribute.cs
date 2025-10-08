using CareerPoint.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CareerPoint.Web.Attributes;

public class RoleAuthorizeAttribute : TypeFilterAttribute
{
    public RoleAuthorizeAttribute(string roleName)
        : base(typeof(RoleAuthorizeFilter))
    {
        Arguments = new[] { roleName };
    }
}
