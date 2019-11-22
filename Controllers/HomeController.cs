using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Demo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Microsoft.AspNetCore.WebUtilities;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        [HttpGet("")]
        public IActionResult RegisterPage()
        {
            return View();
        }
        [HttpGet("login")]
        public IActionResult LoginPage()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User newuser)
        {
            Console.WriteLine("$$$$$$$$$$$$$$$$$WORKING$$$$$$$$$$$$$$$$$$$$");
            if (ModelState.IsValid)
            {
                User newUser = newuser;
                if (dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("RegisterPage");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("user_id", newUser.UserId);
                return RedirectToAction("HomePage");
            }
            return View("RegisterPage");

        }
        [HttpPost]
        public IActionResult Login(LoginCredentials loggedUser)
        {
            if (ModelState.IsValid)
            {
                LoginCredentials loggeduser = loggedUser;
                User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == loggeduser.Email);
                if (userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("LoginPage");
                }
                var hasher = new PasswordHasher<LoginCredentials>();
                var result = hasher.VerifyHashedPassword(loggeduser, userInDb.Password, loggeduser.Password);
                if (result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid Email/Password");
                    return View("LoginPage");
                }
                HttpContext.Session.SetInt32("user_id", userInDb.UserId);
                return RedirectToAction("HomePage");
            }
            return View("LoginPage");
        }
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();


            return RedirectToAction("RegisterPage");
        }

        [HttpGet("chatroom")]
        public IActionResult ChatRoomPage()
        {
            if (HttpContext.Session.GetInt32("user_id") != null)
            {
                User user = dbContext.Users.Where(a => a.UserId == (int)HttpContext.Session.GetInt32("user_id")).FirstOrDefault();
                return View(user);

            }
            return View("RegisterPage");

        }
        [HttpGet("translator")]
        public IActionResult TranslatorPage()
        {
            if (HttpContext.Session.GetInt32("user_id") != null)
            {
                return View();
            }
            return View("RegisterPage");

        }


        [HttpGet("home")]
        public IActionResult HomePage()
        {
            if (HttpContext.Session.GetInt32("user_id") != null)
            {
                User theuser = dbContext.Users.Where(u => u.UserId == (int)HttpContext.Session.GetInt32("user_id")).FirstOrDefault();
                return View(theuser);

            }
            return View("RegisterPage");
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
}
