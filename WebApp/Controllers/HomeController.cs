using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using WebApp.Models;
using GraphQL.Client.Http;

public class HomeController : Controller
{
    
    private readonly GraphQLHttpClient _client;
    
    public HomeController(GraphQLHttpClient client)
    {
        _client = client;
    }
    
    public IActionResult Index()
    {
        // Check for token in session
        var token = HttpContext.Session.GetString("userToken");
        if (!string.IsNullOrEmpty(token))
        {
            return Redirect("https://google.com/");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (IsValidEmail(model.Email) && model.Password.Length > 12)
            {
                // Implement GraphQL call here
                var token = await AuthenticateUser(model.Email, model.Password);
                if (!string.IsNullOrEmpty(token))
                {
                    HttpContext.Session.SetString("userToken", token);
                    return Redirect("https://google.com/");
                }
            }
        }

        return View("Index");
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
        catch
        {
            return false;
        }
    }

    private async Task<string> AuthenticateUser(string email, string password)
    {
        var request = new GraphQLHttpRequest
        {
            Query = @"
        query ($email: String!, $password: String!) {
            login(email: $email, password: $password) {
                token
            }
        }",
            Variables = new
            {
                email = email,
                password = password
            }
        };

        var response = await _client.SendQueryAsync<LoginResponse>(request);

        if (response.Errors?.Any() ?? false)
        {
            // Handle any errors that occur during the query
            return null;
        }

        return response.Data.login.token;
    }


    public IActionResult Privacy()
    {
        return View();
    }
    
}