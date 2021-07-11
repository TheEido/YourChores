using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourChores.Authentication
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services ,IConfiguration config)
        {
            //Adding Secret key
            var secret = config.GetSection("Jwt").GetSection("Key").Value;
            var Key = Encoding.ASCII.GetBytes(secret);

            //Adding the authentication to the service
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                //Adding the Jwt Token authentication
                .AddJwtBearer(x =>
                {
                    //Token Configuration
                    x.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudience = config.GetSection("Jwt").GetSection("Audience").Value,
                        ValidIssuer = config.GetSection("Jwt").GetSection("Issuer").Value,
                        
                    };
                });
            return services;
        }
    }
}
