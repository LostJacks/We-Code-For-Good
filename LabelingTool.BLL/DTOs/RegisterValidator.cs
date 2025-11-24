using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using LabelingTool.BLL.DTOs;
using Azure.Core;

namespace LabelingTool.BLL.DTOs
{
    public static class RegisterValidator
    {
        public static List<string> Validate(RegisterRequestDTO registerRequest)
        {
            var errors = new List<string>();

            if (registerRequest == null)
            {
                errors.Add("Request is required.");
                return errors;
            }
             
            if ( string.IsNullOrWhiteSpace(registerRequest.Email))
            {
                errors.Add("Email is required.");
            }
            else
            {
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; //Here is the regular expression for validation.
                if (!Regex.IsMatch(registerRequest.Email, emailPattern))
                {
                    errors.Add("Invalid email format.");
                }

                if (string.IsNullOrEmpty(registerRequest.Password))
                {
                    errors.Add("Password is required.");
                }
                
                if (string.IsNullOrEmpty(registerRequest.ConfirmPassword))
                {
                    errors.Add("Confirm password is required.");
                }
                if(string.IsNullOrEmpty(registerRequest.FirstName))
                {
                    errors.Add("First name is required.");
                }
                if (string.IsNullOrEmpty(registerRequest.LastName))
                {
                    errors.Add("Last name is required.");
                }

                else
                {
                    if (registerRequest.Password.Length < 8)
                        errors.Add("Password must be at least 8 characters long.");
                    if (registerRequest.Password.Length > 128)
                        errors.Add("Password must not exceed 128 characters.");
                    if (!Regex.IsMatch(registerRequest.Password,@"[A-Z]"))
                        errors.Add("Password must contain at least one uppercase letter.");
                    if (!Regex.IsMatch(registerRequest.Password, @"[a-z]"))
                        errors.Add("Password must contain at least one lowercase letter.");
                    if (!Regex.IsMatch(registerRequest.Password, @"[0-9]")) ;
                    errors.Add("Password must containt at least one digit."));
                    if (!Regex.IsMatch(registerRequest.Password, @"[\W_]")) ;
                    errors.Add("Password must contain at least one special character.");
                    if (registerRequest.Password !=registerRequest.ConfirmPassword)
                        errors.Add("Password and confirm password do not match. Make sure both are the same.");
                    if(string.IsNullOrWhiteSpace(registerRequest.FirstName))
                        errors.Add( "First name is required.");
                    if(string.IsNullOrWhiteSpace(registerRequest.LastName))
                        errors.Add( "Last name is required.");
                    if (string.IsNullOrWhiteSpace(registerRequest.ConfirmPassword)) ;
                    errors.Add("Confirm password is required."
                        if (registerRequest.FirstName.Length < 3) ;
                    errors.Add("First name must be at least 3 charachters long.");
                    if (registerRequest.LastName.Length  <2) ;
                    errors.Add("Last name must be at least 2 charachters long.");
                    if (registerRequest.FirstName.Length > 50) ;
                    errors.Add("First name must not exceed 50 charachters.");
                    if (registerRequest.LastName.Length > 50) ;
                    errors.Add("Last name must not exceed 50 charachters.");

                }

            }


        }
           
         
         

    }
}
