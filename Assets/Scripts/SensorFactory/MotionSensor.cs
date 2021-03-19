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
            return body.Remove(body.Length - data.value.Length - 7) + "Status: " + value;
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
