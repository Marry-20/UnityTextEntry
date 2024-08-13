using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;

#if !UNITY_EDITOR && UNITY_WSA
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

// This performs if the user eye gaze at the corresponding suggested sentence 
public class PlayAutoComp : BaseEyeFocusHandler
{
    private bool IsFinished = true;
    private DateTime startTime_lookAt;
    private DateTime startTime_dwellFeedback;
    private float feedbackDelayInSeconds = 1.5f;
    private float dwellTimeInSecondsToSelect = 2.0f;
    private string mySocketString;
    private string AutoPredSent;
    public static PlayAutoComp instance = null;
    private string log;

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
                //ProgressbarRef.progress = time / Mathf.Max(feedbackDelayInSeconds, SmallEpsilon);
            }
            

                           //KeyBoardScript.instance.SquarePlayIsFinished = true;

        }

    }


    protected override void OnEyeFocusStop()
    {


        // KeyBoardScript.instance.PredPanel.GetComponent<CapsuleCollider>().enabled = true;
        if (IsFinished == true) { 
        this.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);}

        this.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
        //KeyBoardScript.instance.Letters[KeyBoardScript.instance.ID].text = KeyBoardScript.instance.alphabet[KeyBoardScript.instance.ID];
}
    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////
    /// </summary>
    


    
    void DwellFeedBack()
    {

        string Text = this.GetComponent<TextMeshProUGUI>().text;
        this.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);

        KeyBoardScript.instance.TextBar.text = Text;
        IsFinished = true;


    }
    

}

