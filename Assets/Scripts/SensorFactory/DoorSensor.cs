using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.SensorFactory
{
    public class DoorSensorFactory : SensorDataFactory
    {
        public override SensorHandler SensorFactory()
        {
            return new DoorSensor(data);
        }
    }

    public class DoorSensor : SensorHandler
    {
        public DoorSensor(SensorData data) : base(data)
        {
            this.data = data;
        }

        public override string getTextOutput()
        {
            string body = ToString();
            string value = data.value.Equals("true") ? "Open" : "Closed";
            return body.Remove(body.Length - data.value.Length - 7) + "Status: " + value;
        }

    }
}

