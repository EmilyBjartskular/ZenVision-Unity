using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    private string SensorID;

    [SerializeField]
    private ToolTip toolTip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSensorID(string id)
    {
        SensorID = id;
        toolTip.ToolTipText = SensorID;
    }
}
