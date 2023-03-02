using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebConsultaSMS.DataBase;
using WebConsultaSMS.Filters;
using WebConsultaSMS.Interfaces;
using WebConsultaSMS.Services;
using WebConsultaSMS.Utils;
using System.Xml;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using System;

namespace WebConsultaSMS
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Consulta SMS", Version = "v1" });
                c.AddSecurityDefinition(
                    "token",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        In = ParameterLocation.Header,
                        Scheme = "Bearer",
                        Description =
                            "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}."
                    }
                );
                c.CustomSchemaIds(
                    x =>
                        x.GetCustomAttributes<DisplayNameAttribute>().SingleOrDefault()?.DisplayName
                        ?? SwashbuckleHelpers.DefaultSchemaIdSelector(x)
                );
                c.OperationFilter<AuthResponsesOperationFilter>();
            });
            services
                .AddHttpClient("HttpClientWithSSLUntrusted")
                .ConfigurePrimaryHttpMessageHandler(
                    () =>
                        new HttpClientHandler
                        {
                            ClientCertificateOptions = ClientCertificateOption.Manual,
                            ServerCertificateCustomValidationCallback = (
                                httpRequestMessage,
                                cert,
                                cetChain,
                                policyErrors
                            ) =>
                            {
                                return true;
                            }
                        }
                );
            // Add cors server permited
            // services.AddCors(
            //     p =>
            //         p.AddPolicy(
            //             "AllowedServers",
            //             builder =>
            //             {
            //                 var AllowServers = Configuration
            //                     .GetValue<string>("AllowedServers")
            //                     .ToString();

            //                 builder.WithOrigins(AllowServers).AllowAnyMethod().AllowAnyHeader();
            //             }
            //         )
            // );

            // Add configuration from database conection
            services.AddDbContext<ApiContext>(options =>
            {
                var connstring =
                    Configuration["ConsultaSMSConnection"]
                    ?? Configuration.GetConnectionString("ConsultaSMSConnection");

                if (Environment.IsDevelopment())
                {
                    options.UseMySQL(connstring);
                }
                else
                {
                    // Descrypt password if enviroment is Qa or Prod
                    var ConnStringBuilder = new MySqlConnectionStringBuilder(
                        Configuration.GetConnectionString("ConsultaSMSConnection")
                    );
                    ConnStringBuilder.Password = FunctionsHelper.Decrypt(
                        ConnStringBuilder.Password
                    );
                    options.UseMySQL(ConnStringBuilder.ToString());
                }
            });

            // singleton Las interfaces related to class
            services.AddTransient<IBaseService, BaseService>();
            services.AddTransient<IRequestService, RequestService>();
            services.AddTransient<IAuthenticateService, AuthenticateService>();

            //services.AddTransient<IAuthenticateService>(p=> p.GetRequiredService<AuthenticateService>());
            //services.AddTransient<IBaseService>(p=> p.GetRequiredService<AuthenticateService>());
            //services.AddTransient<IBaseService>(p => p.GetRequiredService<AuthenticateService>());
            //services.AddTransient<IBaseService, AuthenticateService>();
            // services.AddTransient<IAuthenticateService, AuthenticateService>();

            //services.AddTransient<IAuthenticateService, AuthenticateService>(p=> p.GetRequiredService<IBaseService>());


            //services.AddTransient<IBaseService>(p=> p.GetRequiredService<RequestService>());
            //services.AddTransient<IBaseService, RequestService>();
            //services.AddTransient<IBaseService, BaseService>();
            //services.AddTransient<IBaseService, RequestService>();
            //services.AddTransient<IAuthenticateService, AuthenticateService>();
            //services.AddTransient<IBaseService>();
            //services.AddTransient<IBaseService>();
            //services.AddTransient<IBaseService, BaseService>();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<SeedD>();
            services.AddTransient<UtilsResponse>();

            // Auth Bearer token Configuration
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = Configuration["Jwt:Audience"],
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])
                        )
                    };
                });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHttpsRedirection();
            }

            app.UseSwagger();
            app.UseSwaggerUI(
                c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebConsultaSMS v1")
            );
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStatusCodePages();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
