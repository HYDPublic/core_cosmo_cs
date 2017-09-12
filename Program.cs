using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

// Following these instructions for Cosmos DB
// https://docs.microsoft.com/en-us/azure/cosmos-db/documentdb-dotnetcore-get-started

namespace core_cosmo_cs
{
    public class Program
    {
        private const string EndpointUri = "https://core-cosmo-cs-sql.documents.azure.com:443/";
        private const string PrimaryKey = "MktS98Vx9ge5G1s6nf7Sc3Ipkuifb88aKG18CNUvhmmy4AOWDhm8ocWmCkt6DXI8NnX3zvmG39ojzi6AfT6CVw==";
        private DocumentClient client;
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        private async Task GetStarted()
        {
            this.client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);
        }
    }
}
