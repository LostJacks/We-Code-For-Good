
using LabelingTool.BLL.Interfaces;
using LabelingTool.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Model;

namespace LabelingTool.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher<AppUser> _hasher;
        private readonly UserRegistrar _registrar;
        private readonly PasswordVerifier _verifier;

        public UserService(IUserRepository repo, IPasswordHasher<AppUser> hasher)
        {
            _repo = repo;
            _hasher = hasher;
            _registrar = new UserRegistrar(repo, hasher);
            _verifier = new PasswordVerifier(hasher);
        }

        public int Register(AppUser user, string password)
        {
            return _registrar.Register(user, password);
        }

        public bool VerifyPassword(AppUser user, string password)
        {
            return _verifier.VerifyPassword(user, password);
        }

        public AppUser? GetByEmail(string email) => _repo.GetByEmail(email);
    }
}