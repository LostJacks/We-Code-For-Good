using Xunit;
using Moq;
using LabelingTool.BLL.Services;
using LabelingTool.DAL.Interfaces;
using Model;
using Microsoft.AspNetCore.Identity;

namespace TestProject1
{
    public class UserServiceTests
    {
        [Fact]
        public void Register_HashesPassword_And_CreatesUser()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            AppUser? captured = null;
            mockRepo.Setup(r => r.Create(It.IsAny<AppUser>()))
                .Callback<AppUser>(u => captured = u)
                .Returns(42);

            var hasher = new PasswordHasher<AppUser>();
            var svc = new UserService(mockRepo.Object, hasher);
            var user = new AppUser { FirstName = "A", LastName = "B", Email = "a@b.com" };
            var plain = "P@ssw0rd!";

            // Act
            var id = svc.Register(user, plain);

            // Assert
            Assert.Equal(42, id);
            Assert.NotNull(captured);
            Assert.False(string.IsNullOrEmpty(captured!.PasswordHash));
            Assert.NotEqual(plain, captured.PasswordHash);
            mockRepo.Verify(r => r.Create(It.IsAny<AppUser>()), Times.Once);
        }

        [Fact]
        public void VerifyPassword_ReturnsTrue_ForValidPassword()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            var hasher = new PasswordHasher<AppUser>();
            var svc = new UserService(mockRepo.Object, hasher);
            var user = new AppUser { FirstName = "A", LastName = "B", Email = "a@b.com" };
            user.PasswordHash = hasher.HashPassword(user, "P@ssw0rd!");

            // Act
            var ok = svc.VerifyPassword(user, "P@ssw0rd!");

            // Assert
            Assert.True(ok);
        }
    }
}