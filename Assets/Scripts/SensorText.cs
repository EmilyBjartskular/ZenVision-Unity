using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SensorText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI target;

    [SerializeField]
    private NetworkGadget network;
    void Start()
    {
        network.DataAvailable += setText;
        target.text = "No Input Yet";
    }
    public void setText(SensorHandler handler) {
#if UNITY_EDITOR
        Debug.Log("Setting Text : "+ handler.getTextOutput());
#endif
        target.text = handler.getTextOutput();
        Canvas.ForceUpdateCanvases();
        //target.ForceMeshUpdate(true, true);
    }
}
