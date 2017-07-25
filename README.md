# unity-threading
Simple implementation to easily perform multithreaded asynchronous operations in Unity3D.
All feedbacks, contributions are very much welcomed.

# Example usages

### Wait for a task to finish executing before returning back to main Unity thread

    private void WaitAndReport()
    {
        Async.Run(() =>
        {
            Thread.Sleep(1000);
        }).ContinueInMainThread(() =>
        {
            Debug.Log("Done waiting");
        });
    }

### Wait for a task to finish executing before returning a value back to main Unity thread

    private void WaitAndReportValue()
    {
        Async.Run(() =>
        {
            Thread.Sleep(1000);
            return 10;
        }).ContinueInMainThread((result) =>
        {
            Debug.Log("Done. Value is " + result);
        });
    }
        
### Perform a background operation then return the value to main thread once done. The background task is repeated every second (1000 ms)

    private void CalcAndReportValueRepeatedly()
    {
        Async.RunInBackground("CalcAndReportValueRepeatedly", 1000, () =>
        {
            long value = DateTime.UtcNow.Ticks;
            return value;
        }).ContinueInMainThread((result) =>
        {
            DateTime dateTime = DateTime.FromBinary(result);
            Debug.Log("DateTime value: " + dateTime);
        });
    }

### Execute a background task, enqueue the results into a threadsafe action queue to be printed out once the background task is done. The background task is repeated every second (1000 ms)

    private readonly Queue<Action> _actionQueue = new Queue<Action>();
    public Queue<Action> ActionQueue
    {
        get
        {
            // Async class provide lock object with unique keyword identifier
            // for thread-safe operations
            lock (Async.GetLock("ActionQueue"))
            {
                return _actionQueue;
            }
        }
    }
    
    private void CalcAndReportRepeatedly()
    {
        Async.RunInBackground("CalcAndReportRepeatedly", 1000, () =>
        {
            long date1 = DateTime.UtcNow.Ticks;
            Thread.Sleep(50);
            long date2 = DateTime.UtcNow.Ticks;
            ActionQueue.Enqueue(() =>
            {
                Debug.Log("date1: " + date1 + " + date2: " + date2 + " = " + (date1 + date2));
            });
        }).ContinueInMainThread(() =>
        {
            Action action = ActionQueue.Dequeue();
            action();
        });
    }
    
# Note
Call Dispatcher.Instance.Reset() to abort and clear all scheduled background or threaded tasks if needed. For example, it's useful when your application restart.
