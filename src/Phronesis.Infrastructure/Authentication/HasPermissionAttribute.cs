using Microsoft.AspNetCore.Authorization;

namespace Phronesis.Infrastructure.Authentication;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(policy: permission)
    {
    }
}
