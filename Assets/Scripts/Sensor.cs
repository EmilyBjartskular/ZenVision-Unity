using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public string SensorID { get { return SensorID; } set { SensorID = value; OnSensorIDUpdate(); } }

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

    public void OnSensorIDUpdate()
    {
        toolTip.ToolTipText = SensorID;
    }
}
