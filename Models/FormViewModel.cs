using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace core_cosmo_cs.Models
{
    public class FormViewModel
    {
        [Required, FromForm]
        public string filePath { get; set; }
    }
}