
using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;
using TMPro;

#if !UNITY_EDITOR && UNITY_WSA
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

// This performs if the user eye gaze at one of the predicted words 

public class PlayAutoWrd : BaseEyeFocusHandler
{
    private bool IsFinished = true;
    private DateTime startTime_lookAt;
    private float feedbackDelayInSeconds = 2f;
    public static PlayAutoComp instance = null;
  
    protected override void OnEyeFocusStart()
    {

        startTime_lookAt = DateTime.UtcNow;
        this.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
        IsFinished = false;


    }

    protected override void OnEyeFocusStay()
    {
        if (this != null)
        {

            if (!IsFinished && ((DateTime.UtcNow - startTime_lookAt).TotalSeconds > feedbackDelayInSeconds))
            {
                DwellFeedBack();
            }

        }
    }


    protected override void OnEyeFocusStop()
    {

        this.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
    }





    void DwellFeedBack()
    {

        string Text = this.GetComponentInChildren<TextMeshProUGUI>().text;

        KeyBoardScript.instance.TextBar.text = KeyBoardScript.instance.TextBar.text + Text+" ";
        KeyBoardScript.instance.flg =  KeyBoardScript.instance.TextBar.text;

        IsFinished = true;

    }




}

