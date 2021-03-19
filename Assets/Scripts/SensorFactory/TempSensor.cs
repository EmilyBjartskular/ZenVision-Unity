using System;

namespace Assets.SensorFactory
{
    public class TempSensor : SensorHandler
    {
        public TempSensor(SensorData data) : base(data)
        {
        }

        public override string getTextOutput()
        {
            string body = ToString();
            return body.Remove(body.Length - data.value.Length) + data.value + "\u00b0";
        }
    }
    public class TempSensorFactory : SensorDataFactory
    {

        public override SensorHandler SensorFactory()
        {
            return new DefaultSensor(data);
        }
    }



}
