using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SensorText : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro target;
    [SerializeField]
    private NetworkGadget network;

    void Start()
    {
        target.text = "No Input Yet";
    }

    public void Select() {

        network.DataUpdate += setText;
    }
    public void UnSelect() {
        network.DataUpdate -= setText;
    }

    void setText(SensorData data) {
        target.text = data.getTextOutput();
    }
}
