using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarShop.Models.Cars;
using CarShop.Models.Users;

namespace CarShop.Services
{
  public  interface IValidator
  {
      ICollection<string> ValidateUser(RegisterUserFormModel model);
      ICollection<string> ValidateCar(AddCarFormModel model);
  } 
}
