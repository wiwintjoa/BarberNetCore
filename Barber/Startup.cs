using Barber.API.DataAccess;
using Barber.API.DomainModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Barber.API
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        private IHostingEnvironment _env;
        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
                
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add service and create Policy with options
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddSingleton(Configuration);
                     
            // Add framework services.
            services.AddMvc();

            services.AddAutoMapper();

            // Use a PostgreSQL database
            var sqlConnectionString = Configuration.GetConnectionString("DefaultConnection");

            services
            .AddEntityFrameworkNpgsql()
            .AddDbContext<BarberContext>(options =>
            {
                options.UseNpgsql(
                    sqlConnectionString,
                    b => b.MigrationsAssembly("Barber.API")
                );               
            });

            services.AddIdentity<BarberUser, BarberRole>(cfg =>
            {
                // if we are accessing API and an authorized request is made
                // do not redirect to the login page but simple return authorized
                cfg.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api"))
                            ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;

                        return Task.FromResult(0);
                    }
                };
            }).AddEntityFrameworkStores<BarberContext>().AddDefaultTokenProviders();
        }

        public virtual void EnsureDatabaseCreated(BarberContext dbContext)
        {
            // run Migrations
            dbContext.Database.Migrate();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIdentity();
            
            app.UseJwtBearerAuthentication(new JwtBearerOptions()
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = Configuration["JwtSecurityToken:Issuer"],
                    ValidAudience = Configuration["JwtSecurityToken:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecurityToken:Key"])),
                    ValidateLifetime = true
                }
            });
          
            app.UseCors("CorsPolicy");


            //app.UseMvc();
            app.UseMvc(routes =>
            {

            });

            // within your Configure method:
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
              .CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<BarberContext>();
                EnsureDatabaseCreated(dbContext);
            }
        }
    }
}
