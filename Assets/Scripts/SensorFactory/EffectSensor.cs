using System;

namespace Assets.SensorFactory
{
    public class EffectSensor : SensorHandler
    {
        public EffectSensor(SensorData data) : base(data)
        {
        }

        public override string getTextOutput()
        {
            string body = ToString();
            return body.Remove(body.Length - data.value.Length) + data.value + "W";
        }
    }
    public class EffectSensorFactory : SensorDataFactory
    {

        public override SensorHandler SensorFactory()
        {
            return new DefaultSensor(data);
        }
    }



}
