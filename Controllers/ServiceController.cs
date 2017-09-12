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

namespace core_cosmo_cs.Controllers
{
    public class ServiceController : Controller
    {
        MyDbContext _context;
        public ServiceController(MyDbContext context) {
            _context = context;
        }
        public IActionResult Index()
        {
            var lastRequest = _context.Results.Find(0);
            ViewData["results"] = lastRequest.jsonResult;
            return View();
        }

        [HttpPost, ActionName("Submit")]
        public IActionResult Submit(FormViewModel model) 
        {
            var json = MakeRequest(model.filePath);

            ResultViewModel results = ProcessResults(json, model);

            _context.Add(results);
            _context.SaveChanges();
            return View("Results", results);
        }

        private ResultViewModel ProcessResults(Task<string> json, FormViewModel model) {
            ResultViewModel results = new ResultViewModel();

            results.jsonResult = json.Result;
            results.filePath = model.filePath;
            results.results = new List<Result>();

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
