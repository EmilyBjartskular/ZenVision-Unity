﻿using System;

namespace Assets.SensorFactory
{
    public class DefaultSensor : SensorHandler
    {
        public DefaultSensor(SensorData data) : base(data)
        {
        }

        public override string getTextOutput()
        {
            return ToString();
        }
    }
    public class DefaultSensorFactory : SensorDataFactory
    {

        public override SensorHandler SensorFactory()
        {
            return new DefaultSensor(data);
        }
    }



}
