﻿using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    private string sensorID;
    public string SensorID { get { return sensorID; } set { sensorID = value; toolTip.ToolTipText = value; } }
    public SensorType SensorType { get; set; }

    [SerializeField]
    private ToolTip toolTip;

    public Interactable button;

    public GameObject DataWindow { get; set; }

    public NetworkGadget Network { get; set; }

    public AnchorManager Manager { get; set; }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        sensorID = "123";
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Action()
    {
        if (Manager.DeleteMode)
        {
            Manager.DeleteSensorAsync(gameObject, sensorID);
        }
        else
        {
            DataWindow.SetActive(true);
            Network.ID = sensorID;
            Network.Type = SensorType;
            Network.StartNetwork();
        }
    }
}
