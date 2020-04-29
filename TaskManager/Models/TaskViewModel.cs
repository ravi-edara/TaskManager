using System;
using System.ComponentModel.DataAnnotations;
using TaskManager.Helper;

namespace TaskManager.Models
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime ModifiedDate { get; set; }

        [Required]
        public TaskStateEnum TaskState { get; set; }

        public string TaskStateValue { get { return TaskState.ToString(); } }

        public string LoggedInUser { get; set; }
    }
}