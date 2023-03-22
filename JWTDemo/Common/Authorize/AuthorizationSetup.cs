using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Authorize
{
    public static class AuthorizationSetup
    {
        public static void AddAuthorizationSetup(this IServiceCollection services) 
        {
            if (services == null) 
                throw new ArgumentNullException(nameof(services));
            var permissionRequirment = new PermissionRequirement
            {
                Permissions = new List<PermissionData>()
            };
            services.AddAuthorization(a => {
                a.AddPolicy("Permission", p => p.Requirements.Add(permissionRequirment));
            });
            services.AddSingleton(permissionRequirment);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
        }
    }
}
