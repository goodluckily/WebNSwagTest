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
                settings.Title = "���Խӿ���Ŀ";
                settings.Description = "�ӿ��ĵ�˵��";


                //�������ô�ע���ļ����أ����Ǽ��ص����ݿɱ�OpenApiTagAttribute���Ը���
                settings.UseControllerSummaryAsTagDescription = true;

                //����JwtBearer��֤��ʽһ
                settings.AddSecurity("JwtBearer", Enumerable.Empty<string>(), new OpenApiSecurityScheme()
                {
                    Description = "���Ƿ�ʽһ(ֱ�����������������֤��Ϣ������Ҫ�ڿ�ͷ���Bearer)",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = OpenApiSecurityApiKeyLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = OpenApiSecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                //����JwtBearer��֤��ʽ��
                settings.AddSecurity("JwtBearer", Enumerable.Empty<string>(), swaggerSecurityScheme: new OpenApiSecurityScheme()
                {
                    Description = "���Ƿ�ʽ��(JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�)",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = OpenApiSecurityApiKeyLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
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
