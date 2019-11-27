using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MyMusic.Core;
using MyMusic.Core.Services;
using MyMusic.Data;
using MyMusic.Services;

namespace MyMusic.Api
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
            /*
                Transient — Objects are different. One new instance is provided to every controller and every service
                Scoped — Objects are same through the request
                Singleton — Objects are the same for every request during the application lifetime
            */
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            /*
                Here we add our MyMusicDbContext, tell to use SqlServer using the Default connection strings in appsettings.json 
                and that our migrations should be run in MyMusic.Data
            */
            services.AddDbContext<MyMusicDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default"), x => x.MigrationsAssembly("MyMusic.Data")));
            
            /*
                dotnet ef --startup-project MyMusic.Api/MyMusic.Api.csproj migrations add InitialModel -p MyMusic.Data/MyMusic.Data.csproj
                
                --startup-project switch tells that MyMusic.Api is the entry project for our app and switch -p tells that 
                the target project of our migrations is MyMusic.Data.InitialModel is the name of this migration.
                You can check in the Data project that a new folder called Migrations was automatically created and it contains our new migration.

                dotnet ef --startup-project MyMusic.Api/MyMusic.Api.csproj database update
                reflect our migrations in our database
            */
            
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IMusicService, MusicService>();
            services.AddTransient<IArtistService, ArtistService>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "My Music", Version = "v1" });
            });

            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Music V1");
            });
        }
    }
}
