using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using UnityEngine;

public class NetworkGadget
{
    public static readonly string ENDPOINT = "ws://localhost:2555";
    private WebsocketClient client { get; set; }
    private static NetworkGadget instance;

    public NetworkGadget() {
        client = new WebsocketClient(new Uri(ENDPOINT));
    }

    public static NetworkGadget getInstance() {
        if (instance == null) {
            instance = new NetworkGadget();
        }
        return instance;
    }

}
public class WebsocketClient
{

    public Action<string> messageReceived { get; }
    private ClientWebSocket client;

    private UTF8Encoding encoder;
    private const UInt64 MAXREADSIZE = 1 * 1024 * 1024;
    public BlockingCollection<string> sendQueue { get; }

    public Uri URI { get; }

    public WebsocketClient(Uri uri)
    {
        URI = uri;
        encoder = new UTF8Encoding();
        client = new ClientWebSocket();

        sendQueue = new BlockingCollection<string>();


    }
    public async Task Connect()
    {

        await client.ConnectAsync(URI, CancellationToken.None);
        while (client.State == WebSocketState.Connecting)
        {
            await Task.Yield();
        }
        Debug.Log("Connect status: " + client.State);


    }

    public async Task Send(string message)
    {
        var encoded = encoder.GetBytes(message);
        var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);

        await client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

    }
    public async Task Receive() 
    {
        byte[] buf = new byte[8 * 1024];
        var stream = new MemoryStream();
        ArraySegment<byte> arrayBuf = new ArraySegment<byte>(buf);
        WebSocketReceiveResult chunkResult = null;
        if (client.State == WebSocketState.Open)
        {
            do
            {
                chunkResult = await client.ReceiveAsync(arrayBuf, CancellationToken.None);
                stream.Write(arrayBuf.Array, arrayBuf.Offset, chunkResult.Count);
                if ((UInt64)(chunkResult.Count) > MAXREADSIZE)
                {
                    Console.Error.WriteLine("Warning: Message is bigger than expected!");//todo fix open stack issue
                }
            } while (!chunkResult.EndOfMessage);
            stream.Seek(0, SeekOrigin.Begin);

            if (chunkResult.MessageType == WebSocketMessageType.Text)
            {
                string message = StreamToString(stream);
                messageReceived(message);

            }

        }


    }

    private string StreamToString(MemoryStream ms)
    {
        string readString = "";

        using (var reader = new StreamReader(ms, encoder))
        {
            readString = reader.ReadToEnd();
        }

        return readString;
    }

    private async Task Process()
    {
        Task dequeSendMessages = Task.Factory.StartNew(async () => {
            while (!sendQueue.IsCompleted)
                await Send(sendQueue.Take());
        });
        while (client.State == WebSocketState.Open)
        {
            if (sendQueue.Count > 1)
            {
                dequeSendMessages.Wait();
            }
            else
            {
                await Receive(); //will this stop sending from happening? mostly server will send responses to the client and mostly should not be reciving data. but sending also intails queries that server responds too
                await Task.Yield();
            }
        }
    }


    private async void CloseConnection()
    {
        await client.CloseAsync(WebSocketCloseStatus);
        DisposeConnection();
    }


    private void DisposeConnection()
    {
        sendQueue.Dispose();
        client.Dispose();
    }

}



