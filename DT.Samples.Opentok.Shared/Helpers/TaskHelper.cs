using System;
using System.Threading.Tasks;

namespace DT.Samples.Opentok.Shared
{
    public static class TaskHelper
    {
        public static void NoWait(this Task task)
        {
            task.ContinueWith(t =>
            {
                if(t.Exception != null)
                {
                    //smth went wrong
                }
            });
        }
    }
}
