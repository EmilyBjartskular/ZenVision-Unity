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
            return "Light level: " + data.value;
        }
    }
}
