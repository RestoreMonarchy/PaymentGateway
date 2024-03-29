﻿using Microsoft.AspNetCore.Mvc;
using RestoreMonarchy.PaymentGateway.Web.Models.View;
using System.Diagnostics;

namespace RestoreMonarchy.PaymentGateway.Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("~/")]
        public IActionResult Index()
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}