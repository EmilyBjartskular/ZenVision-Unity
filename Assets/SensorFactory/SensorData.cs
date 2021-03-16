using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SensorHandler
{
    public SensorData data { get; set; }
    public string getCurrentData()
    {
        return this.data.value;
    }

  
    public SensorHandler(SensorData data) {
        this.data = data;
    }

    public abstract string getTextOutput();

  

}

[Serializable]
public class SensorData {
    public int id;
    public string name;
    public string type;
    public string baseType;
    public long created;
    public long modifier;
    public int batteryLevel;
    public string value;
}

