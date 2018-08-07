using System;

namespace Service
{
    public interface IWebSocketManage
    {
        Boolean RegisterService(String id, IServiceWebSocket serviceWebSocket);
        Boolean UnRegisterService(String id);
        Boolean SendToService(String id, String message);

        Boolean RegisterClient(String id, IClientWebSocket clientWebSocket);
        Boolean UnRegisterClient(String id);
        Boolean SetWebSocketServiceId(String id, String serviceId);
        Boolean SendToClient(String id, String message);
    }
}
