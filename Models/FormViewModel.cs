using System;
using System.ComponentModel.DataAnnotations;

namespace core_cosmo_cs.Models
{
    public class FormViewModel
    {
        [Required]
        public string filePath { get; set; }

        public bool ShowFilePath => !string.IsNullOrEmpty(filePath);
    }
}