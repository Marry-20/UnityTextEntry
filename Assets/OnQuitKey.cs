
using Microsoft.MixedReality.Toolkit.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

#if !UNITY_EDITOR && UNITY_WSA
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif
// Quit the Text Entry App by dwelling at the Stop Object in coordinator script (This)

public class OnQuitKey : BaseEyeFocusHandler
{

    private bool IsFinished = true;
    private DateTime startTime_lookAt;
    private float feedbackDelayInSeconds = 2f;

    protected override void OnEyeFocusStart()
    {
        if (coordinator.instance.Keyboard.activeSelf == true) {
        startTime_lookAt = DateTime.UtcNow;
        this.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);

        IsFinished = false;

    } }

    protected override void OnEyeFocusStay()
    {
        if (this != null)
        {

            if (!IsFinished && ((DateTime.UtcNow - startTime_lookAt).TotalSeconds > feedbackDelayInSeconds))
            {
                this.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
                coordinator.instance.Keyboard.SetActive(!coordinator.instance.Keyboard.activeSelf);

                //save data including the typed texts
                SaveData("Report", KeyBoardScript.instance.list);

                KeyBoardScript.instance.TextBar.text = "";
                IsFinished = true;
            }

        }
    }
    protected override void OnEyeFocusStop()
    {

        startTime_lookAt = DateTime.UtcNow;
        if (!IsFinished)
        {
            this.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
        }

    }

    
    public void SaveData(string filename, List<Tuple<string,float,float,string>> obj)
    {
        string path = string.Format("{0}/{1}.json", Application.persistentDataPath, filename);

        string json = JsonConvert.SerializeObject(obj);
        byte[] data = Encoding.ASCII.GetBytes(json);

        UnityEngine.Windows.File.WriteAllBytes(path, data);
    }
}

