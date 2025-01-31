﻿using System;
using System.Collections.Generic;

namespace RemotingInterfaces
{
    public interface IServer
    {
        List<IClient> RegisterClient(string NewClientPort);
        void AddMoves(int gameID, List<bool> moves);
        void Crash();
        int isAlive();
        void Freeze();
        void Unfreeze();
        
    }

    public interface IClient
    {
        void MsgToClient(string message, int[] messageVector);
        void SendMsg(string message, int[] messageVector);
        void AddNewPlayer(string NewClientName);
        void SetGameID(int gameID);
        void PlayMoves(Dictionary<string, Tuple<string, int, int, int>> whatToSend, int round);
        void setInitialGame(List<Tuple<string, string, int, int, int, int, int>> myList, int round);
        String getLostMsgFromClient(int numMsg);
        String GetMsg(int numMsg);
        void Crash();
        int isAlive();
        void Freeze();
        void Unfreeze();
        void GameOver();
        void Winner();
        Dictionary<string, Tuple<string, int, int, int>> localState(int round);
        void addInputFile(Dictionary<int, List<bool>> clientMoves);
    }


    public interface IPCS
    {
        void close();
        void createReplica(string pid, string pcs_url, string cli_srv_url, int msec_per_round, int num_players, int cli, string path);
    }

    public interface IReplica
    {
        void Freeze();
        void Unfreeze();
        void Crash(string url);
        string GlobalStatus();

    }
}
