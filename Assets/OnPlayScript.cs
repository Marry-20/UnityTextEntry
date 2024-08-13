
using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit;

#if !UNITY_EDITOR && UNITY_WSA
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

//[UnityEngine.AddComponentMenu("Scripts/MRTK/Examples/ColorTap")]
/// <summary>
/// It starts being executed when the user eye gaze hover on this (the keyboard character)
/// dwellIsFinished: True if the user eye gaze for feedbackDelayInSeconds duration
/// doubleDwellIsFinished:  True if the user eye gaze for dwellTimeInSecondsToSelect duration
/// thisCharacter: The current eye gazed character alphabet
/// ID: Is the ID of the keyboard characters corresponding to i and inverse_i ni keyboardscript
/// words: The predicted words received from the python server (through the function "testSocketServer")
/// displayTextLimit: The display string size of 20 to show in the text bar
/// </summary>
public class OnPlayScript : BaseEyeFocusHandler
{
    public Vector3 GazePosition;
    
    private Color color;

    private string thisCharacter;
    private string[] words;

    private bool dwellIsFinished = false;
    private bool doubleDwellIsFinished = false;

    private DateTime startTime_lookAt;
    private DateTime startTime_dwellFeedback;

    private float feedbackDelayInSeconds = 1f;
    private float dwellTimeInSecondsToSelect = 2.2f;

    private Int32 ID;
    private int displayTextLimit = 20;    

    protected override void OnEyeFocusStart()
    {
        dwellIsFinished = false;
        doubleDwellIsFinished = false;
        GazeInfo();
        this.transform.localScale += new Vector3(0.3f, 0.3f, 0.3f);
        startTime_lookAt = DateTime.UtcNow;
        // Remove the prediction panels if they are open from the previous predictions
        try
        {
            KeyBoardScript.instance.PredPanel.SetActive(false);

            for (int i = 0; i < 3; i++)
            {

                KeyBoardScript.instance.NxtSntLetter[i].SetActive(false);
                KeyBoardScript.instance.NxtSntWedge[i].SetActive(false);
                KeyBoardScript.instance.NxtWrdLetter[i].SetActive(false);
                KeyBoardScript.instance.NxtWrdrWedge[i].SetActive(false);

            }
        }
        catch
        {
            //Debug.Log("NxtSntLetter is Not active");
        }
    }

    protected override void OnEyeFocusStay()
    {
        if (this != null)
        {

            if (!dwellIsFinished && ((DateTime.UtcNow - startTime_lookAt).TotalSeconds > feedbackDelayInSeconds))
            {
                DwellFeedBack();
                dwellIsFinished = true;
            }
            else if (words[0] != "None" && !doubleDwellIsFinished && dwellIsFinished && ((DateTime.UtcNow - startTime_lookAt).TotalSeconds > dwellTimeInSecondsToSelect))
            {
                DoubleDwellFeedBack();
                doubleDwellIsFinished = true;

            }
        }
    }


    protected override void OnEyeFocusStop()
    {
        if (doubleDwellIsFinished)
        {
            this.transform.localScale -= new Vector3(0.3f, 0.3f, 0.3f);

        }
        this.gameObject.layer = LayerMask.NameToLayer("Default");
        GazePosition = CoreServices.InputSystem.EyeGazeProvider.HitPosition;
        KeyBoardScript.instance.list.Add(new Tuple<string, float, float, string>(DateTime.UtcNow.ToString(), GazePosition.x, GazePosition.y, this.name));
        this.transform.localScale -= new Vector3(0.3f, 0.3f, 0.3f);
        //this.color = UnityEngine.Random.ColorHSV(0.5f, 0.5f, 0.1f, 0.1f, 0.5f, 0.5f);
        KeyBoardScript.instance.Letters[ID].text = KeyBoardScript.instance.alphabet[ID]; 

    }

