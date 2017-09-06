using System;
using System.ComponentModel.DataAnnotations;

namespace core_cosmo_cs.Models
{
    public class ResultViewModel
    {
        [Required]
        public string jsonResult { get; set; }
    }
}