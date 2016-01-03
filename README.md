DNTScheduler
=======
DNTScheduler is a super simple ASP.NET's background tasks runner and scheduler.
It's compatible with .NET 4.0+

Install via NuGet
-----------------
To install DNTScheduler, run the following command in the Package Manager Console:

```
PM> Install-Package DNTScheduler
```

You can also view the [package page](http://www.nuget.org/packages/DNTScheduler/) on NuGet.

## Usage
* To start using DNTScheduler package, download its source code from the GitHub and then take a look at these files:
  - How to define a new task:
       [DNTScheduler.TestWebApplication\WebTasks\DoBackupTask.cs](/DNTScheduler.TestWebApplication/WebTasks/DoBackupTask.cs)
  - How to register it:
       [DNTScheduler.TestWebApplication\WebTasks\ScheduledTasksRegistry.cs](/DNTScheduler.TestWebApplication/WebTasks/ScheduledTasksRegistry.cs)
  - How to initialize it:
       [DNTScheduler.TestWebApplication\Global.asax.cs](/DNTScheduler.TestWebApplication/Global.asax.cs)
  - How to get the list of active tasks:
       [DNTScheduler.TestWebApplication\Default.aspx.cs](/DNTScheduler.TestWebApplication/Default.aspx.cs)