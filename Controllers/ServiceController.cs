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
            var test = _context.Results;
            return View();
        }

        [HttpPost, ActionName("Submit")]
        public IActionResult Submit(FormViewModel model) 
        {
            var json = MakeRequest(model.filePath);
            ResultViewModel results = new ResultViewModel();

            // Raw JSON, parse into more readable result
            results.jsonResult = json.Result;
            results.filePath = model.filePath;
            results.scores = new List<Scores>();

            var obj = JArray.Parse(results.jsonResult);

            var props = obj.First.Last.First;

            foreach (var property in props)
            {
                Scores score = new Scores();
                score.Score = property.ToString();
                results.scores.Add(score);
            }

            if(_context == null) {
                results.scores.Clear();
            }

            return View("Results", results);
        }

        static async Task<string> MakeRequest(string filePath) {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "560d169268e3471685c09dbadf41422e");

            string uri = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize?";

            HttpResponseMessage response;
            string responseContent;

            byte[] data = GetImageAsByteArray(filePath);

            using (var content = new ByteArrayContent(data))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                responseContent = response.Content.ReadAsStringAsync().Result;
            }

            //A peak at the JSON response.
            // Console.WriteLine(responseContent);
            return responseContent;
        }

        static byte[] GetImageAsByteArray(string filePath) {
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }
}
