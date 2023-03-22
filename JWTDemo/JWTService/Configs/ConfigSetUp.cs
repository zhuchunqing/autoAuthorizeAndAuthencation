using Common.Authorize;
using JWTService.Tokens;

namespace JWTService.Configs
{
    public static class ConfigSetUp
    {
        public static void AddConfigs(this IServiceCollection services)
        {
            services.AddSingleton<ITokenHelper, TokenHelper>();
            //services.Configure<JWTConfig>(.GetSection("JWT"));
            //JWTConfig config = new JWTConfig();
            //Configuration.GetSection("JWT").Bind(config);
        }
    }
}
