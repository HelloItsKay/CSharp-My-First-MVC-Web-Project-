using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CarShop.Models.Cars;
using CarShop.Models.Users;

namespace CarShop.Services
{
    using static Data.DataConstants;
    public class Validator : IValidator

    {
        public ICollection<string> ValidateUser(RegisterUserFormModel model)
        {
            var errors = new List<string>();

            if (model.Username.Length < UserMinUsername || model.Username.Length > DefaultMaxLength)
            {
                errors.Add($"Username {model.Username} is not valid. It must be between {UserMinUsername} and {DefaultMaxLength} characters long.");
            }

            if (!Regex.IsMatch(model.Email, UserEmailRegularExpression))
            {
                errors.Add($"Email {model.Email} is not valid e-mail address.");
            }

            if (model.Password.Length < UserMinPassword || model.Password.Length> DefaultMaxLength)
            {
                errors.Add($"Password {model.Password} is not valid. It must be between {UserMinPassword} and {DefaultMaxLength} characters long.");
            }

            if (model.Password != model.ConfirmPassword)
            {
                errors.Add($"Password and Confirm Password does not match.");
            }

            if (model.UserType!= UserTypeMechanic && model.UserType != UserTypeClient)
            {
                errors.Add($"User should be either a {UserTypeMechanic} or {UserTypeClient}");
            }

            return errors;
        }

        public ICollection<string> ValidateCar(AddCarFormModel model)
        {
            var errors = new List<string>();

            if (model.Model.Length < UserMinUsername || model.Model.Length > DefaultMaxLength)
            {
                errors.Add($"Model {model.Model} is not valid. It must be between {CarModelMinLength} and {DefaultMaxLength} characters long.");
            }

            if (model.Year<CarYearMinValue || model.Year>CarYearMaxValue)
            {
                errors.Add($"Year {model.Year} is not valid.");
            }

            if (model.Image == null || !Uri.IsWellFormedUriString(model.Image, UriKind.Absolute))
            {
                errors.Add($"Image '{model.Image}' is not valid. It must be a valid URL.");
            }

            if (!Regex.IsMatch(model.PlateNumber,CarPlateNumberRegularExpression))
            {
                errors.Add($"Plate number {model.PlateNumber} is not valid or has wrong format.");
            }

            return errors;
        }
    }
}
