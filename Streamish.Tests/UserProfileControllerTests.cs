using Streamish.Controllers;
using Streamish.Models;
using Streamish.Tests.Mocks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Streamish.Tests
{
    public class UserProfileControllerTests
    {
        [Fact]
        public void Get_Returns_All_UserProfiles()
        {
            // Arrange 
            var userProfileCount = 20;
            var userProfiles = CreateTestUserProfiles(userProfileCount);

            var repo = new InMemoryUserProfileRepository(userProfiles);
            var controller = new UserProfileController(repo);

            // Act 
            var result = controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualUserProfiles = Assert.IsType<List<UserProfile>>(okResult.Value);

            Assert.Equal(userProfileCount, actualUserProfiles.Count);
            Assert.Equal(userProfiles, actualUserProfiles);
        }

        [Fact]
        public void Get_By_Id_Returns_NotFound_When_Given_Unknown_id()
        {
            var userProfiles = new List<UserProfile>();

            var repo = new InMemoryUserProfileRepository(userProfiles);
            var controller = new UserProfileController(repo);

            var result = controller.Get(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Get_By_Id_Returns_UserProfile_With_Given_Id()
        {
            var testUserProfileId = 10000;
            var userProfiles = CreateTestUserProfiles(5);
            userProfiles[0].Id = testUserProfileId;

            var repo = new InMemoryUserProfileRepository(userProfiles);
            var controller = new UserProfileController(repo);

            var result = controller.Get(testUserProfileId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualUserProfile = Assert.IsType<UserProfile>(okResult.Value);

            Assert.Equal(testUserProfileId, actualUserProfile.Id);
        }

        [Fact]
        public void Post_Method_Adds_A_New_UserProfile()
        {
            var userProfileCount = 5;
            var userProfiles = CreateTestUserProfiles(userProfileCount);

            var repo = new InMemoryUserProfileRepository(userProfiles);
            var controller = new UserProfileController(repo);

            var newUserProfileToPost = new UserProfile()
            {
                Name = "Name",
                Email = "email@email.com",
                DateCreated = DateTime.Today,
                ImageUrl = "http://user.url/",
            };

            controller.Post(newUserProfileToPost);

            Assert.Equal(userProfileCount + 1, repo.InternalData.Count);
        }

        [Fact]
        public void Put_Method_Returns_BadRequest_When_Ids_Do_Not_Match()
        {
            var testUserProfileId = 1000;
            var userProfiles = CreateTestUserProfiles(5);
            userProfiles[0].Id = testUserProfileId;

            var repo = new InMemoryUserProfileRepository(userProfiles);
            var controller = new UserProfileController(repo);

            var userProfileToUpdate = new UserProfile()
            {
                Id = testUserProfileId,
                Name = "Name",
                Email = "email@email.com",
                DateCreated = DateTime.Today,
                ImageUrl = "http://user.url/",
            };

            var anotherUserProfileId = testUserProfileId + 1;

            var result = controller.Put(anotherUserProfileId, userProfileToUpdate);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void Put_Method_Updates_A_UserProfile()
        {
            var testUserProfileId = 1000;
            var userProfiles = CreateTestUserProfiles(5);
            userProfiles[0].Id = testUserProfileId;

            var repo = new InMemoryUserProfileRepository(userProfiles);
            var controller = new UserProfileController(repo);

            var userProfileToUpdate = new UserProfile()
            {
                Id = testUserProfileId,
                Name = "Name",
                Email = "email@email.com",
                DateCreated = DateTime.Today,
                ImageUrl = "http://user.url/",
            };

            controller.Put(testUserProfileId, userProfileToUpdate);

            var userProfileFromDb = repo.InternalData.FirstOrDefault(up => up.Id == testUserProfileId);

            Assert.NotNull(userProfileFromDb);

            Assert.Equal(userProfileToUpdate.Name, userProfileFromDb.Name);
            Assert.Equal(userProfileToUpdate.Email, userProfileFromDb.Email);
            Assert.Equal(userProfileToUpdate.DateCreated, userProfileFromDb.DateCreated);
            Assert.Equal(userProfileToUpdate.ImageUrl, userProfileFromDb.ImageUrl);

        }

        [Fact]
        public void Delete_Method_Removes_A_UserProfile()
        {
            var testUserProfileId = 1000;
            var userProfiles = CreateTestUserProfiles(5);
            userProfiles[0].Id = testUserProfileId;

            var repo = new InMemoryUserProfileRepository(userProfiles);
            var controller = new UserProfileController(repo);

            controller.Delete(testUserProfileId);

            var deletedUserProfile = repo.InternalData.FirstOrDefault(up => up.Id == testUserProfileId);

            Assert.Null(deletedUserProfile);
        }

        private List<UserProfile> CreateTestUserProfiles(int count)
        {
            var userProfiles = new List<UserProfile>();
            for (var i = 1; i <= count; i++)
            {
                userProfiles.Add(new UserProfile()
                {
                    Id = i,
                    Name = $"User {i}",
                    Email = $"user{i}@example.com",
                    DateCreated = DateTime.Today.AddDays(-i),
                    ImageUrl = $"http://user.url/{i}",
                });
            }
            return userProfiles;
        }
    }
}