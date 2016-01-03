using System;
using System.Threading.Tasks;

namespace DNTScheduler.TestWebApplication.WebTasks
{
    public class DoBackupTask : ScheduledTaskTemplate
    {
        /// <summary>
        /// If you have multiple jobs at the same time, this value indicates the order of their execution.
        /// </summary>
        public override int Order
        {
            get { return 2; }
        }

        public override bool RunAt(DateTime utcNow)
        {
            if (this.IsShuttingDown || this.Pause)
                return false;

            var now = utcNow.AddHours(3.5);
            return (now.Day % 3 == 0) && (now.Hour == 0 && now.Minute == 1 && now.Second == 1);
            /*(now.DayOfWeek == DayOfWeek.Friday) &&
                   (now.Hour == 3) &&
                   (now.Minute == 1) &&
                   (now.Second == 1)*/
            //now.Hour == 23 && now.Minute == 33 && now.Second == 1;
        }

        public override void Run()
        {
            if (this.IsShuttingDown || this.Pause)
                return;

            System.Diagnostics.Trace.WriteLine("Running Do Backup");
        }

        public override Task RunAsync()
        {
            return base.RunAsync();
        }

        public override string Name
        {
            get { return "تهيه پشتيبان"; }
        }
    }
}