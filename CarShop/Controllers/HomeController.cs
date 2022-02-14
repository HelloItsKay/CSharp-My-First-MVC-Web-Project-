using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyWebServer.Controllers;
using MyWebServer.Http;

namespace CarShop.Controllers
{
   public class HomeController:Controller
    {
        public HttpResponse Index()
        {
            if (this.User.IsAuthenticated)
            {
                return Redirect($"/Cars/All");
            }
            return View();
        }
    }
}
