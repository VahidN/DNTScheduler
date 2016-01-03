using System;
using System.Threading;
using ThreadTimer = System.Threading.Timer;

namespace DNTScheduler
{
    internal class JobsRunnerTimer
    {
        private ThreadTimer _threadTimer; //keep it alive

        public Action OnTimerCallback { set; get; }

        public void Start(long startAfter = 1000, long interval = 1000)
        {
            _threadTimer = new ThreadTimer(timerCallback, null, Timeout.Infinite, 1000);
            _threadTimer.Change(startAfter, interval);
        }

        public void Stop()
        {
            if (_threadTimer != null)
                _threadTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void timerCallback(object state)
        {
            if (OnTimerCallback != null)
                OnTimerCallback();
        }
    }
}