using System;
using System.Linq;

namespace DNTScheduler.TestWebApplication
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TasksGridView.DataSource = ScheduledTasksCoordinator.Current.ScheduledTasks.Select(x => new
            {
                TaskName = x.Name,
                LastRunTime = x.LastRun,
                LastRunWasSuccessful = x.IsLastRunSuccessful,
                IsPaused = x.Pause,
            }).ToList();
            TasksGridView.DataBind();
        }
    }
}