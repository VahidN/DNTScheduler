using System;
using System.Threading.Tasks;

namespace DNTScheduler
{
    /// <summary>
    /// DNTScheduler needs a ping service to keep it alive.
    /// </summary>
    public class PingTask : ScheduledTaskTemplate
    {
        /// <summary>
        /// If you have multiple jobs at the same time, this value indicates the order of their execution.
        /// </summary>
        public override int Order
        {
            get { return 1; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="utcNow"></param>
        /// <returns></returns>
        public override bool RunAt(DateTime utcNow)
        {
            if (this.IsShuttingDown || this.Pause)
                return false;

            return utcNow.Second == 1;
        }

        /// <summary>
        ///
        /// </summary>
        public override void Run()
        {
            if (this.IsShuttingDown || this.Pause)
                return;

            ThisApp.WakeUp();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override Task RunAsync()
        {
            return base.RunAsync();
        }

        /// <summary>
        ///
        /// </summary>
        public override string Name
        {
            get { return "Ping Task"; }
        }
    }
}