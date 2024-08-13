using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;


#if !UNITY_EDITOR && UNITY_WSA
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

/// <summary>
///  Delete, Space and Word Delete Fucntions Class
/// </summary>
public class Dele : BaseEyeFocusHandler
{
    private bool IsFinished = true;

    private DateTime startTime_lookAt;
    private DateTime startTime_dwellFeedback;

    private float feedbackDelayInSeconds = 1f;
    private float dwellTimeInSecondsToSelect = 2.0f;

    private string mySocketString;
    private string AutoPredSent;
    private string currentChar;

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
             currentChar = KeyBoardScript.instance.alphabet[Int32.Parse(this.name)];


            if (!IsFinished && ((DateTime.UtcNow - startTime_lookAt).TotalSeconds > feedbackDelayInSeconds))
            {
                DwellFeedBack();
                this.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);

            }



        }

    }


    protected override void OnEyeFocusStop()
    {


        // KeyBoardScript.instance.PredPanel.GetComponent<CapsuleCollider>().enabled = true;
        if (!IsFinished) {
        this.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f); }
        //KeyBoardScript.instance.Letters[KeyBoardScript.instance.ID].text = KeyBoardScript.instance.alphabet[KeyBoardScript.instance.ID];
    }
    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////
    /// </summary>




    void DwellFeedBack()
    {
        if (currentChar == "WDel")
        {
            WrdErsFun(KeyBoardScript.instance.TextBar.text);

        }
        else if (currentChar == "Del")
        {
            EraseFun(KeyBoardScript.instance.TextBar.text);

        }
        else if (currentChar == "Spc")
        {
            SpcFun(KeyBoardScript.instance.TextBar.text);

        }
        KeyBoardScript.instance.TextBar.text = KeyBoardScript.instance.flg;

        IsFinished = true;


    }

    void EraseFun(String text)
    {
        if (text.Length > 0)
        {
            KeyBoardScript.instance.flg = text.Remove(text.Length - 1, 1);

        }
    }
    void SpcFun(String text)
    {
        if (text.Length > 0)
        {
            KeyBoardScript.instance.flg = text + " ";
        }
    }
    void WrdErsFun(String text)
    {
        if (text.Length > 0)
        {
            var lastSpaceIndex = text.Substring(0, text.Length - 1).LastIndexOf(' ');
            if (lastSpaceIndex == -1)
            {
                KeyBoardScript.instance.flg = "";
            }
            else
            {
                KeyBoardScript.instance.flg = text.Substring(0, lastSpaceIndex) + " ";

            }
            KeyBoardScript.instance.TextBar.text = KeyBoardScript.instance.flg;
        }
    }
}

