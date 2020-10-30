using System;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Server_CS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var onlineCheckerThread = new Thread(OnlineCheckerCycle) {Name = "OnlineCheckerThread"};
            onlineCheckerThread.Start();
        }

        public IConfiguration Configuration { get; }

        public static void OnlineCheckerCycle()
        {
            while (true)
            {
                foreach (var user in Program.OnlineUsersTimeout)
                    if (user.Value.AddSeconds(5) < DateTime.Now)
                    {
                        Program.OnlineUsers[user.Key] = false;
                        Program.Messages.Add(new Message
                        {
                            Name = "",
                            Text = $"{user.Key} left",
                            Ts = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
                        });
                        JsonWorker.Save(Program.Messages);

                        Program.OnlineUsersTimeout.Remove(user.Key);
                    }

                Thread.Sleep(100);
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}