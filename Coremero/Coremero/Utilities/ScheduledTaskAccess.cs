using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Coremero.Utilities
{
    public static class ScheduledTaskAccess
    {
        public static Task[] GetScheduledTasksForDebugger(this TaskScheduler ts)
        {
            var mi = ts.GetType()
                .GetMethod("GetScheduledTasksForDebugger", BindingFlags.NonPublic | BindingFlags.Instance);
            if (mi == null)
                return null;
            return (Task[]) mi.Invoke(ts, new object[0]);
        }

        public static TaskScheduler[] GetTaskSchedulersForDebugger()
        {
            var mi = typeof(TaskScheduler).GetMethod("GetTaskSchedulersForDebugger",
                BindingFlags.NonPublic | BindingFlags.Static);
            if (mi == null)
                return null;
            return (TaskScheduler[]) mi.Invoke(null, new object[0]);
        }
    }
}