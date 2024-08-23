using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BraketsEngine;

public class BridgeServer
{
    private TcpListener _listener;
    private CancellationTokenSource cancel;

    public BridgeServer(int port)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        cancel = new CancellationTokenSource();
    }

    public void Start()
    {
        _listener.Start();
        Debug.Log("Bridge Server started!");

        Task.Run(async () => {
            while (!cancel.IsCancellationRequested)
            {
                TcpClient client = _listener.AcceptTcpClient();
                Debug.Log("Bridge Client connected! Listening...");

                await HandleClientAsync(client, cancel.Token);
            }
        }, cancel.Token);
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] data = new byte[1024];

            while (client.Connected && !cancellationToken.IsCancellationRequested)
            {
                int bytes = await stream.ReadAsync(data, 0, data.Length, cancellationToken);
                if (bytes == -1)
                {
                    break;
                }

                string incomingData = Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: " + incomingData);
            }
        }
        catch (Exception ex)
        {
            Debug.Error("Error handling bridge client: " + ex.Message, this);
        }
        finally
        {
            client.Close();
        }
    }
}
