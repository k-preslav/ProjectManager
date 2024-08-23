using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BraketsEngine
{
    public class BridgeClient
    {
        public event Action<string> OnReceive;

        private TcpClient _client;
        private CancellationTokenSource _cancel;
        private NetworkStream _stream;

        public BridgeClient()
        {
            _client = new TcpClient();
            _cancel = new CancellationTokenSource();
        }

        public async Task ConnectAsync(string hostname, int port)
        {
            try
            {
                await _client.ConnectAsync(hostname, port, _cancel.Token);
                _stream = _client.GetStream();
                Debug.Log("Connected to bridge server");

                // Start listening for incoming messages
                _ = ListenForMessagesAsync(_cancel.Token);
            }
            catch (Exception ex)
            {
                Debug.Error("Error connecting to bridge server: " + ex.Message, this);
            }
        }

        public async void SendMessageAsync(string message)
        {
            if (_client is null || !_client.Connected || _stream is null)
                return;

            try
            {
                byte[] data = Encoding.ASCII.GetBytes(message);
                await _stream.WriteAsync(data, 0, data.Length, _cancel.Token);
            }
            catch (Exception ex)
            {
                Debug.Error("Error sending message to bridge server: " + ex.Message, this);
                Disconnect();
            }
        }

        private async Task ListenForMessagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                byte[] buffer = new byte[1024];

                while (_client.Connected && !cancellationToken.IsCancellationRequested)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                    if (bytesRead > 0)
                    {
                        string responseData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        OnReceive?.Invoke(responseData);
                    }

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Debug.Error("Error receiving message from bridge server: " + ex.Message, this);
                Disconnect();
            }
        }

        public void Disconnect()
        {
            _cancel.Cancel();
            _client.Close();
            Debug.Log("Disconnected from bridge server.");
        }
    }
}
