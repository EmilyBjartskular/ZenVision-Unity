using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.SensorFactory
{
    public class DoorSensorFactory : SensorDataFactory
    {

        public override ISensorData Factory()
        {
            return new DoorSensor();
        }
    }

    public class DoorSensor : SensorData
    {
        private string value;
        public DoorSensor() : base()
        {
            this.value = propertiy.value;
        }

        public override string getTextOutput() //todo make this so that we can generate a icon instead that is highligted vs not heighlited.
        {
            return value;
        }

    }
}

