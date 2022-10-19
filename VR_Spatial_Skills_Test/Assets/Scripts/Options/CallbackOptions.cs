using System;
using UnityEngine;
using UnityEngine.Events;

/*
 Part of the OpenFracture Open Source Project by Greenheck D., Dearborn J
*/
[Serializable]
public class CallbackOptions
{
    [Tooltip("This callback is invoked when the fracturing/slicing process has been completed.")]
    public UnityEvent onCompleted;

    public CallbackOptions()
    {
        this.onCompleted = null;
    }
}