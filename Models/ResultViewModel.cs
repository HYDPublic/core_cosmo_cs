using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace core_cosmo_cs.Models
{
    public class ResultViewModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [Required]
        [JsonProperty(PropertyName = "jsonResult")]
        public string jsonResult { get; set; }

        [JsonProperty(PropertyName = "filePath")]
        public string filePath { get; set; }

        [JsonProperty(PropertyName = "results")]
        public List<Result> results { get; set; }
    }
}