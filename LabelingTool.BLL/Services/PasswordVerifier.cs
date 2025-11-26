using Microsoft.AspNetCore.Identity;
using Model;

namespace LabelingTool.BLL.Services
{
    public class PasswordVerifier
    {
        private readonly IPasswordHasher<AppUser> _hasher;

        public PasswordVerifier(IPasswordHasher<AppUser> hasher)
        {
            _hasher = hasher;
        }

        public bool VerifyPassword(AppUser user, string password)
        {
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                return false;

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success
                   || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
