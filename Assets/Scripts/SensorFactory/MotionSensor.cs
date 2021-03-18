using System;

namespace Assets.SensorFactory
{
    public class MotionSensor : SensorHandler
    {
        public MotionSensor(SensorData data) : base(data)
        {
        }

        public override string getTextOutput()
        {
            string body = ToString();
            string value = data.value.Equals("true") ? "No detected motion" : "Motion!";
            return body.Remove(body.Length - 1 - data.value.Length - 7, body.Length - 1) + "Status: " + value;
        }
    }
    public class MotionSensorFactory : SensorDataFactory
    {

        public override SensorHandler SensorFactory()
        {
            return new DefaultSensor(data);
        }
    }



}
