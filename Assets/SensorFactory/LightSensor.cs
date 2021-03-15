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
        public override SensorData SensorFactory(string message)
        {
            try
            {
                var value = JsonUtility.FromJson<SensorData>(message);
                var res = new LightSensor();
                res.SetBaseProperties(value);
                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
    public class LightSensor : SensorData 
    {
       
        public override string getTextOutput()
        {
            return "Light level: " + propertiy.value;
        }
    }
}
