using DiLPr.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DiLPr.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using System;
using Microsoft.EntityFrameworkCore;

namespace DiLPr.Controllers
{
  public class AccountController : Controller
  {
    private readonly DiLPrContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, DiLPrContext db)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _db = db;
    }
    public IActionResult Register()
    {

      return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      else
      {
        AppUser user = new AppUser { Email = model.Email, UserName = model.UserName, Breed = model.Breed, Age = model.Age };
        IdentityResult result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
          return RedirectToAction("Login");
        }
        else
        {
          foreach (IdentityError error in result.Errors)
          {
            ModelState.AddModelError("", error.Description);
          }
          return View(model);
        }
      }
    }
    public ActionResult Login()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      else
      {
        AppUser user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
          Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: false);
          if (result.Succeeded)
          {
            return RedirectToAction("Index");
          }
          else
          {
          ModelState.AddModelError("", "Whoopsies! There is something wrong with your email or username. Please try again.");
          return View(model);
          }
        }
        else
        {
          Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(userName: model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);
          if (result.Succeeded)
          {
            return RedirectToAction("Index");
          }
          else
          {
            ModelState.AddModelError("", "Whoops! There is something wrong with your email or username. Please try again.");
            return View(model);
          }
        }
      }
    }

    [HttpPost]
    public async Task<IActionResult> LogOut()
    {
      await _signInManager.SignOutAsync();
      return RedirectToAction("Index", "Home");
    }
    public async Task<IActionResult> Index()
    {
      string userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      AppUser currentUser = await _userManager.FindByIdAsync(userId);
      if (currentUser != null)
      {
        return View(currentUser);
      }
      return View(currentUser);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string Email, string UserName)
    {
      string userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      AppUser userToUpdate = await _userManager.FindByIdAsync(userId);
      userToUpdate.UserName = UserName;
      userToUpdate.Email = Email;
      IdentityResult result = await _userManager.UpdateAsync(userToUpdate);
      return RedirectToAction("Index");
    }
  }
}