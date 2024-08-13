using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit;
using Newtonsoft.Json;
using System.Text;

// This is the text entry keyboard control script
public class KeyBoardScript : MonoBehaviour//, IPointerEnterHandler
{


    // This list is filled in OnPlayScript script during typing in every time frame to include
    // <(Time, Horizontal GazePosition, GazePosition, the name of the gazzed character))>
    public List<Tuple<string, float,float,string>> list = new List<Tuple<string, float,float,string>>(); 

    public Image[] wrdWedge = new Image[3]; // Image of the 3 predicted words 
    public Image[] newWedge = new Image[30];
    private Image BarWidget ; // The center of the keyboard where the typing test is displayed 

    // keyboard characters objects prefabs
    public GameObject wedgePrefab;
    public GameObject DelPrefab;
    public GameObject WrdSentPrefab; // Nest Words prefab
    public GameObject SentPrefab;  // Next Sentence Widget Prefab and Text Bar Prefab
    public GameObject NxtSenPrefab; // Nest Sentence Letter Prefab
    public GameObject NxtWrdPrefab;  // Next words letter prefab
    public GameObject PredPanel; // The panel incluing the predictions

    // Prediction objects and contents
    public GameObject[]  NxtSntWedge= new GameObject[3];
    public GameObject[] NxtWrdrWedge = new GameObject[3]; 
    public GameObject[] NxtWrdLetter = new GameObject[3]; 
    public GameObject[] NxtSntLetter = new GameObject[3]; 

    private Color[] WedgeColors = new Color[30];
    // Content Game Object
    public TextMeshProUGUI TextBar;
    public TextMeshProUGUI[] Letters = new TextMeshProUGUI[30];
    public TextMeshProUGUI LettersPrefab;

    // The circular keyboard consists of 3 rings. Number of Keyboard characters in every ring:
    private int sliceNum1, sliceNum2, sliceNum3; 

    // Keyboard objects coordinates
    public float ringRedius;
    public float ringWidth;
    private float[] centerAngle = new float[30];
    public float[] x = new float[30];
    public float[] y = new float[30];


    public string[] alphabet = {".?123", "a", "b", "c", "d", "e", "f", "g", "h", "i", "k", "z", "l", "m", "n", "o", "p", "q", "r", "j", "s", "t", "u", "v", "x", "w", "y", "Spc", "WDel",  "Del" };
    public string flg;  // The total text that is displayed in the text bar and typed so far
    public string AllWords;
    public String[] EachWords = new string[3]; // The 3 predicted words string text
    public bool SquarePlayIsFinished = true;

    // Creating Instance from the prefab to call in other scripts
    public static KeyBoardScript instance = null;

    // Add all text entry character to a list to edit all together in case
    private List<GameObject> activeWedges;
    public List<GameObject> ActiveWedges
    {
        get { return activeWedges; }
        set { activeWedges = value; }
    }

    private List<GameObject> activeBars;
    public List<GameObject> ActiveBars
    {
        get { return activeBars; }
        set { activeBars = value; }
    }


    private void Awake()
    {


        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        activeWedges = new List<GameObject>();
        activeBars = new List<GameObject>();

    }



    void Start()
    {

        sliceNum1 = 14;
        sliceNum2 = 10;
        sliceNum3 = 6;

        // setting up the typing text bar
        BarWidget = Instantiate(SentPrefab.GetComponent<Image>(), new Vector3(0, 0, 0), Quaternion.identity) as Image;
        BarWidget.enabled = true;
        BarWidget.transform.SetParent(transform, false);
        TextBar = Instantiate(TextBar.GetComponent<TextMeshProUGUI>(), new Vector3(0, 0, 0), Quaternion.identity) ;
        TextBar.transform.SetParent(BarWidget.transform, false);
        // Initiating every ring of the keyboard
        instantiatefunction1(sliceNum1, 150, alphabet);
        instantiatefunction2(sliceNum2, 95, alphabet);
        instantiatefunction3(sliceNum3, 40, alphabet);
        // setting up the prediction panel
        PredPanel = Instantiate(PredPanel, new Vector3(0, 0, 0), Quaternion.identity);
        PredPanel.SetActive(false);
        PredPanel.transform.SetParent(transform, false);
        PredPanel.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.2f;
        PredPanel.layer = LayerMask.NameToLayer("TransparentFX");

        // setting up the prediction game objects (next sentence suggestion object, next word prediciton object)
        instantiatefunction();

    }

