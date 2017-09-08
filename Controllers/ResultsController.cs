using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core_cosmo_cs.Data;
using Microsoft.AspNetCore.Mvc;

namespace core_cosmo_cs.Controllers
{
    public class ResultsController : Controller
    {
        MyDbContext _context;
        public IActionResult Index(MyDbContext context)
        {
            _context = context;
            return View(_context.Results);
        }
    }
}