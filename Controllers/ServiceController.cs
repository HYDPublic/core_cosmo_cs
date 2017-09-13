using System;
using Microsoft.AspNetCore.Mvc;
using core_cosmo_cs.Models;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using core_cosmo_cs.Data;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Net;
using System.Linq;

namespace core_cosmo_cs.Controllers
{
    public class ServiceController : Controller
    {   
        DocumentClient _client;
        // MyDbContext _context;
        public ServiceController(DocumentClient client) {
            // _context = context;
            _client = client;
        }
        public IActionResult Index()
        {
            // var lastRequest = _context.Results.Find(1);
            // ViewData["results"] = lastRequest.jsonResult;
            var x = ExecuteSimpleQuery("Results", "ResultsCollection");
            return View();
        }

        [HttpPost, ActionName("Submit")]
        public IActionResult Submit(FormViewModel model) 
        {
            var json = MakeRequest(model.filePath);

            ResultViewModel results = ProcessResults(json, model);

            if (results != null) {
                CreateDocument(results);
                return View("Results", results);
            } else {
                ViewData["error"] = "Error in request";
                return View("Index");
            }
        }

        private async void CreateDocument(ResultViewModel results) {
            await CreateResultsDocumentIfNotExists("Results", "ResultsCollection", results);
        }

        private async Task CreateResultsDocumentIfNotExists(string databaseName, string collectionName, ResultViewModel results)
        {
            try
            {
                await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, results.Id.ToString()));
                Console.WriteLine("Found Result {0}", results.Id);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), results);
                    Console.WriteLine("Created Result {0}", results.Id);
                }
                else
                {
                    throw;
                }
            }
        }

        private int DocumentCount(string databaseName, string collectionName) {
            var queryOptions = new FeedOptions { MaxItemCount = -1 };     

            var resultsQuery = _client.CreateDocumentQuery<ResultViewModel>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                    .Where(r => r.Id != "-1");

            return resultsQuery.Count();
        }

        private IQueryable<ResultViewModel> ExecuteSimpleQuery(string databaseName, string collectionName)
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<ResultViewModel> resultsQuery = _client.CreateDocumentQuery<ResultViewModel>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                    .Where(r => r.Id == "0"); //string?
            
            foreach (ResultViewModel result in resultsQuery)
            {
                Console.WriteLine("\tRead {0}", result);
            }
            return resultsQuery;
        }

        private ResultViewModel ProcessResults(Task<string> json, FormViewModel model) {
            ResultViewModel results = new ResultViewModel();

            var length = DocumentCount("Results", "ResultsCollection");

            results.Id = length.ToString();
            results.jsonResult = json.Result;
            results.filePath = model.filePath;
            results.results = new List<Result>();

            if(results.jsonResult.Contains("error")) {
                return null;
            } else {
                var obj = JArray.Parse(results.jsonResult);

                var props = obj.First.Last.First;

                foreach (var property in props)
                {
                    Result result = new Result();
                    result.Score = property.ToString();
                    results.results.Add(result);
                }
                return results;
            }
        }
        private static async Task<string> MakeRequest(string filePath) {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "560d169268e3471685c09dbadf41422e");

            string uri = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize?";

            HttpResponseMessage response;
            string responseContent;

            byte[] data = GetImageAsByteArray(filePath);

            using (var content = new ByteArrayContent(data))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                responseContent = response.Content.ReadAsStringAsync().Result;
            }
            return responseContent;
        }

        static byte[] GetImageAsByteArray(string filePath) {
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }
}
