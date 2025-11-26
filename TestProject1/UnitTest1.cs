using System;
using Xunit;
using Moq;
using LabelingTool.BLL.Services;
using LabelingTool.DAL.Interfaces;
using Model;
using Microsoft.AspNetCore.Identity;
using LabelingTool.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LabelingTool.BLL.DTOs;
using Smart_Server.Controllers;

namespace TestProject1
{
    public class AuthenticationTests
    {
        [Fact]
        public void UserService_Register_HashesPassword_And_CreatesUser()
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
        public void UserService_VerifyPassword_ReturnsTrue_ForValidPassword()
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

        [Fact]
        public void AuthController_Register_Returns_BadRequest_When_EmailExists()
        {
            // Arrange
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.EmailExists("a@b.com")).Returns(true);

            var tokenSvc = new Mock<ITokenService>();
            var userSvc = new Mock<IUserService>();

            var ctrl = new AuthController(repo.Object, tokenSvc.Object, userSvc.Object);

            var req = new registerRequest { Email = "a@b.com", Password = "P@ssw0rd!", ConfirmPassword = "P@ssw0rd!", FirstName = "Ann", LastName = "Bo" };

            // Act
            var result = ctrl.Register(req);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void AuthController_Register_Returns_Ok_When_Successful()
        {
            // Arrange
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.EmailExists(It.IsAny<string>())).Returns(false);

            var tokenSvc = new Mock<ITokenService>();
            tokenSvc.Setup(t => t.CreateToken(It.IsAny<AppUser>())).Returns("access");
            tokenSvc.Setup(t => t.GenerateRefreshToken()).Returns("refresh");

            var userSvc = new Mock<IUserService>();
            userSvc.Setup(u => u.Register(It.IsAny<AppUser>(), It.IsAny<string>())).Returns(123);

            var ctrl = new AuthController(repo.Object, tokenSvc.Object, userSvc.Object);

            var req = new registerRequest { Email = "new@user.com", Password = "P@ssw0rd!", ConfirmPassword = "P@ssw0rd!", FirstName = "Ann", LastName = "Bo" };

            // Act
            var result = ctrl.Register(req);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var auth = Assert.IsType<AuthResponse>(ok.Value);
            Assert.Equal("access", auth.AccessToken);
            Assert.Equal("refresh", auth.RefreshToken);
            Assert.Equal(123, auth.User.Id);
        }
    }
}