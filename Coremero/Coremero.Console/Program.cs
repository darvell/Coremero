using System;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        var exitEvent = new ManualResetEvent(false);

        Console.CancelKeyPress += (sender, eventArgs) => {
            eventArgs.Cancel = true;
            exitEvent.Set();
        };

        Coremero.Coremero.Initialize();
        exitEvent.WaitOne();
    }
}