using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NJsonSchema.Generation;
using NSwag;

namespace WebNSwagTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(options =>
            {
                //// Use camel case properties in the serializer and the spec (optional)
                //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //// Use string enums in the serializer and the spec (optional)
                //options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            services.AddOpenApiDocument(settings =>
            {
                settings.DocumentName = "v1";
                settings.Version = "v0.0.1";
                settings.Title = "测试接口项目";
                settings.Description = "接口文档说明";


                //可以设置从注释文件加载，但是加载的内容可被OpenApiTagAttribute特性覆盖
                settings.UseControllerSummaryAsTagDescription = true;

                //定义JwtBearer认证方式一
                settings.AddSecurity("JwtBearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme()
                {
                    Description = "这是方式一(直接在输入框中输入认证信息，不需要在开头添加Bearer)",
                    Name = "Authorization",//jwt默认的参数名称
                    In = OpenApiSecurityApiKeyLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = OpenApiSecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                //定义JwtBearer认证方式二
                settings.AddSecurity("JwtBearer", Enumerable.Empty<string>(), swaggerSecurityScheme: new OpenApiSecurityScheme()
                {
                    Description = "这是方式二(JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）)",
                    Name = "Authorization",//jwt默认的参数名称
                    In = OpenApiSecurityApiKeyLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = OpenApiSecuritySchemeType.ApiKey
                });
            });
           
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // Add OpenAPI/Swagger middlewares
            app.UseOpenApi(); // Serves the registered OpenAPI/Swagger documents by default on `/swagger/{documentName}/swagger.json`

            app.UseSwaggerUi(config => config.TransformToExternalPath = (internalUiRoute, request) =>
            {
                if (internalUiRoute.StartsWith("/") == true && internalUiRoute.StartsWith(request.PathBase) == false)
                {
                    return request.PathBase + internalUiRoute;
                }
                else
                {
                    return internalUiRoute;
                }
            });

            // Register the middleware before UseRouting()
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
