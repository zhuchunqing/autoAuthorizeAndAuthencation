using Microsoft.AspNetCore.Authorization;

namespace Common.Authorize
{
    public class PermissionRequirement:IAuthorizationRequirement
    {
        public List<PermissionData> Permissions { get; set; }
    }
    public class PermissionData 
    {
        public string UserId { get; set; }
        public string Url { get; set; }
    }
}
