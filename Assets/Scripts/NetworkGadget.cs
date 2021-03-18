
using UnityEngine;
using Assets.SensorFactory;
using UnityEngine.Events;
using System;

public enum SensorType
{
    DoorSensor, LightSensor, DefaultSensor, EffectSensor, TemperatureSensor, HumiditySensor, MultiLevelSensor, ZWaveSensor, MotionSensor
}

public class NetworkGadget : MonoBehaviour
{

    public static readonly string ENDPOINT = "ws://130.240.114.14:8010/";
    private WebsocketClient client;

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
        }
        await client.Connect();
        await client.Send(id);
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
        switch (type)
        {
            case SensorType.DoorSensor:
                return new DoorSensorFactory();
            case SensorType.LightSensor:
                return new LightSensorFactory();
            case SensorType.DefaultSensor:
                return new DefaultSensorFactory();
            case SensorType.EffectSensor:
                return new EffectSensorFactory();
            case SensorType.TemperatureSensor:
                return new TempSensorFactory();
            case SensorType.HumiditySensor:
                return new HumiditySensorFactory();
            case SensorType.MultiLevelSensor:
                return new MultiLevelSensorFactory();
            case SensorType.ZWaveSensor:
                return new ZWaveSensorFactory();
            case SensorType.MotionSensor:
                return new MotionSensorFactory();
        }

        return null;

    }

    private void FactoryNewData(string message)
    {
        var factory = GetSensorFactory();
        factory.FactorySensorData(message);
        DataAvailable?.Invoke(factory.SensorFactory());

    }

    public void Update()
    {
        if (client != null) //there is an connection
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


