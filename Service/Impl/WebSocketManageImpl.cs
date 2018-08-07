using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Service.Impl
{
    /// <summary>
    /// WebSocket管理
    /// </summary>
    public class WebSocketManageImpl : IWebSocketManage
    {
        private readonly Dictionary<String, IServiceWebSocket> serviceWebSockets;
        private readonly Dictionary<String, IClientWebSocket> clientWebSockets;

        public WebSocketManageImpl()
        {
            serviceWebSockets = new Dictionary<String, IServiceWebSocket>();
            clientWebSockets = new Dictionary<String, IClientWebSocket>();
        }

        public Boolean RegisterService(String id, IServiceWebSocket serviceWebSocket)
        {
            if (!serviceWebSockets.ContainsKey(id))
            {
                serviceWebSockets.Add(id, serviceWebSocket);
                return true;
            }

            return false;
        }

        public Boolean UnRegisterService(String id)
        {
            if (serviceWebSockets.ContainsKey(id))
            {
                /* 重置狀態 */
                if (clientWebSockets.ContainsKey(serviceWebSockets[id].WebSocketClientId))
                {
                    clientWebSockets[serviceWebSockets[id].WebSocketClientId].Reset();
                }

                serviceWebSockets.Remove(id);
                return true;
            }

            return false;
        }

        public Boolean SendToService(String id, String message)
        {
            try
            {
                if (serviceWebSockets.ContainsKey(id))
                {
                    Byte[] data = Encoding.UTF8.GetBytes(message);

                    Send(serviceWebSockets[id].WebSocket, data);

                    return true;
                }
            }
            catch (Exception)
            {

            }

            return false;
        }

        public bool RegisterClient(string id, IClientWebSocket clientWebSocket)
        {
            if (!clientWebSockets.ContainsKey(id))
            {
                clientWebSockets.Add(id, clientWebSocket);
                return true;
            }

            return false;
        }

        public bool UnRegisterClient(string id)
        {
            if (clientWebSockets.ContainsKey(id))
            {
                /* 重置狀態 */
                if (serviceWebSockets.ContainsKey(clientWebSockets[id].WebSocketServiceId))
                {
                    serviceWebSockets[clientWebSockets[id].WebSocketServiceId].Reset();
                }

                clientWebSockets.Remove(id);
                return true;
            }

            return false;
        }

        public Boolean SetWebSocketServiceId(String id, String serviceId)
        {
            if (clientWebSockets.ContainsKey(id))
            {
                clientWebSockets[id].WebSocketServiceId = serviceId;
                return true;
            }

            return false;
        }

        public bool SendToClient(string id, string message)
        {
            try
            {
                if (clientWebSockets.ContainsKey(id))
                {
                    Byte[] data = Encoding.UTF8.GetBytes(message);

                    Send(clientWebSockets[id].WebSocket, data);

                    return true;
                }
            }
            catch (Exception)
            {

            }

            return false;
        }

        private void Send(WebSocket webSocket, Byte[] data)
        {
            webSocket.SendAsync(
                new ArraySegment<Byte>(data, 0, data.Length),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }
    }
}