    void GazeInfo()
    {

        ID = Int32.Parse(this.name); // current keyboard character ID
        GazePosition = CoreServices.InputSystem.EyeGazeProvider.HitPosition; // current eye gaze ray cast location 
        KeyBoardScript.instance.list.Add(new Tuple<string, float,float, string>(DateTime.UtcNow.ToString(), GazePosition.x, GazePosition.y, this.name)); // Save gaze info into the list 
        thisCharacter = KeyBoardScript.instance.alphabet[ID];
        testSocketServer.instance.Send(KeyBoardScript.instance.flg + thisCharacter);   // Sending the total text typed on the text bar to the python server to predict the next word or suggest sentence.         
        }
    /// <summary>
    /// By dwelling at the key (DwellFeedBack function):
    /// 1. The character is selected and integrated with the previous typed text
    /// 2. The predicted next words and suggested sentences are received and put into "words"
    /// 3. words[4:6] may include up to three value for next predicted words and the rest are "None"
    /// 4. words[1:3] may include up to three predicted sentences and the rest are "None".
    /// 5. words[0] is the current word prediction 
    ///  For example if the current character is 'w', then the words[0] =  ['what'] , words[4:6] = ['is','are','do'], words[1:3] = ['what is the name of a recant movie?', 'what are the previous majors?',  'What can you do?']
    /// 6. Transform the position of the words coresponding to the position of the character object (NxtSntLetter)
    /// </summary>

    void DwellFeedBack()
    {

        this.gameObject.layer = LayerMask.NameToLayer("UI"); // Layer Priority changed

        KeyBoardScript.instance.flg += thisCharacter;
        string _mySocketString = testSocketServer.instance.Received;
        words = _mySocketString.Split(',');
        // current character is replaced with thw predicted word and by double dwelling at it it will be selected
        try
        {
            if (words[0] != "None")

            {
                KeyBoardScript.instance.Letters[ID].text = words[0];
            }
        }
        catch
        {
            // Debug.LogError("no data transfered! ");
        }

        // Sentence suggestion 
        for (int i = 0; i < 3; i++)
        {

            KeyBoardScript.instance.NxtSntWedge[i].transform.position =this.transform.position + new Vector3(0, -0.03f * i - 0.05f, 0);
            KeyBoardScript.instance.NxtSntLetter[i].GetComponent<TextMeshProUGUI>().text = words[i + 1];
            KeyBoardScript.instance.NxtSntLetter[i].SetActive(true);
            KeyBoardScript.instance.NxtSntWedge[i].SetActive(true);

        }

        // Next Words Prediction 
        for (int i = 3; i < 6; i++)
        {

            if (words[i + 1] != " " && words[i + 1] != "")
            {
                KeyBoardScript.instance.NxtWrdLetter[i - 3].GetComponent<TextMeshProUGUI>().text = words[i+1];
                KeyBoardScript.instance.NxtWrdrWedge[i - 3].transform.position = this.transform.position + new Vector3(0.05f * (i - 4), 0.06f - Mathf.Abs(i - 4) * 0.020f, 0);

                KeyBoardScript.instance.NxtWrdLetter[i - 3].SetActive(true);
                KeyBoardScript.instance.NxtWrdrWedge[i - 3].SetActive(true);


            }
        }
        // Locate the panel
        KeyBoardScript.instance.PredPanel.transform.position = this.transform.position - new Vector3(0, 0.08f, 0);
        KeyBoardScript.instance.PredPanel.SetActive(true);

        // Limit text displayed in text bar
        if (KeyBoardScript.instance.flg.Length > displayTextLimit)
        {
            KeyBoardScript.instance.TextBar.text = KeyBoardScript.instance.flg.Substring(KeyBoardScript.instance.flg.Length - displayTextLimit -1); 
        }
        else
        {
            KeyBoardScript.instance.TextBar.text = KeyBoardScript.instance.flg;
        }

    }

    // The current predicted word, words[0] is selected by double dwelling at it
    void DoubleDwellFeedBack()
    {
        if (KeyBoardScript.instance.flg.LastIndexOf(" ") == -1) // flg is empty or include a typing word e.g. "appl" then is fully replaced with the words[0] 

        {
            KeyBoardScript.instance.flg = words[0];
        }
        else //flg is not empty and include some previously typed words and also the current typing word e.g. "How can I eat appl" then appl is just replaced with the words[0]
        {
            KeyBoardScript.instance.flg = KeyBoardScript.instance.flg.Substring(0, KeyBoardScript.instance.flg.LastIndexOf(" "));
            KeyBoardScript.instance.flg += words[0];
        }
        KeyBoardScript.instance.flg += "   ";

        if (KeyBoardScript.instance.flg.Length > displayTextLimit)
        {
            KeyBoardScript.instance.TextBar.text = KeyBoardScript.instance.flg.Substring(KeyBoardScript.instance.flg.Length - displayTextLimit);
        }
        else
        {
            KeyBoardScript.instance.TextBar.text = KeyBoardScript.instance.flg;
        }

        this.transform.localScale += new Vector3(0.3f, 0.3f, 0.3f);

    }


   

}

