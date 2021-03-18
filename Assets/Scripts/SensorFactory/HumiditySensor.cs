using System;

namespace Assets.SensorFactory
{
    public class HumiditySensor : SensorHandler
    {
        public HumiditySensor(SensorData data) : base(data)
        {
        }

        public override string getTextOutput()
        {
            string body = ToString();
            return body.Remove(body.Length - data.value.Length - 1, body.Length - 1) + data.value + "\u0025";
        }
    }
    public class HumiditySensorFactory : SensorDataFactory
    {

        public override SensorHandler SensorFactory()
        {
            return new DefaultSensor(data);
        }
    }



}
