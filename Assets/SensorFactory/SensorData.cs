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
    public SensorProperty propertiy { get ; set ; }
    public DateTime created { get ; set ; }
    public DateTime modifier { get ; set ; }
    public List<string> interfaces { get ; set ; }


    public string getCurrentData()
    {
        return this.propertiy.value;
    }

    public abstract string getTextOutput();

    public virtual void SetBaseProperties(SensorData value) {
        this.deviceID = value.deviceID;
        this.name = value.name;
        this.roomID = value.roomID;
        this.type = value.type;
        this.baseType = value.baseType;
        this.enabled = value.enabled;
        this.visable = value.visable;
        this.parentId = value.parentId;
        this.propertiy = value.propertiy;
        this.created = value.created;
        this.modifier = value.modifier;
        this.interfaces = value.interfaces;

    }
    protected SensorData(string deviceID, string name, string roomID, string type, string baseType, bool enabled, bool visable, string parentId, SensorProperty propertiy, DateTime created, DateTime modifier, List<string> interfaces)
    {
        this.deviceID = deviceID;
        this.name = name;
        this.roomID = roomID;
        this.type = type;
        this.baseType = baseType;
        this.enabled = enabled;
        this.visable = visable;
        this.parentId = parentId;
        this.propertiy = propertiy;
        this.created = created;
        this.modifier = modifier;
        this.interfaces = interfaces;
    }
    protected SensorData() { }

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
    public string value { get; set; }

    public SensorProperty(string zwaveCompany, string zwaveInfo, float zwaveVersion, float wakeUpTime, float pollingTimeSec, float batteryLevel, string alarmDelay, string alarmExclude, string alarmTimeTimestamp, string armConditions, string armConfig, string armDelay, string armError, string armTimeTimestamp, string armed, string batteryLowNotification, string configured, string dead, string deviceControlType, string deviceIcon, string emailNotificationID, string emailNotificationType, string endPointId, string fibaroAlarm, string interval, string lastBreached, string liliOffCommand, string liliOnCommand, string log, string logTemp, string manufacturer, string markAsDead, string model, string nodeId, string parametersTemplate, string productInfo, string pushNotificationID, string pushNotificationType, string remoteGatewayId, string saveLogs, string smsNotificationID, string smsNotificationType, string useTemplate, string value)
    {
        this.zwaveCompany = zwaveCompany;
        this.zwaveInfo = zwaveInfo;
        this.zwaveVersion = zwaveVersion;
        this.wakeUpTime = wakeUpTime;
        this.pollingTimeSec = pollingTimeSec;
        this.batteryLevel = batteryLevel;
        this.alarmDelay = alarmDelay;
        this.alarmExclude = alarmExclude;
        this.alarmTimeTimestamp = alarmTimeTimestamp;
        this.armConditions = armConditions;
        this.armConfig = armConfig;
        this.armDelay = armDelay;
        this.armError = armError;
        this.armTimeTimestamp = armTimeTimestamp;
        this.armed = armed;
        this.batteryLowNotification = batteryLowNotification;
        this.configured = configured;
        this.dead = dead;
        this.deviceControlType = deviceControlType;
        this.deviceIcon = deviceIcon;
        this.emailNotificationID = emailNotificationID;
        this.emailNotificationType = emailNotificationType;
        this.endPointId = endPointId;
        this.fibaroAlarm = fibaroAlarm;
        this.interval = interval;
        this.lastBreached = lastBreached;
        this.liliOffCommand = liliOffCommand;
        this.liliOnCommand = liliOnCommand;
        this.log = log;
        this.logTemp = logTemp;
        this.manufacturer = manufacturer;
        this.markAsDead = markAsDead;
        this.model = model;
        this.nodeId = nodeId;
        this.parametersTemplate = parametersTemplate;
        this.productInfo = productInfo;
        this.pushNotificationID = pushNotificationID;
        this.pushNotificationType = pushNotificationType;
        this.remoteGatewayId = remoteGatewayId;
        this.saveLogs = saveLogs;
        this.smsNotificationID = smsNotificationID;
        this.smsNotificationType = smsNotificationType;
        this.useTemplate = useTemplate;
        this.value = value;
    }
    public void SetSensorProperty(SensorProperty value) {
        this.zwaveCompany = value.zwaveCompany;
        this.zwaveInfo = value.zwaveInfo;
        this.zwaveVersion = value.zwaveVersion;
        this.wakeUpTime = value.wakeUpTime;
        this.pollingTimeSec = value.pollingTimeSec;
        this.batteryLevel = value.batteryLevel;
        this.alarmDelay = value.alarmDelay;
        this.alarmExclude = value.alarmExclude;
        this.alarmTimeTimestamp = value.alarmTimeTimestamp;
        this.armConditions = value.armConditions;
        this.armConfig = value.armConfig;
        this.armDelay = value.armDelay;
        this.armError = value.armError;
        this.armTimeTimestamp = value.armTimeTimestamp;
        this.armed = value.armed;
        this.batteryLowNotification = value.batteryLowNotification;
        this.configured = value.configured;
        this.dead = value.dead;
        this.deviceControlType = value.deviceControlType;
        this.deviceIcon = value.deviceIcon;
        this.emailNotificationID = value.emailNotificationID;
        this.emailNotificationType = value.emailNotificationType;
        this.endPointId = value.endPointId;
        this.fibaroAlarm = value.fibaroAlarm;
        this.interval = value.interval;
        this.lastBreached = value.lastBreached;
        this.liliOffCommand = value.liliOffCommand;
        this.liliOnCommand = value.liliOnCommand;
        this.log = value.log;
        this.logTemp = value.logTemp;
        this.manufacturer = value.manufacturer;
        this.markAsDead = value.markAsDead;
        this.model = value.model;
        this.nodeId = value.nodeId;
        this.parametersTemplate = value.parametersTemplate;
        this.productInfo = value.productInfo;
        this.pushNotificationID = value.pushNotificationID;
        this.pushNotificationType = value.pushNotificationType;
        this.remoteGatewayId = value.remoteGatewayId;
        this.saveLogs = value.saveLogs;
        this.smsNotificationID = value.smsNotificationID;
        this.smsNotificationType = value.smsNotificationType;
        this.useTemplate = value.useTemplate;
        this.value = value.value;
    }

    public SensorProperty() { }

}