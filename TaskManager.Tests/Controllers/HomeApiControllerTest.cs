using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Moq;
using TaskManager.Controllers;
using TaskManager.Helper;
using NUnit.Framework;
using TaskManager.Models;

namespace TaskManager.Tests.Controllers
{
    [TestFixture]
    public class HomeApiControllerTest
    {
        private HomeApiController homeApiController;
        private string uri = "http://localhost:63586/api/home";
        private QueryOptions queryOptions;
        private Mock<IPathFinder> pathFinder;
        private string rFilePath = @"C:\Projects\TaskManager\TaskManager.Tests\App_TestData\rTaskDataMock.json";
        private string mFilePath = @"C:\Projects\TaskManager\TaskManager.Tests\App_TestData\mTaskDataMock.json";
        private TaskViewModel taskViewModel;

        [SetUp]
        public void Initialise()
        {
            InitialiseMocks();
            SetFakeApiControllerContext();
        }

        [Test]
        public void GetTaskSet_Returns_StatusCodeAOK()
        {
            // Arrange

            // Act
            var response = homeApiController.GetTaskSet(homeApiController.Request, queryOptions);

            // Assert
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public void GetTaskSet_Returns_ListOfTasksForTheUserRobert()
        {
            // Arrange
            queryOptions.LoggedInUser = "1";

            // Act
            var response = homeApiController.GetTaskSet(homeApiController.Request, queryOptions);

            // Assert
            dynamic actual;
            response.TryGetContentValue(out actual);
            var robertData = (TaskViewModel)actual[0];

            Assert.AreEqual("Robert extension should change", robertData.Title);
        }

        [Test]
        public void GetTaskSet_Returns_TypeOfTaskViewModelList()
        {
            // Arrange
            queryOptions.LoggedInUser = "1";

            // Act
            var response = homeApiController.GetTaskSet(homeApiController.Request, queryOptions);

            // Assert
            dynamic actual;
            response.TryGetContentValue(out actual);

            Assert.IsInstanceOf<List<TaskViewModel>>(actual);
        }

        [Test]
        public void CreateTask_SaveDataToTest_ToMockJsonFile()
        {
            // Arrange
            queryOptions.LoggedInUser = "1";
            taskViewModel = new TaskViewModel
            {
                Id = -1,
                Title = "Saved permanantly from test",
                Description = "Saved permanantly to file without editing",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                TaskState = TaskStateEnum.Active,
                LoggedInUser = "1"
            };

            // Act
            var response = homeApiController.CreateTask(homeApiController.Request, taskViewModel);

            // Assert
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public void CreateTask_SaveData_ToMockJsonFile()
        {
            // Arrange
            queryOptions.LoggedInUser = "1";
            taskViewModel = new TaskViewModel
            {
                Id = -1,
                Title = "Saved from test",
                Description = "Saved",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                TaskState = TaskStateEnum.Active,
                LoggedInUser = "1"
            };

            // Act
            var response = homeApiController.CreateTask(homeApiController.Request, taskViewModel);

            // Assert
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public void UpdateTask_SaveData_ToMockJsonFile()
        {
            // Arrange
            queryOptions.LoggedInUser = "1";
            taskViewModel = new TaskViewModel
            {
                Id = 3,
                Title = "Edited from test",
                Description = "Edited",
                CreatedDate = DateTime.Now.AddDays(1),
                ModifiedDate = DateTime.Now.AddDays(5),
                TaskState = TaskStateEnum.Completed,
                LoggedInUser = "1"
            };

            // Act
            var response = homeApiController.UpdateTask(homeApiController.Request, taskViewModel);

            // Assert
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        private void SetFakeApiControllerContext()
        {
            homeApiController = new HomeApiController(pathFinder.Object)
            {
                Configuration = new HttpConfiguration(),
                Request = new HttpRequestMessage()
            };

            if (!string.IsNullOrEmpty(uri))
            {
                homeApiController.Request.RequestUri = new Uri(uri);
            }
        }

        private void InitialiseMocks()
        {
            queryOptions = new QueryOptions
            {
                FilterBy = "",
                LoggedInUser = "1"
            };
            pathFinder = new Mock<IPathFinder>();
            pathFinder.Setup(x => x.GetMccalFilePath()).Returns(mFilePath);
            pathFinder.Setup(x => x.GetRobertFilePath()).Returns(rFilePath);
        }
    }
}