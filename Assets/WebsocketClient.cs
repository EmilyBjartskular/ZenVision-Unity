
using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;
#if UNITY_EDITOR
using System.Net.WebSockets;
#elif WINDOWS_UWP || UNITY_WSA_10_0 ||UNITY_WSA
using Windows.Networking.Sockets
using Windows.Storage.Streams 
#endif
using System.Threading;
using System.Text;
using System.Collections.Concurrent;

public class WebsocketClient
{

    public Action<string> messageReceived { get; set; }

#if UNITY_EDITOR
    private ClientWebSocket client;
    private UTF8Encoding encoder;
    private Thread processThread;
    private const UInt64 MAXREADSIZE = 10 * 1024 * 1024;
    public BlockingCollection<string> sendQueue { get; }
    

#elif WINDOWS_UWP || UNITY_WSA_10_0 || UNITY_WSA
    private MessageWebSocket client;
    private DataWriter writer;
#endif
    public Uri URI { get; }

    public WebsocketClient(Uri uri)
    {
        URI = uri;
#if UNITY_EDITOR
        encoder = new UTF8Encoding();
        client = new ClientWebSocket();
        sendQueue = new BlockingCollection<string>();
        processThread = new Thread(Process);
#elif WINDOWS_UWP || UNITY_WSA_10_0 || UNITY_WSA
        client = new MessageWebSocket();
        client.MessageReceived += Receive;
        client.Closed += CloseConnection;
#endif

    }
    public async Task Connect()
    {
#if UNITY_EDITOR
        await client.ConnectAsync(URI, CancellationToken.None);
        while (client.State == WebSocketState.Connecting)
        {
            await Task.Yield();
        }
        Debug.Log("Connect status: " + client.State);
        processThread.Start();

#elif WINDOWS_UWP || UNITY_WSA_10_0 || UNITY_WSA
        try
        {
         await client.ConnectAsync(URI);

        }
        catch (Exception e)
        {
            client.Dispose();
            client = null;
            Debug.log("Websocket failed to connect to server");
            Debug.log(e.message);
            return;
        }
    
        writer = new DataWriter(client.OutputStream);
#endif

    }

    public async Task Send(string message)
    {
#if UNITY_EDITOR

        var encoded = encoder.GetBytes(message);
        var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);

        await client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
#elif WINDOWS_UWP || UNITY_WSA_10_0 || UNITY_WSA
        writer.WriteString(message);
        try 
	    {	       
		    await writer.StoreAsync();
	    }
	    catch (Exception e)
	    {
            Debug.log(e.message)
		    throw;
	    }
#endif

    }

#if UNITY_EDITOR

    private async Task Receive()
    {
        try
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
                    //messageReceived(message);
                    Debug.Log(message);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
        


    }
#elif WINDOWS_UWP || UNITY_WSA_10_0 || UNITY_WSA
    private void Receive(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
    {
        using (DataReader reader = args.GetDataReader())
        {
            reader.UnicodeEncoding = UnicodeEncoding.Utf8;

            try
            {
                string read = reader.ReadString(reader.UnconsumedBufferLength);
                messageReceived(read);
            }
            catch (Exception ex)
            {
                Debug.log(ex.Message);
            }
        }
    }
#endif

#if UNITY_EDITOR
    private string StreamToString(MemoryStream ms)
    {
        string readString = "";

        using (var reader = new StreamReader(ms, encoder))
        {
            readString = reader.ReadToEnd();
        }

        return readString;
    }

    public async void Process()
    {
        while (true)
        {

            if (sendQueue.Count > 1)
            {
                while (!sendQueue.IsCompleted)
                    await Send(sendQueue.Take());
            }
            else
            {
                await Receive(); //will this stop sending from happening? mostly server will send responses to the client and mostly should not be reciving data. but sending also intails queries that server responds too
                await Task.Yield();
            }
        }
    }
#endif

#if UNITY_EDITOR

    public async void CloseConnection()
    {
        processThread.Abort();
        Debug.Log("Closing Connection");
        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "disconnect", CancellationToken.None);
        DisposeConnection();
    }
    private void DisposeConnection()
    {
        sendQueue.Dispose();
        client.Dispose();
        client = null;
    }

    public bool isConnected()
    {
        return client.State == WebSocketState.Open;
    }
#elif WINDOWS_UWP || UNITY_WSA_10_0 || UNITY_WSA

    private async void CloseConnection(IWebSocket sender, WebSocketClosedEventArgs args)
    {
        if (messageWebSocket == sender)
        {
            DisposeConnection();
        }
    }
    private void DisposeConnection()
    {
        if (writer != null)
        {
            // In order to reuse the socket with another DataWriter, the socket's output stream needs to be detached.
            // Otherwise, the DataWriter's destructor will automatically close the stream and all subsequent I/O operations
            // invoked on the socket's output stream will fail with ObjectDisposedException.
            //
            // This is only added for completeness, as this sample closes the socket in the very next code block.
            writer.DetachStream();
            writer.Dispose();
            writer = null;
        }

        if (client != null)
        {
            try
            {
                client.Close(1000, "Closed due to user request.");
            }
            catch (Exception ex)
            {
                Debug.log(ex.Message);
            }
            client = null;
        }
    }
#endif

}


