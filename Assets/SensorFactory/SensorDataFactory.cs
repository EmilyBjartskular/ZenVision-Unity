using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SensorDataFactory 
{
    SensorDataFactory() {
    }

    public virtual ISensorData Factory(SensorType type,string querry="*") {
        
    }
}

public enum SensorType {
    MotionSensor, DoorSensor, ZWaveSensor1 //this is super temporary
}