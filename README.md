# ZenVision
The purpose of this project was an assignment for the course M7012E at LTU (Luleå Tekniska Universitet) to design a pervasive system. The decision was to create an app that Visualizes sensors in a smart home environment. Using the Hololens 2, user is able to interact and display sensors in the environment in real time. Three services was designed to facilitate the app. An [API](https://github.com/EmilyBjartskular/ZenVision-API) that keeps takes the data from a Fibaro home center 3 System. A [store app](https://github.com/EmilyBjartskular/ZenVision-Store) that keeps context of spacial anchor and the main app in unity. Services was facilitated to add and remove sensor in the environment and ability (if user input was correct) get data in real time.
## Project Ideas

1. Precense detection for sensor activation along with sensor visualization with a Microsoft Hololens


### Installation & Setup of project



### Documentation
___
#### Networking
Websocket implementation classes;
##### [NetworkGadget](https://github.com/EmilyBjartskular/ZenVision-Unity/blob/main/Assets/Scripts/NetworkGadget.cs)
This object attaches to the display, it handles networking from unity intreface.
When adding a new SensorType it is require that it is define a new factory supporting this type within GetSensorFactory method.
Recommend that you find more scalable solution to this as there is quite a few sensor types, and they do not follow the same format.

##### [Websocket](https://github.com/EmilyBjartskular/ZenVision-Unity/blob/main/Assets/Scripts/WebsocketClient.cs)
This is a support class for dealing with websocket connections in accordance to RFC6455. Observe, that if deployed on the Hololense (WINDOWS_UWP and the like) it does not support
video streaming or any larger format, this is because it is using [messageWebsocket](https://docs.microsoft.com/en-us/uwp/api/windows.networking.sockets.messagewebsocket?view=winrt-19041). Reimplement with [StreamableWebsocket](https://docs.microsoft.com/en-us/uwp/api/windows.networking.sockets.streamwebsocket?view=winrt-19041) instead.


#### [SensorFactory](https://github.com/EmilyBjartskular/ZenVision-Unity/tree/main/Assets/Scripts/SensorFactory)
A simple factory pattern.
Inherit from the SensorDataFactory and manipulate an SensorHandler Object.
Exampel:

```cs
    public class DefaultSensorFactory : SensorDataFactory
    {

        public override SensorHandler SensorFactory()
        {
            return new DefaultSensor(data);
        }
    }
```

The sensorHandler is in charge what the final output will be on for the end user, thus it deals with the data. currently there is only one supported data format between the api and
unity. This is unfortunate and needs to be fixed in the future. The sensor data class is where the format need to be decided.
Exampel Sensor handler:
```cs
public class DefaultSensor : SensorHandler
    {
        public DefaultSensor(SensorData data) : base(data)
        {
        }

        public override string getTextOutput()
        {
            return ToString();
        }
    }
```

#### Azure Spacial ancor

#### 
