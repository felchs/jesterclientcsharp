using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkstarSharp;

namespace JesterClient
{
    public class GameSpace : Space
	{
		public Dictionary<int, GamePlayer> gamePlayersMap = new Dictionary<int, GamePlayer>();
		
		public Dictionary<int, RobotInterface> robotsMap = new Dictionary<int, RobotInterface>();
		
		public GamePlayer thisPlayer;

		public GameSpace(int id, ClientSession clientSession) : base(id, clientSession)
		{
		}
		
		public GamePlayer getPlayer(int key)
		{
			return gamePlayersMap[key];
		}
		
		public GamePlayer getPlayerWithClientId(int clientId)
		{
            JesterLogger.log("GameSpace.getPlayerWithClientId, clientId: " + clientId);
            JesterLogger.log("GameSpace.getPlayerWithClientId, clientsMapCount: " + clientsMap.Count);

            int idx = 0;
            List<int> list = new List<int>(clientsMap.Keys);
            foreach (int k in list)
            {
                JesterLogger.log("Key: " + k + ", idx: " + idx++);
            }
            
			Client client = clientsMap[clientId];
            JesterLogger.log("GameSpace.getPlayerWithClientId, client: " + client);
			GamePlayer gamePlayer = client.getPlayer(getId());
            JesterLogger.log("GameSpace.getPlayerWithClientId, gamePlayer: " + gamePlayer);
			return gamePlayer;
		}
		
		public RobotInterface getRobot(int key)
		{
			return robotsMap[key];
		}
		
		protected override void updateOnClientEnter(Client client, BinaryReader additionalInitInfo)
		{
            JesterLogger.log("GameSpace.updateOnClientEnter");
			GamePlayer player = createGamePlayer(client, additionalInitInfo);
			
			if (thisPlayer == null)
            {
				this.thisPlayer = player;
			}
			
			client.addPlayer(player);
			gamePlayersMap.Add(player.getClient().getId(), player);
			
			base.updateOnClientEnter(client, additionalInitInfo);
		}

		protected virtual GamePlayer createGamePlayer(Client client, BinaryReader additionalInitInfo)
		{
			return new GamePlayer(client, getId());
		}
		
		protected void putGameRobotEnter(BinaryReader message)
		{
			RobotInterface robotPlayer = createRobot(message);
            JesterLogger.log("GameSpace.putGameRobotEnter, getRobotIndex(): " + robotPlayer.getRobotIndex());
			robotsMap.Add(robotPlayer.getRobotIndex(), robotPlayer);

            List<Object> objects = new List<Object>();
            objects.Add(robotPlayer);
            eventQueue.addFunction(new Event(GameProtocol.GAME_ROBOT_ENTER, objects));
		}
		
		protected void gameRobotReplacement(BinaryReader message)
		{
			int clientId = message.ReadInt16();
			int robotIndex = message.ReadByte();
            Client client = clientsMap[clientId];
            RobotInterface gameRobot = robotsMap[robotIndex];
			
            List<Object> objects = new List<Object>();
            objects.Add(client);
            objects.Add(gameRobot);
            eventQueue.addFunction(new Event(GameProtocol.GAME_ROBOT_REPLACEMENT, objects));
		}
		
		protected virtual RobotInterface createRobot(BinaryReader message)
		{
            int robotIndex = message.ReadByte();
			return new GenericGameRobot(robotIndex, getId());
		}
		
		protected void onGameRobotExit(BinaryReader message)
		{
			int robotIndex = message.ReadByte();
            RobotInterface gameRobot = robotsMap[robotIndex];
            List<Object> objects = new List<Object>();
            objects.Add(gameRobot);
            eventQueue.addFunction(new Event(GameProtocol.GAME_ROBOT_EXIT, objects));
		}
		
		public override bool callFunction(int fnc, BinaryReader message, CompletionCallback callback = null)
		{
			bool ret = base.callFunction(fnc, message, callback);
			if (ret)
            {
				return true;
			}
			
            JesterLogger.log("GameSpace.function: " + fnc);

			switch (fnc) 
            {
				case GameProtocol.GAME_PLAYER_GIVE_UP:
                    List<Object> objects = new List<Object>();
                    int clientId = message.ReadInt16();
                    GamePlayer gamePlayer = getPlayerWithClientId(clientId);
                    objects.Add(gamePlayer);
                    eventQueue.addFunction(new Event(GameProtocol.GAME_PLAYER_GIVE_UP, objects));
                    clientsMap.Remove(clientId);
                    return true;
				
				case GameProtocol.GAME_PLAYER_FALL:
                    objects = new List<Object>();
                    clientId = message.ReadInt16();
                    gamePlayer = getPlayerWithClientId(clientId);
                    objects.Add(gamePlayer);
                    eventQueue.addFunction(new Event(GameProtocol.GAME_PLAYER_FALL, objects));
                    clientsMap.Remove(clientId);
                    return true;
				
				case GameProtocol.GAME_STARTED:
                    objects = new List<Object>();
                    objects.Add(message);
                    eventQueue.addFunction(new Event(GameProtocol.GAME_STARTED, objects));
                    return true;
				
				case GameProtocol.GAME_STOPPED:
                    objects = new List<Object>();
                    objects.Add(message);
                    eventQueue.addFunction(new Event(GameProtocol.GAME_STOPPED, objects));
                    return true;

				case GameProtocol.GAME_RESULTS:
                    objects = new List<Object>();
                    objects.Add(message);
                    eventQueue.addFunction(new Event(GameProtocol.GAME_RESULTS, objects));
                    return true;
				
				case GameProtocol.UPDATE_SCORE:
                    objects = new List<Object>();
                    objects.Add(message);
                    eventQueue.addFunction(new Event(GameProtocol.UPDATE_SCORE, objects));
                    return true;
				
				case GameProtocol.GAME_FINISHED:
                    objects = new List<Object>();
                    objects.Add(message);
                    eventQueue.addFunction(new Event(GameProtocol.GAME_FINISHED, objects));
                    return true;
				
				case GameProtocol.GAME_CAN_RESTART:
					bool canRestartFlag = message.ReadBoolean();
                    objects = new List<Object>();
                    objects.Add(canRestartFlag);
                    eventQueue.addFunction(new Event(GameProtocol.GAME_CAN_RESTART, objects));
                    return true;
				
				case GameProtocol.GAME_ROBOT_ENTER:
					putGameRobotEnter(message);
                    return true;
				
				case GameProtocol.GAME_ROBOT_REPLACEMENT:
					gameRobotReplacement(message);
                    return true;
				
				case GameProtocol.GAME_ROBOT_EXIT:
					onGameRobotExit(message);
                    return true;
			}

            return false;
		}

		public void sendGameRestart()
		{
            MemoryStream m = new MemoryStream();
            m.SetLength(4 + 1);
            BinaryWriter bw = new BinaryWriter(m);
            int id_ = getId();
			bw.Write(Converter.GetBigEndian(id_));
            byte event_ = GameProtocol.GAME_RESTART;
            bw.Write(event_);
			sessionSend(m);
		}
		
		public void sendGameFillWithBots()
		{
            MemoryStream m = new MemoryStream();
            m.SetLength(4 + 1);
            BinaryWriter bw = new BinaryWriter(m);
            int id_ = getId();
            bw.Write(Converter.GetBigEndian(id_));
            byte event_ = GameProtocol.GAME_FILL_WITH_ROBOTS;
            bw.Write(event_);
			sessionSend(m);
		}
	}
}