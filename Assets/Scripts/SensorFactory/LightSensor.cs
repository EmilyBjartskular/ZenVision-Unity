using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.SensorFactory
{
    public class LightSensorFactory : SensorDataFactory
    {
        public override SensorHandler SensorFactory()
        {
            return new LightSensor(data);
        }
    }
    public class LightSensor : SensorHandler
    {
        public LightSensor(SensorData data) : base(data)
        {
        }

        public override string getTextOutput()
        {
            string body = ToString();
            return body.Remove(body.Length - 1 - data.value.Length - 7, body.Length-1) + "Light level: " + data.value + "lx";
        }
    }
}
