using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CarShop.Data;
using CarShop.Data.Models;
using CarShop.Models.Users;
using CarShop.Services;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using MyWebServer.Controllers;
using MyWebServer.Http;

namespace CarShop.Controllers
{
    public class UsersController : Controller
    {
        private readonly IValidator validator;
        private readonly IPasswordHasher passwordHasher;
        private readonly CarShopDbContext data;

        public UsersController(IValidator validator,
            IPasswordHasher passwordHasher,
            CarShopDbContext data )
        {
            this.validator = validator;
            this.passwordHasher=passwordHasher;
            this.data = data;

        }
        public HttpResponse Register()
        {
            return View();
        }

        [HttpPost]
        public HttpResponse Register(RegisterUserFormModel model)
        {
            var modelErrors = this.validator.ValidateUser(model);
            if (this.data.Users.Any(u=>u.Username==model.Username))
            {
                modelErrors.Add($"{model.Username} already exists.");
            }

            if (this.data.Users.Any(u => u.Email == model.Email))
            {
                modelErrors.Add($"{model.Username} is already used.");
            }

            if (modelErrors.Any())
            {
                return Error(modelErrors);
            }

            var user = new User
            {
                Username = model.Username,
                Password = this.passwordHasher.HashPassword(model.Password),
                Email = model.Email, 
                IsMechanic = model.UserType == DataConstants.UserTypeMechanic
            };

            data.Users.Add(user);
            data.SaveChanges();

            return Redirect("/Users/Login");
        }

        public HttpResponse Login() => View();

        [HttpPost]
        public HttpResponse Login(LoginUserFormModel model)
        {
            var hashedPassword = this.passwordHasher.HashPassword(model.Password);
            var userId = this.data.Users
                .Where(u => u.Username == model.Username &&
                            u.Password == hashedPassword)
                .Select(u=>u.Id)
                .FirstOrDefault();

            if (userId==null)
            {
                return Error($"Username and password combination is not valid.");
            }
            this.SignIn(userId);

            return Redirect($"/Cars/All");

        }

        public HttpResponse Logout()
        {
            this.SignOut();
            return Redirect("/");
        }

    }
}
