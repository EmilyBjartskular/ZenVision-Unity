﻿using System.Collections;
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
    Time created { get; set; }
    Time modifier { get; set; }
    List<string> interfaces { get; set; }

    string getCurrentData();
    string filter(string filter);
    string getTextOutput(); //todo make format for canvas item.
}