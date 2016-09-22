using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(DNTScheduler.AddModules), "PreStart")]
[assembly: PostApplicationStartMethod(typeof(DNTScheduler.AddModules), "PostStart")]

namespace DNTScheduler
{
    /// <summary>
    /// Registers HTTP modules dynamically.
    /// </summary>
    public static class AddModules
    {
        /// <summary>
        /// Registering the DNTSchedulerModule automatically.
        /// </summary>
        public static void PreStart()
        {
            DynamicModuleUtility.RegisterModule(typeof(DNTSchedulerModule));
        }

        /// <summary>
        ///
        /// </summary>
        public static void PostStart()
        {
        }
    }
}