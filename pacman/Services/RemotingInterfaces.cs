using System;
using System.Collections.Generic;

namespace RemotingInterfaces
{
    public interface IServer
    {
        List<IClient> RegisterClient(string NewClientPort);
        void SendMsg(string message);
        void InformNewClientArrival(string NewClientName);
    }

    public interface IClient
    {
        void MsgToClient(string message);
        void SendMsg(string message);
        void AddNewPlayer(string NewClientName);
    }
}
