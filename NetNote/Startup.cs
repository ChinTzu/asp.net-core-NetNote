using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetNote.Models;
using Microsoft.EntityFrameworkCore;
using NetNote.Repository;
using NetNote.Middleware;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace NetNote
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = @"Filename=netnote.db";
            services.AddDbContext<NoteContext>(options => options.UseSqlite(connection));
            services.AddIdentity<NoteUser, IdentityRole>()
                .AddEntityFrameworkStores<NoteContext>()
                .AddDefaultTokenProviders();
            // Add framework services.
            services.AddMvc();
            services.AddScoped<INoteRepository, NoteRepository>();
            services.AddScoped<INoteTypeRepository, NoteTypeRepository>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            InitData(app.ApplicationServices);
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //app.UseBasicMiddleware(new BasicUser { UserName = "admin", Password = "123456" });
            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void InitData(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                
                var db = serviceScope.ServiceProvider.GetService<NoteContext>();
                db.Database.EnsureCreated();
                if (db.NoteTypes.Count() == 0)
                {
                    var notetypes = new List<NoteType>{
                        new NoteType{ Name="Daily Record"},
                        new NoteType{ Name="Code Collection"},
                        new NoteType{ Name="Expenses Record"},
                        new NoteType{ Name="Website Favorites"}
                    };
                    db.NoteTypes.AddRange(notetypes);
                    db.SaveChanges();
                }
            }
        }
    }
}
