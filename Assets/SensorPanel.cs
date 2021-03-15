using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorPanel : MonoBehaviour
{

    [SerializeField]
    private NetworkGadget networkGadget;
    private StateManager stateManager;
    void Start()
    {

        stateManager = new IdleState();
    }

    void Update()
    {
        stateManager = stateManager.Process();
    }
}
