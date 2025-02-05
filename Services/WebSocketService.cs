using System.Net.WebSockets;
using System.Text;

namespace Message.Services
{
    public class WebSocketService
    {
        private readonly List<WebSocket> _clients = new();
        private readonly object _lock = new();

        public async Task HandleClientAsync(WebSocket webSocket)
        {
            lock (_lock)
            {
                _clients.Add(webSocket);
            }

            var buffer = new byte[1024 * 4];
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                }
            }
            finally
            {
                lock (_lock)
                {
                    _clients.Remove(webSocket);
                }
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
            }
        }

        public async Task BroadcastMessageAsync(string message)
        {
            List<WebSocket> disconnectedClients = new();
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            lock (_lock)
            {
                foreach (var client in _clients)
                {
                    if (client.State == WebSocketState.Open)
                    {
                        _ = client.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else
                    {
                        disconnectedClients.Add(client);
                    }
                }

                foreach (var client in disconnectedClients)
                {
                    _clients.Remove(client);
                }
            }
        }
    }
}