    void instantiatefunction()
    {

        for (int i = 0; i < 3; i++)
        {

            NxtWrdrWedge[i] = Instantiate(WrdSentPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            NxtWrdrWedge[i].name = "NxtWrd" + i.ToString();
            NxtWrdrWedge[i].transform.SetParent(transform, false);

            NxtWrdrWedge[i].SetActive(false);
            NxtWrdrWedge[i].GetComponent<Image>().alphaHitTestMinimumThreshold = 0.2f;
            NxtWrdLetter[i] = Instantiate(NxtWrdPrefab);
            //NxtWrdLetter[i].transform.localScale += new Vector3 (-0.7f,2,0);

            NxtWrdLetter[i].GetComponent<TextMeshProUGUI>().text = "Wait";
            NxtWrdLetter[i].SetActive (false);
            //NxtSen[i].fontSize = 12 * (1 - 0.5) + 6;
            NxtWrdLetter[i].layer = LayerMask.NameToLayer("UI");
            NxtWrdrWedge[i].layer = LayerMask.NameToLayer("TransparentFX");
            NxtWrdLetter[i].transform.SetParent(NxtWrdrWedge[i].transform, false);
}

        for (int i = 0; i < 3; i++)
        {

            NxtSntWedge[i] = Instantiate(SentPrefab, new Vector3(0, 0, 0), Quaternion.identity) ;
            NxtSntWedge[i].transform.SetParent(transform, false);


            NxtSntWedge[i].name = "bar"+i.ToString();
            NxtSntWedge[i].SetActive ( false);
            NxtSntWedge[i].GetComponent<Image>().alphaHitTestMinimumThreshold = 0.2f;
            NxtSntLetter[i] = Instantiate(NxtSenPrefab);
            NxtSntLetter[i].transform.SetParent(NxtSntWedge[i].transform, false);
            NxtSntLetter[i].GetComponent<TextMeshProUGUI>().text = "wait";
            NxtSntLetter[i].SetActive(false);
            NxtSntWedge[i].layer = LayerMask.NameToLayer("TransparentFX"); // Layer proiarity set up to enable eye gaze selection on the top layer
            NxtSntLetter[i].layer = LayerMask.NameToLayer("UI");

            //NxtSen[i].fontSize = 12 * (1 - 0.5) + 6;
            //activeBars.Add(NxtSntWedge[i].gameObject);
        }

    }
        // i: The ID of half of the keyboard characters
        // invers_i: The ID of another half of the keyboard characters
        // NewWedge: The Object of the  Keyboard Characters
        // Letters: The content Object of the keyboard characters
    void instantiatefunction1(int sliceNum, float centerRaduis, string[] alphabet)
    {

        for (int i = 0; i < sliceNum/2; i++)//sliceNum
        {
            int invers_i = i + sliceNum / 2;
            centerAngle[i] = 180 - (360 / ((float)sliceNum-2f )) * ((float)i );
            x[i] = centerRaduis * Mathf.Cos(Mathf.PI * 2f * centerAngle[i] / 360f);
            y[i] = centerRaduis * Mathf.Sin(Mathf.PI * 2f * centerAngle[i] / 360f) +35f;

            y[invers_i] = -y[i];
            x[invers_i] = -x[i];

            newWedge[i] = Instantiate(wedgePrefab.GetComponent<Image>(), new Vector3(x[i], y[i], 0), Quaternion.identity) as Image;
            newWedge[i].name = i.ToString();
            newWedge[i].transform.SetParent(transform, false);
            newWedge[i].enabled = true;
            //WedgeColors[i] = UnityEngine.Random.ColorHSV(0.5f, 0.5f, 0.1f, 0.1f, 0.5f, 0.5f);
            //WedgeColors[i].a = 0.2f;
            //newWedge[i].color = WedgeColors[i];
            newWedge[i].alphaHitTestMinimumThreshold = 0.2f;
            Letters[i] = Instantiate(LettersPrefab) as TextMeshProUGUI;
            Letters[i].transform.SetParent(newWedge[i].transform, false);
            Letters[i].text = alphabet[i];
            Letters[i].enabled = true;
            activeWedges.Add(newWedge[i].gameObject);


            newWedge[invers_i] = Instantiate(wedgePrefab.GetComponent<Image>(), new Vector3(x[invers_i], y[invers_i], 0), Quaternion.identity) as Image;
            newWedge[invers_i].name = invers_i.ToString();
            newWedge[invers_i].transform.SetParent(transform, false);
            newWedge[invers_i].enabled = true;
            newWedge[invers_i].alphaHitTestMinimumThreshold = 0.2f;
            Letters[invers_i] = Instantiate(LettersPrefab) as TextMeshProUGUI;
            Letters[invers_i].transform.SetParent(newWedge[invers_i].transform, false);
            Letters[invers_i].text = alphabet[invers_i];
            Letters[invers_i].enabled = true;
            activeWedges.Add(newWedge[invers_i].gameObject);
        }
    }

    void instantiatefunction2(int sliceNum, float centerRaduis, string[] alphabet)
    {
        for (int i = 0 ; i < sliceNum / 2 ; i++)
        {
            var j = i + sliceNum1;
            var invers_j = j+ sliceNum / 2;
            centerAngle[j] = 180f - (360f / ((float)sliceNum - 2f)) * ((float)i);
            x[j] = centerRaduis * Mathf.Cos(Mathf.PI * 2f * centerAngle[j] / 360f);
            y[j] = centerRaduis * Mathf.Sin(Mathf.PI * 2f * centerAngle[j] / 360f)+35f; 


            y[invers_j] = -y[j];
            x[invers_j] = -x[j];
            newWedge[j] = Instantiate(wedgePrefab.GetComponent<Image>(), new Vector3(x[j], y[j], 0), Quaternion.identity) as Image;   
            
            newWedge[j].transform.SetParent(transform, false);
            newWedge[j].name = j.ToString();

            newWedge[j].enabled = true;
            //WedgeColors[i] = UnityEngine.Random.ColorHSV(0.5f, 0.5f, 0.1f, 0.1f, 0.5f, 0.5f);
            //WedgeColors[i].a = 0.2f;
            //newWedge[i].color = WedgeColors[i];
            newWedge[j].alphaHitTestMinimumThreshold = 0.2f;
            Letters[j] = Instantiate(LettersPrefab) as TextMeshProUGUI;
            Letters[j].transform.SetParent(newWedge[j].transform, false);
            Letters[j].text = alphabet[j];
            Letters[j].enabled = true;

            newWedge[invers_j] = Instantiate(wedgePrefab.GetComponent<Image>(), new Vector3(x[invers_j], y[invers_j], 0), Quaternion.identity) as Image;
            newWedge[invers_j].name = invers_j.ToString();
            newWedge[invers_j].transform.SetParent(transform, false);
            newWedge[invers_j].enabled = true;
            newWedge[invers_j].alphaHitTestMinimumThreshold = 0.2f;
            Letters[invers_j] = Instantiate(LettersPrefab) as TextMeshProUGUI;
            Letters[invers_j].transform.SetParent(newWedge[invers_j].transform, false);
            Letters[invers_j].text = alphabet[invers_j];
            Letters[invers_j].enabled = true;

            activeWedges.Add(newWedge[invers_j].gameObject);
            activeWedges.Add(newWedge[i].gameObject);
        }
    }
    void instantiatefunction3(int sliceNum, float centerRaduis, string[] alphabet)
    {
        for (int i = 0; i < sliceNum / 2; i++)
        {
            var j = i + sliceNum1+ sliceNum2;
            var invers_j = j + sliceNum / 2;

            centerAngle[j] = 180f - (360f / ((float)sliceNum - 2f)) * ((float)i);
            x[j] = centerRaduis * Mathf.Cos(Mathf.PI * 2f * centerAngle[j] / 360f);
            y[j] = centerRaduis * Mathf.Sin(Mathf.PI * 2f * centerAngle[j] / 360f) + 35f;
            y[invers_j] = -y[j];
            x[invers_j] = -x[j];
            newWedge[j] = Instantiate(wedgePrefab.GetComponent<Image>(), new Vector3(x[j], y[j], 0), Quaternion.identity) as Image;
            newWedge[j].transform.SetParent(transform, false);
            newWedge[j].name = j.ToString();
            newWedge[j].enabled = true;
            //WedgeColors[i] = UnityEngine.Random.ColorHSV(0.5f, 0.5f, 0.1f, 0.1f, 0.5f, 0.5f);
            //WedgeColors[i].a = 0.2f;
            //newWedge[i].color = WedgeColors[i];
            newWedge[j].alphaHitTestMinimumThreshold = 0.2f;
            Letters[j] = Instantiate(LettersPrefab) as TextMeshProUGUI;
            Letters[j].transform.SetParent(newWedge[j].transform, false);
            Letters[j].text = alphabet[j];
            Letters[j].enabled = true;

            // Three keyboard characters act differetly including the delete, word delete and the space functions
            if  (alphabet[invers_j] == "Del" || alphabet[invers_j] == "WDel" || alphabet[invers_j] == "Spc" )
            {
                newWedge[invers_j] = Instantiate(DelPrefab.GetComponent<Image>(), new Vector3(x[invers_j], y[invers_j], 0), Quaternion.identity) as Image;
            }
            else { 
                 newWedge[invers_j] = Instantiate(wedgePrefab.GetComponent<Image>(), new Vector3(x[invers_j], y[invers_j], 0), Quaternion.identity) as Image;
            }
            newWedge[invers_j].name = invers_j.ToString();
            newWedge[invers_j].transform.SetParent(transform, false);
            newWedge[invers_j].enabled = true;
            newWedge[invers_j].alphaHitTestMinimumThreshold = 0.2f;
            Letters[invers_j] = Instantiate(LettersPrefab) as TextMeshProUGUI;
            Letters[invers_j].transform.SetParent(newWedge[invers_j].transform, false);
            Letters[invers_j].text = alphabet[invers_j];
            Letters[invers_j].enabled = true;

            activeWedges.Add(newWedge[invers_j].gameObject);
            activeWedges.Add(newWedge[i].gameObject);
        }
    }



}