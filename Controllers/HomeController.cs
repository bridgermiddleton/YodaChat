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
                return RedirectToAction("Index");
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
                    return View("LoginAndRegPage");
                }
                var hasher = new PasswordHasher<LoginCredentials>();
                var result = hasher.VerifyHashedPassword(loggeduser, userInDb.Password, loggeduser.Password);
                if (result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid Email/Password");
                    return View("LoginAndRegPage");
                }
                HttpContext.Session.SetInt32("user_id", userInDb.UserId);
                return RedirectToAction("Index");
            }
            return View("LoginPage");
        }
        [HttpPost]
        public IActionResult Logout()
        {
            User user = dbContext.Users.Where(a => a.UserId == (int)HttpContext.Session.GetInt32("user_id")).Include(m => m.CreatedMessages).FirstOrDefault();
            foreach (Message message in user.CreatedMessages.ToList())
            {
                dbContext.Remove(message);
                dbContext.SaveChanges();
            }
            HttpContext.Session.Clear();


            return RedirectToAction("RegisterPage");
        }

        [HttpGet("home")]
        public IActionResult Index()
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
        [HttpPost]
        public IActionResult YodaSpeak(YodaMessage yodaMessage)
        {

            YodaMessage newMessage = yodaMessage;
            string message = yodaMessage.Content;
            string url = QueryHelpers.AddQueryString("https://yodish.p.rapidapi.com/yoda.json", "text", message);
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("x-rapidapi-host", "yodish.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "e9077e6ee9mshe29568a25c117e8p111788jsn2ad418d390bc");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            IRestResponse<YodaResponse> response = client.Execute<YodaResponse>(request);
            HttpContext.Session.SetString("yoda_translation", response.Data.contents.translated);

            return RedirectToAction("TranslatorPage");



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
