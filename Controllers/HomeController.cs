using System.Diagnostics;
using ElectronicsStoreMVC.Models;
using ElectronicsStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicsStoreMVC.Controllers;

public class HomeController : Controller
{

    private readonly ApplicationDbContext context;
    public HomeController(ApplicationDbContext context)
    {
        this.context = context;
    }

    public IActionResult Index()
    {
        var prodcucts = context.Products.OrderByDescending(p => p.Id).Take(4).ToList();
        return View(prodcucts);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
