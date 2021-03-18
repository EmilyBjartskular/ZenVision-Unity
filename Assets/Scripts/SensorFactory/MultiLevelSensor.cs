﻿using System;

namespace Assets.SensorFactory
{
    public class MultiLevelSensor : SensorHandler
    {
        public MultiLevelSensor(SensorData data) : base(data)
        {
        }

        public override string getTextOutput()
        {
            string body = ToString();
            return body.Remove(body.Length - data.value.Length - 1, body.Length - 1) + data.value + "UV";
        }
    }
    public class MultiLevelSensorFactory : SensorDataFactory
    {

        public override SensorHandler SensorFactory()
        {
            return new DefaultSensor(data);
        }
    }



}
