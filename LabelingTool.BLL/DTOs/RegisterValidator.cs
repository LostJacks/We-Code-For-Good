using System.Text.RegularExpressions;


using System.Collections.Generic;


namespace LabelingTool.BLL.DTOs
{
    public static class RegisterValidator
    {
        private static object registerRequest;

        public static List<string> Validate(registerRequest request)
		{
			var errors = new List<string>();

            if (request == null)
            {
                errors.Add("Request is required.");
                return errors;
            }

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

            if (string.IsNullOrWhiteSpace(request.Password))
                errors.Add("Password is required.");

            if (string.IsNullOrWhiteSpace(request.ConfirmPassword))
                errors.Add("Confirm password is required.");

            if (string.IsNullOrWhiteSpace(request.FirstName))
                errors.Add("First name is required.");

            if (string.IsNullOrWhiteSpace(request.LastName))
                errors.Add("Last name is required.");

            if (!string.IsNullOrEmpty(request.Password))
            {
                if (request.Password.Length < 8)
                    errors.Add("Password must be at least 8 characters long.");
                if (request.Password.Length > 128)
                    errors.Add("Password must not exceed 128 characters.");
                if (!Regex.IsMatch(request.Password, @"[A-Z]"))
                    errors.Add("Password must contain at least one uppercase letter.");
                if (!Regex.IsMatch(request.Password, @"[a-z]"))
                    errors.Add("Password must contain at least one lowercase letter.");
                if (!Regex.IsMatch(request.Password, @"[0-9]"))
                    errors.Add("Password must contain at least one digit.");
                if (!Regex.IsMatch(request.Password, @"[\W_]"))
                    errors.Add("Password must contain at least one special character.");
            }

            if (!string.IsNullOrEmpty(request.Password)
                && !string.IsNullOrEmpty(request.ConfirmPassword)
                && request.Password != request.ConfirmPassword)
            {
                errors.Add("Password and confirm password do not match.");
            }

            if (!string.IsNullOrWhiteSpace(request.FirstName))
            {
                if (request.FirstName.Length < 3)
                    errors.Add("First name must be at least 3 characters long.");
                if (request.FirstName.Length > 50)
                    errors.Add("First name must not exceed 50 characters.");
            }

            if (!string.IsNullOrWhiteSpace(request.LastName))
            {
                if (request.LastName.Length < 2)
                    errors.Add("Last name must be at least 2 characters long.");
                if (request.LastName.Length > 50)
                    errors.Add("Last name must not exceed 50 characters.");
            }

            return errors;
        }
    }
}