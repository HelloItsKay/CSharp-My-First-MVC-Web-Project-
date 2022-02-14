﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CarShop.Services
{
  public  class PasswordHasher:IPasswordHasher
    {
        public string HashPassword(string password)
        {
            using var sha256Hash = SHA256.Create();
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    return string.Empty;
                }
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string   
                var  builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}