using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace core_cosmo_cs.Models
{
    public class ResultViewModel
    {
        [Required]
        public string jsonResult { get; set; }

        public string filePath { get; set; }

        public List<string> scores { get; set; }
    }
}