using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SensorDataFactory 
{
    public SensorData Data { get; set; }
    public virtual SensorData SensorFactory(string message) {
        try
        {
            var value = JsonUtility.FromJson<SensorData>(message);
            Data = value;
            return value;
        }
        catch (Exception)
        {

            throw;
        }
    }
}

