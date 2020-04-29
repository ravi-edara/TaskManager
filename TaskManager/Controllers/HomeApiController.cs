using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;
using TaskManager.Helper;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    [RoutePrefix("api/home")]
    public class HomeApiController : ApiController
    {
        private string rFilePath = @"C:\Projects\TaskManager\TaskManager\App_Data\rTaskData.json";
        private string mFilePath = @"C:\Projects\TaskManager\TaskManager\App_Data\mTaskData.json";
        private string filePath = string.Empty;
        public readonly IPathFinder pathFinder;

        public HomeApiController()
        {
            pathFinder = new PathFinder();
        }

        public HomeApiController(IPathFinder _pathFinder)
        {
            pathFinder = _pathFinder ?? new PathFinder();
        }

        [Route("")] // List
        public HttpResponseMessage GetTaskSet(HttpRequestMessage request, [FromUri] QueryOptions queryOptions)
        {
            filePath = GetFilePathByLoggedInUser(queryOptions.LoggedInUser);

            List<TaskViewModel> taskViewModelList = GetTasks();

            if (!string.IsNullOrEmpty(queryOptions.FilterBy))
            {
                taskViewModelList = taskViewModelList.Where(x => x.Title.ToLower().Contains(queryOptions.FilterBy.ToLower())).ToList();
            }

            return request.CreateResponse(HttpStatusCode.OK, taskViewModelList);
        }

        [HttpGet]
        [Route("{id}")] // Edit Get
        public HttpResponseMessage GetTaskById(HttpRequestMessage request, int? id, string loggedInUser)
        {
            filePath = GetFilePathByLoggedInUser(loggedInUser);

            List<TaskViewModel> taskViewModelList = GetTasks();
            TaskViewModel returnData = new TaskViewModel();

            if (id.HasValue && id > 0)
            {
                returnData = taskViewModelList.Where(x => x.Id == id).SingleOrDefault();
            }

            return request.CreateResponse(HttpStatusCode.OK, returnData);
        }

        [HttpPost]
        [Route("")]  // Create Post
        public HttpResponseMessage CreateTask(HttpRequestMessage request, [FromBody]TaskViewModel taskModel)
        {
            return SaveTask(request, taskModel);
        }

        [HttpPut]
        [Route("")]  // Update Put
        public HttpResponseMessage UpdateTask(HttpRequestMessage request, [FromBody]TaskViewModel taskModel)
        {
            return SaveTask(request, taskModel);
        }

        private HttpResponseMessage SaveTask(HttpRequestMessage request, TaskViewModel taskModel)
        {
            var isErrorfound = false;

            // validate model.
            if (ModelState.IsValid)
            {
                filePath = GetFilePathByLoggedInUser(taskModel.LoggedInUser);

                List<TaskViewModel> items = GetTasks();

                if (taskModel != null && taskModel.Id <= 0) // create the task
                {
                    taskModel.Id = items.Count() + 1;
                    items.Add(taskModel);
                    File.WriteAllText(filePath, JsonConvert.SerializeObject(items.ToArray()));
                }
                else if (taskModel != null && taskModel.Id > 0) // update the Task
                {
                    var taskToUpdate = items.Where(x => x.Id == taskModel.Id).SingleOrDefault();
                    if (taskToUpdate != null)
                    {
                        taskToUpdate.Title = taskModel.Title;
                        taskToUpdate.Description = taskModel.Description;
                        taskToUpdate.CreatedDate = taskModel.CreatedDate;
                        taskToUpdate.ModifiedDate = taskModel.ModifiedDate;
                        taskToUpdate.TaskState = taskModel.TaskState;
                    }
                    File.WriteAllText(filePath, JsonConvert.SerializeObject(items.ToArray()));
                }

                var result = new
                {
                    Task = taskModel
                };

                return isErrorfound
                    ? request.CreateResponse(HttpStatusCode.Conflict, result)
                    : request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, ModelStateErrors());
            }
        }

        private IEnumerable<JsonValidationError> ModelStateErrors()
        {
            var errorList = new List<JsonValidationError>();
            foreach (var key in ModelState.Keys)
            {
                ModelState modelState = null;
                if (ModelState.TryGetValue(key, out modelState))
                {
                    errorList.AddRange(modelState.Errors.Select(error => new JsonValidationError()
                    {
                        Key = key,
                        Message = error.ErrorMessage
                    }));
                }
            }
            return errorList;
        }

        private List<TaskViewModel> GetTasks()
        {
            List<TaskViewModel> items = new List<TaskViewModel>();

            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<TaskViewModel>>(json) ?? new List<TaskViewModel>();
            }
            return items;
        }

        private string GetFilePathByLoggedInUser(string loggedInUserId)
        {
            string filePath = pathFinder.GetRobertFilePath();

            if (!string.IsNullOrEmpty(loggedInUserId))
            {
                filePath = loggedInUserId == "1" ? pathFinder.GetRobertFilePath() : pathFinder.GetMccalFilePath();
            }

            return filePath;
        }
    }
}

public class JsonValidationError
{
    public string Key { get; set; }

    public string Message { get; set; }
}

public interface IPathFinder
{
    string GetRobertFilePath();

    string GetMccalFilePath();
}

public class PathFinder : IPathFinder
{
    private string rFilePath = @"C:\Projects\TaskManager\TaskManager\App_Data\rTaskData.json";
    private string mFilePath = @"C:\Projects\TaskManager\TaskManager\App_Data\mTaskData.json";

    public string GetRobertFilePath()
    {
        return rFilePath;
    }

    public string GetMccalFilePath()
    {
        return mFilePath;
    }
}