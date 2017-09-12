using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core_cosmo_cs.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace core_cosmo_cs
{
    public class Startup
    {
        private const string EndpointUri = "https://core-cosmo-cs-sql.documents.azure.com:443/";
        private const string PrimaryKey = "MktS98Vx9ge5G1s6nf7Sc3Ipkuifb88aKG18CNUvhmmy4AOWDhm8ocWmCkt6DXI8NnX3zvmG39ojzi6AfT6CVw==";
        private DocumentClient client;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);

            this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = "Results" });
            this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("Results"), new DocumentCollection { Id = "ResultsCollection" });
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(client);

            // services.AddDbContext<MyDbContext>(options =>
            //     options.UseSqlite("Data Source=Results.db"));
                // options.UseSqlServer(Configuration.GetConnectionString("MyDbContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
