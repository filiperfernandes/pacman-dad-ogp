using System;
using System.Collections.Generic;

namespace RemotingInterfaces
{
    public interface IServer
    {
        List<IClient> RegisterClient(string NewClientPort);
        void AddMoves(int gameID, List<bool> moves);
        void Crash();
        int isAlive();
    }

    public interface IClient
    {
        void MsgToClient(string message);
        void SendMsg(string message);
        void AddNewPlayer(string NewClientName);
        void SetGameID(int gameID);
        void PlayMoves(Dictionary<string, Tuple<string, int, int, int>> whatToSend);
        void setInitialGame(List<Tuple<string, string, int, int, int, int, int>> myList);
        void Crash();
    }

    public interface IPCS
    {
        void createReplica(string pid, string pcs_url, string cli_srv_url, int msec_per_round, int num_players, int cli);
    }

    public interface IReplica
    {
        void Freeze();
        void Unfreeze();
        void Crash(string url);
        string GlobalStatus();
        
    }
}
