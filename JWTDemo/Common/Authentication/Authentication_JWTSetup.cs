﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Common.Authorize;
using Common.Authentication;

namespace Common
{
    public static class Authentication_JWTSetup
    {
        public static void AddAuthentication_JWTSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            ValidateDate tt= new ValidateDate();

            var symmetricKeyAsBase64 = "QWEqwe!@#123QWEqwe!@#123QWEqwe!@#123QWEqwe!@";
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var Issuer = "xiaozhu";
            var Audience = "jwtdemo";

            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // 令牌验证参数
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = Issuer,//发行人
                ValidateAudience = true,
                ValidAudience = Audience,//订阅人
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true,
            };
            // 开启Bearer认证
            services.AddAuthentication(o =>
            {
                //o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme=nameof(ApiResponseHandler);
                o.DefaultChallengeScheme = nameof(ApiResponseHandler);
                o.DefaultForbidScheme = nameof(ApiResponseHandler);
            })
             // 添加JwtBearer服务
             .AddJwtBearer(o =>
             {
                 o.TokenValidationParameters = tokenValidationParameters;
                 o.Events = new JwtBearerEvents
                 {
                     OnChallenge = context =>
                     {
                         context.Response.Headers.Add("Token-Error", context.ErrorDescription);
                         return Task.CompletedTask;
                     },
                     OnAuthenticationFailed = context =>
                     {
                         var jwtHandler = new JwtSecurityTokenHandler();
                         var token = context.Request.Headers["Authorization"].ObjToString().Replace("Bearer ", "");

                         if (token.IsNotEmptyOrNull() && jwtHandler.CanReadToken(token))
                         {
                             var jwtToken = jwtHandler.ReadJwtToken(token);

                             if (jwtToken.Issuer != Issuer)
                             {
                                 context.Response.Headers.Add("Token-Error-Iss", "issuer is wrong!");
                             }

                             if (jwtToken.Audiences.FirstOrDefault() != Audience)
                             {
                                 context.Response.Headers.Add("Token-Error-Aud", "Audience is wrong!");
                             }
                         }


                         // 如果过期，则把<是否过期>添加到，返回头信息中
                         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                         {
                             context.Response.Headers.Add("Token-Expired", "true");
                         }
                         return Task.CompletedTask;
                     }
                 };
             })
             //添加验证方案
             .AddScheme<AuthenticationSchemeOptions, ApiResponseHandler>(nameof(ApiResponseHandler), o => tt.Validate());
        }
    }
}
