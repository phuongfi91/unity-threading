using System;
using System.Collections.Generic;
using System.Threading;
using Threading;
using UnityEngine;

public class Example : MonoBehaviour
{
    private readonly Queue<Action> _actionQueue = new Queue<Action>();
    public Queue<Action> ActionQueue
    {
        get
        {
            lock (Async.GetLock("ActionQueue"))
            {
                return _actionQueue;
            }
        }
    }

    void Start()
    {
        WaitAndReport();
        WaitAndReportValue();
        CalcAndReportRepeatedly();
        CalcAndReportValueRepeatedly();
    }

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
}