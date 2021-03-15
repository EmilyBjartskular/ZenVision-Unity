using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISensorData
{
    string deviceID { get; set; }
    string name { get; set; }
    string roomID { get; set; }
    string type { get; set; }
    string baseType { get; set; }
    bool enabled { get; set; }
    bool visable { get; set; }
    string parentId { get; set; }    
    SensorProperty propertiy { get; set; }
    DateTime created { get; set; }
    DateTime modifier { get; set; }
    List<string> interfaces { get; set; }

    string getCurrentData();
    string getTextOutput(); 
}