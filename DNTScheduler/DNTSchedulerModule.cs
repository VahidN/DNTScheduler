using System;
using System.Web;

namespace DNTScheduler
{
    /// <summary>
    ///
    /// </summary>
    public class DNTSchedulerModule : IHttpModule
    {
        static DNTSchedulerModule()
        {
            AppDomain.CurrentDomain.DomainUnload += currentDomainUnload;
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        { }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += contextBeginRequest;
        }

        private static void contextBeginRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;

            if (string.IsNullOrWhiteSpace(ThisApp.SiteRootUrl))
            {
                ThisApp.SiteRootUrl = string.Format("{0}{1}", context.Request.Url.GetLeftPart(UriPartial.Authority), context.Request.ApplicationPath);
            }
        }

        private static void currentDomainUnload(object sender, EventArgs e)
        {
            ScheduledTasksCoordinator.Current.Dispose();
            try
            {
                ThisApp.WakeUp();
            }
            catch (Exception ex)
            {
                if (ScheduledTasksCoordinator.Current.OnUnexpectedException != null)
                    ScheduledTasksCoordinator.Current.OnUnexpectedException(ex, new PingTask());
            }
        }
    }
}