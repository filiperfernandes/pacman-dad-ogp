using System;
using System.Collections.Generic;

namespace RemotingInterfaces
{
    public interface IServer
    {
        List<IClient> RegisterClient(string NewClientPort);
        void AddMoves(int gameID, List<bool> moves);
    }

    public interface IClient
    {
        void MsgToClient(string message);
        void SendMsg(string message);
        void AddNewPlayer(string NewClientName);
        void SetGameID(int gameID);
        void PlayMoves(Dictionary<string, Tuple<string, int, int, int>> whatToSend);
        void setInitialGame(List<Tuple<string, string, int, int, int, int, int>> myList);
    }
}
