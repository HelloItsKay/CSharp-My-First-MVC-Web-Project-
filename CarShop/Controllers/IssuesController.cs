using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CarShop.Data;
using CarShop.Data.Migrations;
using CarShop.Data.Models;
using CarShop.Models.Cars;
using CarShop.Models.Issues;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MyWebServer.Controllers;
using MyWebServer.Http;

namespace CarShop.Controllers
{
      
   public class IssuesController:Controller
   {
       private readonly CarShopDbContext data;
       
        public IssuesController(CarShopDbContext data)
       {
           this.data = data;
       }
       [Authorize]
        public HttpResponse CarIssues(string carId)
        {
            if (!UserIsMechanic())
            {
                var userOwnsCar = this.data.Cars
                    .Any(c => c.Id == carId && c.OwnerId == this.User.Id);

                if (!userOwnsCar)
                {
                    return Error("You do not have access to this car.");
                }
            }

            var carIssues = this.data.Cars
                .Where(c => c.Id == carId)
                .Select(c => new CarIssuesViewModel
                {
                    Id = c.Id,
                    Model = c.Model,
                    Year = c.Year,
                    UserIsMechanic = data.Users
                        .Any(u => u.Id == this.User.Id && u.IsMechanic),
            Issues = c.Issues.Select(i => new IssueListingViewModel
                    {
                        Id = i.Id,
                        Description = i.Description,
                        IsFixed = i.IsFixed,
                        IsFixedInformation = i.IsFixed ? "Yes": "Not Yet"
                    })
                })
                .FirstOrDefault();
            if (carIssues==null)
            {
                return NotFound();
            }
            return View(carIssues);
        }
       
        [Authorize]
        public HttpResponse Add(string carId) => View( new AddIssueViewModel
        {
            CarId=carId
        });

        [Authorize]
        [HttpPost]
        public HttpResponse Add(AddIssueFormModel model)
        {

            var issue = new Issue
            {
                Description = model.Description,
                CarId = model.CarId
            };

            this.data.Issues.Add(issue);
            this.data.SaveChanges();
            return Redirect($"/Issues/CarIssues?carId={model.CarId}");
        }

        [Authorize]
        public HttpResponse Fix(string issueId, string carId)
        {
            

            var issues = data.Issues.Find(issueId);
            issues.IsFixed = true;
            data.SaveChanges();

            return Redirect($"/Issues/CarIssues?carId={carId}");
        }

        [Authorize]
        public  HttpResponse Delete(string issueId, string carId)
        {
            

            if (!UserIsMechanic())
            {
                return Unauthorized();
            }

            var issue = this.data.Issues.Find(issueId);

            issue.IsFixed = true;

            this.data.SaveChanges();

            return Redirect($"/Issues/CarIssues?carId={carId}");
        }



        private bool UserIsMechanic()
            => this.data.Users
                .Any(u => u.Id == this.User.Id && u.IsMechanic);


    }
}
