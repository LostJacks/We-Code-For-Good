using LabelingTool.BLL.Interfaces;
using LabelingTool.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Model;

namespace LabelingTool.BLL.Services
{
    public class UserRegistrar
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher<AppUser> _hasher;

        public UserRegistrar(IUserRepository repo, IPasswordHasher<AppUser> hasher)
        {
            _repo = repo;
            _hasher = hasher;
        }

        public int Register(AppUser user, string password)
        {
            user.PasswordHash = _hasher.HashPassword(user, password);
            return _repo.Create(user);
        }
    }
}
