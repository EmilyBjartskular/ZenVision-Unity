using System.Collections.Generic;
public interface IProperties
{
    string zwaveCompany { get; set; }
    string zwaveInfo { get; set; }
    float zwaveVersion { get; set; }
    float wakeUpTime { get; set; }
    float pollingTimeSec { get; set; }
    float batteryLevel { get; set; }
    string alarmDelay { get; set; }
    string alarmExclude { get; set; }
    string alarmTimeTimestamp { get; set; }
    string armConditions { get; set; }
    string armConfig { get; set; }
    string armDelay { get; set; }
    string armError { get; set; }
    string armTimeTimestamp { get; set; }
    string armed { get; set; }
    string batteryLowNotification { get; set; }
    string configured { get; set; }
    string dead { get; set; }
    string deviceControlType { get; set; }
    string deviceIcon { get; set; }
    string emailNotificationID { get; set; }
    string emailNotificationType { get; set; }
    string endPointId { get; set; }
    string fibaroAlarm { get; set; }
    string interval { get; set; }
    string lastBreached { get; set; }
    string liliOffCommand { get; set; }
    string liliOnCommand { get; set; }
    string log { get; set; }
    string logTemp { get; set; }
    string manufacturer { get; set; }
    string markAsDead { get; set; }
    string model { get; set; }
    string nodeId { get; set; }
    string parametersTemplate { get; set; }
    string productInfo { get; set; }
    string pushNotificationID { get; set; }
    string pushNotificationType { get; set; }
    string remoteGatewayId { get; set; }
    string saveLogs { get; set; }
    string smsNotificationID { get; set; }
    string smsNotificationType { get; set; }
    string useTemplate { get; set; }
    string value { get; set; }

    List<string> DataDeluge();

}