using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SensorText : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro target;
    private SensorDataFactory factory;

    void Start()
    {
        target.text = "No Input Yet";
    }

    void Update()
    {
    }
}
