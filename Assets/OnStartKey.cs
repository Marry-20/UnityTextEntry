
using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;

#if !UNITY_EDITOR && UNITY_WSA
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif
// Start or Pause the Text Entry App by dwelling at the StartPause Object in coordinator script (This)
public class OnStartKey : BaseEyeFocusHandler
{

    private bool IsFinished = true;   
    private DateTime startTime_lookAt;
    private float feedbackDelayInSeconds = 2f;

    protected override void OnEyeFocusStart()
    {
        
        startTime_lookAt = DateTime.UtcNow;
        this.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);

        IsFinished = false;  // After a glance on StartPause Object in coordinator script (This)
    }

    protected override void OnEyeFocusStay()
    {
        if (this != null)
        {

            if (!IsFinished && ((DateTime.UtcNow - startTime_lookAt).TotalSeconds > feedbackDelayInSeconds))
            {
                this.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
                coordinator.instance.Keyboard.SetActive(!coordinator.instance.Keyboard.activeSelf); // Start or Pause the Text Entry App after dwelling for feedbackDelayInSeconds time on StartPause Object in coordinator script (This)

                IsFinished = true;  
            }

        }
    }

    protected override void OnEyeFocusStop()
    {

        startTime_lookAt = DateTime.UtcNow;

        if (!IsFinished) { 
        this.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f); }

    }
}

