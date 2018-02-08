using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace YFAPICommonCore
{
    public class Startup
    {
        public static readonly SymmetricSecurityKey symmetricKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("need_to_get_this_from_enviroment"));
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ///Init Mysql
            var connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            services.AddDbContext<MyDBContext>(options =>
                options.UseMySQL(connectionString)
            );

            //Init Swagger  
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "WebAPI"
                });

                //启用auth支持  
                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                //Determine base path for the application.    
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                //Set the comments path for the swagger json and ui.    
                var xmlPath = Path.Combine(basePath, "YFAPICommonCore.xml");//TestCore.xml是项目帮助文档，需要在项目属性-生成里开启文档  
                options.IncludeXmlComments(xmlPath);
            });

            //Init Authenticate
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role,

                    ValidIssuer = "YFAPICommomCore",
                    ValidAudience = "api",
                    IssuerSigningKey = symmetricKey
                };
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Init Authenticate
            app.UseAuthentication();

            //InitSwagger  
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI V1");
                c.ShowRequestHeaders();
            });

            app.UseMvc();
        }
    }
}
