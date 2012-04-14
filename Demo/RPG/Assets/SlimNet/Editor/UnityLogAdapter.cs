using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SlimNetUnityLogAdapter : SlimNet.ILogAdapter
{
    public void Log(SlimNet.Log.LogEvent @event)
    {
        switch (@event.Level)
        {
            case SlimNet.LogLevel.Error:
                Debug.LogError(@event.Message);
                break;

            case SlimNet.LogLevel.Warn:
                Debug.LogWarning(@event.Message);
                break;

            default:
                Debug.Log(@event.Message);
                break;
        }
    }
}
