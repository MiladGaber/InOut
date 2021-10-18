using InOut.Models;
using InOut.Models.ViewModels;
using InOut.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InOut.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        RoleManager<IdentityRole> _roleManager;
        
        public AccountController(ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        //GET
        public IActionResult Login()
        {
            return View();
        }

        //GET
        public async Task<IActionResult> Register()
        {
            if (!_roleManager.RoleExistsAsync(Helper.Admin).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(Helper.Admin));
                await _roleManager.CreateAsync(new IdentityRole(Helper.User));
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Register(RegisterViewModel obj)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = obj.Email,
                    Email = obj.Email,
                    Name = obj.Name
                };

                var result = await _userManager.CreateAsync(user,obj.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, obj.RoleName);
                    await _signInManager.SignInAsync(user,isPersistent:false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register Error", error.Description);
                }
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel obj)
        {
            if (ModelState.IsValid)
            {
                var result =  await _signInManager.PasswordSignInAsync(obj.Email ,obj.Password ,obj.RememberMe,false);
                if (result.Succeeded)
                {
                    return RedirectToAction("index", "Home");
                }

                ModelState.AddModelError("Login Error", "Invalid Loggin ");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login","Account");
        }


    }
}
