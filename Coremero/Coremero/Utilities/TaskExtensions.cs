using System.Threading.Tasks;

namespace Coremero.Utilities
{
    public static class TaskExtensions
    {
        public static void Forget(this Task task)
        {
            task.ContinueWith(
                t =>
                {
                    if (task.Exception != null) Log.Error(task.Exception.ToString());
                },
                TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}