using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BasketJam.Helper;
using BasketJam.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BasketJam
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
            services.AddCors();
            services.AddOptions();
            services.AddMvc();
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //configuro los objetos settings de manera fuertemente tipada
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configuración de autenticación JWT
            var appSettings = appSettingsSection.Get<AppSettings>();
            //string forToken=appSettings.TopSecret+DateTime.Now;
            string forToken=appSettings.TopSecret;
            var key = Encoding.ASCII.GetBytes(forToken);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
 
            // configure DI for application services
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IEquipoService,EquipoService>();
            services.AddScoped<IJugadorService,JugadorService>();
            services.AddScoped<ICuerpoTecnicoService, CuerpoTecnicoService>();
            services.AddScoped<IJuezService,JuezService>();
            services.AddScoped<IPartidoService,PartidoService>();
            services.AddScoped<ITorneoService,TorneoService>();
            services.AddScoped<IBitacoraService,BitacoraService>();
            services.AddScoped<IEstadisticasEquipoPartidoService,EstadisticasEquipoPartidoService>();
            services.AddScoped<IEstadisticasJugadorPartidoService,EstadisticasJugadorPartidoService>();
            services.AddScoped<IVotacionPartidoService,VotacionPartidoService>();
            services.AddScoped<ITablaDePosicionesService, TablaDePosicionesService>();

            //services.AddScoped<DataContext>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            } 

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();
          //  app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
