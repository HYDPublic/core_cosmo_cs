using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace core_cosmo_cs.Models
{
    public class ResultViewModel
    {
        public int Id { get; set; }

        [Required]
        public string jsonResult { get; set; }

        public string filePath { get; set; }

        public List<Scores> scores { get; set; }
    }
}