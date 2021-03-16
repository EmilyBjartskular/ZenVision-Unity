
using UnityEngine;
using Assets.SensorFactory;
using UnityEngine.Events;
using System;
#if UNITY_EDITOR
#elif WINDOWS_UWP || UNITY_WSA_10_0 ||UNITY_WSA
using Windows.networking.Sockets
#endif

public enum SensorType
{
    DoorSensor, LightSensor, DefaultSensor
}

public class NetworkGadget : MonoBehaviour
{

    public static readonly string ENDPOINT = "ws://localhost:8080/";
    private WebsocketClient client { get; set; }

    [SerializeField]
    private string id;

    [SerializeField]
    private SensorType type = SensorType.DefaultSensor;

    public Action<SensorHandler> DataAvailable { get; set; }

    [ContextMenu("Start Network")]
    public async void StartNetwork()
    {
        if (client == null) 
        {
            client = new WebsocketClient(new Uri(ENDPOINT));
            client.messageReceived += FactoryNewData;
        }
        await client.Connect();
        await client.Send(id);
    }
    [ContextMenu("Stop Network")]
    public void StopNetwork()
    {
        client.messageReceived -= FactoryNewData;
        client.CloseConnection();
        client = null;
    }

    public SensorDataFactory GetSensorFactory()
    {
        switch (type)
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
    public void Start()
    {
    }



}


