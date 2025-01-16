using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OcculusControlle_UI.Models;

namespace OcculusControlle_UI.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Return the view with validations errors
            return View(model);
        }

        // Construct the payload to send to the external API
        var payload = new
        {
            email = model.Email,
            password = model.Password
        };

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync("http://localhost:5164/api/Auth/login", payload);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<dynamic>();

            // Save data to seesion
            HttpContext.Session.SetString("Token", (string)result!.ToString());
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt");
        return View(model);
    }
}
