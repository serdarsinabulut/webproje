using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportsApp.ViewModels;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Login(string ReturnUrl)
    {
        // ReturnUrl keeps the url of the action we are coming from, after login action, we want to return where we came from
        ViewData["ReturnUrl"] = ReturnUrl; //we are not using it?
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string ReturnUrl)
{
    // ReturnUrl keeps the url of the action we are coming from, after login action, we want to return where we came from
    var user = await _userManager.FindByNameAsync(model.UserName);

    if (user != null)
    {
        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

        if (result.Succeeded)
        {
            
            return RedirectToAction("Index", "Product");
        }
        else
        {
            // If login fails, this will show the error message
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }
    }
    else
    {
        // If the user is not found, this will show the error message
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
    }

    // This will return the view to the user with validation errors
    return View(model); // Display the model with validation error messages
}

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }


    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }
}

