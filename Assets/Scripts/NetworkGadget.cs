﻿
using UnityEngine;
using Assets.SensorFactory;
using UnityEngine.Events;
using System;

public enum SensorType
{
    DoorSensor, LightSensor, DefaultSensor
}

public class NetworkGadget : MonoBehaviour
{

    public string Endpoint = "ws://130.240.114.14:8010/";

    private WebsocketClient client;

    public string ID { get; set; }

    public SensorType Type { get; set; } = SensorType.DefaultSensor;

    public Action<SensorHandler> DataAvailable { get; set; }

    [ContextMenu("Start Network")]
    public async void StartNetwork()
    {
        if (client == null)
        {
            client = new WebsocketClient(new Uri(Endpoint));
            await client.Connect();
        }
        await client.Send(ID);
    }
    [ContextMenu("Stop Network")]
    public void StopNetwork()
    {
#if UNITY_EDITOR
        client.CloseConnection();
#elif WINDOWS_UWP || UNITY_WSA_10_0 || UNITY_WSA
        client.client.Close(0, "");
#endif
        client = null;
    }

    public SensorDataFactory GetSensorFactory()
    {
        switch (Type)
        {
            case SensorType.DoorSensor:
                return new DoorSensorFactory();
            case SensorType.LightSensor:
                return new LightSensorFactory();
            case SensorType.DefaultSensor:
                return new DefaultSensorFactory();
        }

        return null;

    }

    private void FactoryNewData(string message)
    {
        var factory = GetSensorFactory();
        factory.FactorySensorData(message);
        DataAvailable.Invoke(factory.SensorFactory());

    }

    public void Update()
    {
        if(client != null) //there is an connection
        {
            while (!client.receiveQueue.IsEmpty)
            {
                string msg;
                client.receiveQueue.TryDequeue(out msg);
                FactoryNewData(msg);
            }

        }
       
    }



}


