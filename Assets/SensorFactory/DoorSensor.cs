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
            return data.value.Equals("true") ? "Open" : "Closed";
        }

    }
}

