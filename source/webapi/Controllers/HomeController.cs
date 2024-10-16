using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webapi.Models;

namespace webapi.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Get()
    {

        return Ok("Test");
    }


}
