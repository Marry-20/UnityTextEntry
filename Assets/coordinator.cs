using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coordinator : MonoBehaviour
{
    public static coordinator instance = null;

    public GameObject StartPause;
    public GameObject Stop;
    public GameObject Keyboard;

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

    }

    // Start is called before the first frame update
    void Start()
    {
        // Get the first Mesh Observer available, generally we have only one registered
        var observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
        // Set to not visible
        observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;
        // To disable visual profiler
        CoreServices.DiagnosticsSystem.ShowDiagnostics = false;
        CoreServices.DiagnosticsSystem.ShowProfiler = false;

        StartPause = Instantiate(StartPause, new Vector3(-250,250,0), Quaternion.identity);
        StartPause.SetActive(true);
        StartPause.transform.SetParent(transform, false);

        Stop = Instantiate(Stop, new Vector3(-300, 250, 0), Quaternion.identity);
        Stop.SetActive(true);
        Stop.transform.SetParent(transform, false);

        Keyboard = Instantiate(Keyboard);
        Keyboard.SetActive(false);
        Keyboard.transform.SetParent(transform, false);
    }


}
