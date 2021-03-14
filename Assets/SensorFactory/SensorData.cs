using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class SensorData : ISensorData
{
    public string deviceID { get ; set ; }
    public string name { get ; set ; }
    public string roomID { get ; set ; }
    public string type { get ; set ; }
    public string baseType { get ; set ; }
    public bool enabled { get ; set ; }
    public bool visable { get ; set ; }
    public string parentId { get ; set ; }
    public IProperties propertiy { get ; set ; }
    public Time created { get ; set ; }
    public Time modifier { get ; set ; }
    public List<string> interfaces { get ; set ; }


    public string getCurrentData()
    {
        return this.propertiy.value;
    }

    public abstract string getTextOutput();

    public SensorData() {
        this.propertiy = new SensorProperty();
    }
}
[Serializable]
public class SensorProperty : IProperties
{
    public string zwaveCompany { get; set; }
    public string zwaveInfo { get; set; }
    public float zwaveVersion { get; set; }
    public float wakeUpTime { get; set; }
    public float pollingTimeSec { get; set; }
    public float batteryLevel { get; set; }
    public string alarmDelay { get; set; }
    public string alarmExclude { get; set; }
    public string alarmTimeTimestamp { get; set; }
    public string armConditions { get; set; }
    public string armConfig { get; set; }
    public string armDelay { get; set; }
    public string armError { get; set; }
    public string armTimeTimestamp { get; set; }
    public string armed { get; set; }
    public string batteryLowNotification { get; set; }
    public string configured { get; set; }
    public string dead { get; set; }
    public string deviceControlType { get; set; }
    public string deviceIcon { get; set; }
    public string emailNotificationID { get; set; }
    public string emailNotificationType { get; set; }
    public string endPointId { get; set; }
    public string fibaroAlarm { get; set; }
    public string interval { get; set; }
    public string lastBreached { get; set; }
    public string liliOffCommand { get; set; }
    public string liliOnCommand { get; set; }
    public string log { get; set; }
    public string logTemp { get; set; }
    public string manufacturer { get; set; }
    public string markAsDead { get; set; }
    public string model { get; set; }
    public string nodeId { get; set; }
    public string parametersTemplate { get; set; }
    public string productInfo { get; set; }
    public string pushNotificationID { get; set; }
    public string pushNotificationType { get; set; }
    public string remoteGatewayId { get; set; }
    public string saveLogs { get; set; }
    public string smsNotificationID { get; set; }
    public string smsNotificationType { get; set; }
    public string useTemplate { get; set; }


    public string value { get { return data; } set { 
            history.Add(value);
            data = value;
        } }

    protected List<string> history;
    protected string data;

    public List<string> DataDeluge()
    {
        return history;
    }



    public SensorProperty(){
        this.history = new List<string>();
        data = "";
    }
}