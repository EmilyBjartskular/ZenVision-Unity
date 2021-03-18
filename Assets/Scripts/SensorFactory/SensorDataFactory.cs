using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SensorDataFactory 
{
    public SensorHandler handler { get; set; }
    public SensorData data { get; set; }
    public abstract SensorHandler SensorFactory();

    public virtual SensorData FactorySensorData(string message)
    {
        try
        {
            var value = JsonUtility.FromJson<SensorData>(message);
            data = value;
            return value;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
    }


}

