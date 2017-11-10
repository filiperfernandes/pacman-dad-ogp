using System;
using System.Collections.Generic;

namespace RemotingInterfaces
{
    public interface IServer
    {
        List<IClient> RegisterClient(string NewClientPort);
        void CheckTime(Boolean time);
        void SendMsg(string message);
        void ReadPlay(String move);
        void InformNewClientArrival(string NewClientName);
    }

    public interface IClient
    {
        void MsgToClient(string message);
        void SendMsg(string message);
        void AddNewPlayer(string NewClientName);
        void UpdateGame(List<string> Moves);
    }
}
