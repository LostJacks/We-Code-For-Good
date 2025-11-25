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
		public static List<string> Validate(RegisterRequestDTO request)
		{
			var errors = new List<string>();

			if (request == null)
			{
				errors.Add("Request is required.");
				return errors;
			}

			// Email
			if (string.IsNullOrWhiteSpace(request.Email))
			{
				errors.Add("Email is required.");
			}
			else
			{
				var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
				if (!Regex.IsMatch(request.Email, emailPattern))
					errors.Add("Invalid email format.");
			}

			// Name
			if (string.IsNullOrWhiteSpace(request.UserName))
				errors.Add("Name is required.");
			else if (request.UserName.Length < 2)
				errors.Add("Name must be at least 2 characters long.");
			else if (request.UserName.Length > 50)
				errors.Add("Name must not exceed 50 characters.");

			// Password
			if (string.IsNullOrWhiteSpace(request.Password))
			{
				errors.Add("Password is required.");
			}
			else
			{
				if (request.Password.Length < 8)
					errors.Add("Password must be at least 8 characters long.");

				if (!Regex.IsMatch(request.Password, @"[A-Z]"))
					errors.Add("Password must contain at least one uppercase letter.");

				if (!Regex.IsMatch(request.Password, @"[a-z]"))
					errors.Add("Password must contain at least one lowercase letter.");

				if (!Regex.IsMatch(request.Password, @"[0-9]"))
					errors.Add("Password must contain at least one digit.");

				if (!Regex.IsMatch(request.Password, @"[\W_]"))
					errors.Add("Password must contain at least one special character.");
			}

			return errors;
		}
	  }
	}


