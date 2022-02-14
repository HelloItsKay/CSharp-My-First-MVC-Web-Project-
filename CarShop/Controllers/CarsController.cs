using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarShop.Data;
using CarShop.Data.Models;
using CarShop.Models.Cars;
using CarShop.Services;
using MyWebServer.Controllers;
using MyWebServer.Http;

namespace CarShop.Controllers
{
    public class CarsController : Controller
    {
        private readonly IValidator validator;
        private readonly CarShopDbContext data;

        public CarsController(IValidator validator, CarShopDbContext data)
        {
            this.validator = validator;
            this.data = data;
        }
        [Authorize]
        public HttpResponse Add()
        {
            if (this.UserIsMechanic())
            {
                return Unauthorized();
            }
            return View();
        }
        [Authorize]
        [HttpPost]
        public HttpResponse Add(AddCarFormModel model)
        {
            var modelErrors = this.validator.ValidateCar(model);

            if (this.UserIsMechanic())
            {
                return Unauthorized();
            }


            if (modelErrors.Any())
            {
                return Error(modelErrors);
            }

            var car = new Car
            {
                Model = model.Model,
                Year = model.Year,
                PictureUrl = model.Image,
                PlateNumber = model.PlateNumber,
                OwnerId = this.User.Id
            };

            data.Cars.Add(car);
            data.SaveChanges();
            return Redirect($"/Cars/All");

        }
        [Authorize]
        public HttpResponse All()
        {
            var querryCars = this.data.Cars.AsQueryable();
            if (this.UserIsMechanic())
            {
                querryCars = this.data.Cars
                    .Where(c => c.Issues.Any(i => !i.IsFixed));

            }
            else
            {
                querryCars = this.data.Cars
                    .Where(c => c.OwnerId == this.User.Id);
            }

            var cars = querryCars.Select(c => new CarListingViewModel
            {
                Id = c.Id,
                Model = c.Model,
                Year = c.Year,
                Image = c.PictureUrl,
                PlateNumber = c.PlateNumber,
                FixedIssues = c.Issues.Where(i => i.IsFixed).Count(),
                RemainingIssues = c.Issues
                         .Where(i => !i.IsFixed).Count()
            })
                 .ToList();

            return View(cars);
        }

        private bool UserIsMechanic()
            => this.data.Users
            .Any(u => u.Id == this.User.Id && u.IsMechanic);

    }
}
