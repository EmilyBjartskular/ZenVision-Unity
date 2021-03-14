using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.SensorFactory
{
    public class LightSensorFactory : SensorDataFactory
    {
        public override ISensorData Factory()
        {
            return new LightSensor();
        }
    }
    public class LightSensor : SensorData 
    {
        public override string getTextOutput()
        {
            return propertiy.value;
        }
    }
}
