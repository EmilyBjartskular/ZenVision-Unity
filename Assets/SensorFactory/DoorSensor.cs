using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.SensorFactory
{
    public class DoorSensorFactory : SensorDataFactory
    {
        public override SensorData SensorFactory(string message)
        {
            try
            {
                var value = JsonUtility.FromJson<SensorData>(message);
                var res = new DoorSensor();
                res.SetBaseProperties(value);
                Data = value;
                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    public class DoorSensor : SensorData
    {

        public override string getTextOutput() 
        {
            return propertiy.value.Equals("true") ? "Open" : "Closed";
        }

    }
}

