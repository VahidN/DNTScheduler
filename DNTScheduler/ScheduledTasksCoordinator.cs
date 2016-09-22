using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DNTScheduler
{
    /// <summary>
    /// Scheduled Tasks Manager
    /// </summary>
    public sealed class ScheduledTasksCoordinator : System.Web.Hosting.IRegisteredObject, IDisposable
    {
        // the 30 seconds is for the entire app to tie up what it's doing.
        private const int TimeToFinish = 30 * 1000;

        private readonly object _syncLock = new object();
        private readonly List<ScheduledTaskTemplate> _tasks = new List<ScheduledTaskTemplate>();
        private readonly JobsRunnerTimer _timer = new JobsRunnerTimer();
        private int _disposed;
        private bool _isShuttingDown;
        private Thread _taskThread;

        /// <summary>
        /// Scheduled Tasks Manager
        /// </summary>
        private ScheduledTasksCoordinator()
        {
            System.Web.Hosting.HostingEnvironment.RegisterObject(this);
            _tasks.Add(new PingTask());
            _timer.Start();
        }

        /// <summary>
        /// Stops the scheduler.
        /// </summary>
        ~ScheduledTasksCoordinator()
        {
            Dispose();
        }

        /// <summary>
        /// The only instance of the ScheduledTasksCoordinator.
        /// </summary>
        public static ScheduledTasksCoordinator Current
        {
            get
            {
                return Nested.CurrentInstance;
            }
        }

        /// <summary>
        /// Fires on unhandled exceptions.
        /// </summary>
        public Action<Exception, ScheduledTaskTemplate> OnUnexpectedException { set; get; }

        /// <summary>
        /// Gets the list of the scheduled tasks.
        /// </summary>
        public ScheduledTaskTemplate[] ScheduledTasks
        {
            get { return _tasks.ToArray(); }
        }

        /// <summary>
        /// Adds a new scheduled task.
        /// </summary>
        /// <param name="scheduledTask">new task</param>
        public void AddScheduledTask(ScheduledTaskTemplate scheduledTask)
        {
            _tasks.Add(scheduledTask);
        }

        /// <summary>
        /// Adds new scheduled tasks.
        /// </summary>
        /// <param name="scheduledTasks">Tasks list</param>
        public void AddScheduledTasks(params ScheduledTaskTemplate[] scheduledTasks)
        {
            foreach (var task in scheduledTasks)
            {
                _tasks.Add(task);
            }
        }

        /// <summary>
        /// Stops the scheduler.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Increment(ref _disposed) != 1)
                return;

            Stop();
            System.Web.Hosting.HostingEnvironment.UnregisterObject(this);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Removes a task from the list.
        /// </summary>
        /// <param name="taskName">name of the task to remove</param>
        public void RemoveTask(string taskName)
        {
            lock (_syncLock)
            {
                var task = _tasks.FirstOrDefault(taskTemplate => taskTemplate.Name == taskName);
                if (task == null)
                {
                    throw new InvalidOperationException(string.Format("{0} not found and is not registered.", taskName));
                }
                _tasks.Remove(task);
            }
        }

        /// <summary>
        /// Starts TimerCallback.
        /// </summary>
        public void Start()
        {
            _timer.OnTimerCallback = () =>
            {
                var now = DateTime.UtcNow;
                var taskToRun = _tasks.Where(x => !x.IsRunning && x.RunAt(now)).OrderBy(x => x.Order).ToList();
                if (_isShuttingDown || !taskToRun.Any())
                    return;

                _taskThread = new Thread(() => taskAction(taskToRun))
                {
                    Priority = ThreadPriority.BelowNormal,
                    IsBackground = true
                };
                _taskThread.Start();
            };
        }

        /// <summary>
        /// Stops the scheduler.
        /// </summary>
        public void Stop()
        {
            Dispose();
        }

        /// <summary>
        /// Call if the app is shutting down. Should only be called by the ASP.Net container.
        /// </summary>
        /// <param name="immediate">ASP.Net sets this to false first, then to true the second
        /// call 30 seconds later.</param>
        public void Stop(bool immediate)
        {
            // See: http://haacked.com/archive/2011/10/16/the-dangers-of-implementing-recurring-background-tasks-in-asp-net.aspx

            if (_isShuttingDown)
                return;

            lock (_syncLock)
            {
                _isShuttingDown = true;

                foreach (var scheduledTask in _tasks)
                {
                    scheduledTask.IsShuttingDown = true;
                }

                var timeOut = TimeToFinish;
                while (_tasks.Any(x => x.IsRunning) && timeOut >= 0)
                {
                    // still running ...
                    Thread.Sleep(50);
                    timeOut -= 50;
                }
                Dispose();
            }
        }

        private void taskAction(IEnumerable<ScheduledTaskTemplate> taskToRun)
        {
            foreach (var scheduledTask in taskToRun)
            {
                try
                {
                    scheduledTask.IsRunning = true;
                    scheduledTask.LastRun = DateTime.UtcNow;

                    scheduledTask.Run();
                    scheduledTask.RunAsync().Wait();

                    scheduledTask.IsLastRunSuccessful = true;
                }
                catch (Exception ex)
                {
                    scheduledTask.IsLastRunSuccessful = false;
                    OnUnexpectedException(ex, scheduledTask);
                }
                finally
                {
                    scheduledTask.IsRunning = false;
                }
            }
        }

        /// <summary>
        /// http://www.yoda.arachsys.com/csharp/singleton.html
        /// </summary>
        class Nested
        {
            internal static readonly ScheduledTasksCoordinator CurrentInstance = new ScheduledTasksCoordinator();

            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }
        }
    }
}