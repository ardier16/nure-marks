using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUREMarks.Data;
using NUREMarks.Models;
using NUREMarks.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace NUREMarks
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<MarksContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(opts =>
            {
                opts.Password.RequiredLength = 6;  
                opts.Password.RequireNonAlphanumeric = false; 
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false; 
            })
                .AddEntityFrameworkStores<MarksContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(options =>
            {
                options.SslPort = 44341;
                options.Filters.Add(new RequireHttpsAttribute());
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseGoogleAuthentication(new GoogleOptions()
            {
                ClientId = "734066827581-50647bucd298nn1gu7b1q50gm1enaatk.apps.googleusercontent.com",
                ClientSecret = "wFLRUv4bpr0dBSWH1xXhrQPm"
            });


            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            SeedData.Initialize(app.ApplicationServices);
            RolesInitialize(app.ApplicationServices).Wait();
            AddRoles(app.ApplicationServices).Wait();
        }


        public async Task RolesInitialize(IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("student") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("student"));
            }
        }

        public async Task AddRoles(IServiceProvider serviceProvider)
        {
            UserManager<User> userManager =
                serviceProvider.GetRequiredService<UserManager<User>>();
            var context = serviceProvider.GetService<MarksContext>();
            var users = context.Users.ToList();

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Roles.Count == 0)
                {
                    await userManager.AddToRoleAsync(users[i], "student");
                }
            }

            if (!users.Exists(u => u.Email.Equals("ardier16@gmail.com")))
            {
                var user = new User
                {
                    UserName = "ardier16@gmail.com",
                    Email = "ardier16@gmail.com",
                    Name = "Администратор",
                    StudentId = null,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "111111");

                await userManager.AddToRoleAsync(user, "admin");
            }


            /*
            for (int i = 0; i < 0; i++)
            {
                string email = EmailGenerator.GenerateNureEmail(students[i].Name);


                if (!users.Exists(u => u.Email.Equals(email)))
                {
                    var user = new User
                    {
                        UserName = email,
                        Email = email,
                        StudentId = students[i].Id,
                        Name = students[i].Name.Split(' ')[1],
                        EmailConfirmed = true
                    };

                    var result = await _userManager.CreateAsync(user, "Qwerty123-");
                }

            }
            */
        }
    }
}
