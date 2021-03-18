using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SensorHandler
{
    public SensorData data { get; set; }

    public override string ToString()
    {
        DateTime created = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime modified = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return $"ID: {data.id}\n" +
            $"Name: {data.name}\n" +
            $"Type: {data.type}\n" +
            $"Created: {created.AddMilliseconds(data.created)}\n" +
            $"Modified: {modified.AddMilliseconds(data.created)}\n" +
            $"Battery: {data.batteryLevel}\u0025\n" +
            $"value: {data.value}";
    }

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

