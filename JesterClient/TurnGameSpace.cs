using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public class TurnGameSpace : GameSpace
	{
		private TurnGamePlayer currentPlayer;
		
		public TurnGameSpace(int id, ClientSession session) : base (id, session)
		{
		}

        public TurnGamePlayer getCurrentPlayer()
        {
            return currentPlayer;
        }

		protected override GamePlayer createGamePlayer(Client client, BinaryReader additionalInitInfo)
		{
			int orientation = additionalInitInfo.ReadByte();
			return new TurnGamePlayer(client, getId(), orientation);
		}
		
		public bool isPlayerOnGame(int screenPos, bool onlyHuman = false)
		{
			return getPlayerWithScreenPos(screenPos, onlyHuman) != null;
		}
		
		public TurnGamePlayer getPlayerWithScreenPos(int screenPos, bool onlyHuman = false)
		{
            List<int> keys = new List<int>(clientsMap.Keys);
			foreach (int key in keys)
            {
				Client client = clientsMap[key];
				GamePlayer gamePlayer = client.getPlayer(getId());
				TurnGamePlayer turnPlayer = (TurnGamePlayer) gamePlayer;
				if (turnPlayer.screenPos == screenPos) 
                {
					return turnPlayer;
				}
			}
			
			if (onlyHuman) 
            {
				return null;
			}
			
			// if checking not only human check the robots
            keys = new List<int>(robotsMap.Keys);
            foreach (int key in keys)
            {
				TurnGamePlayer turnPlayer = (TurnGamePlayer)robotsMap[key];
				if (turnPlayer.screenPos == screenPos) 
                {
					return turnPlayer;
				}
			}
			
			return null;
		}

        protected override RobotInterface createRobot(BinaryReader message)
        {
            int robotIndex = message.ReadByte();
            int screenPos = message.ReadByte();
            int spaceId = getId();
            return new TurnRobotPlayer(robotIndex, spaceId, screenPos);
        }

		public override bool callFunction(int fnc, BinaryReader message, CompletionCallback callback = null)
		{
			bool ret = base.callFunction(fnc, message, callback);
            JesterLogger.log("TurnGameSpace.callFunction, ret: " + ret + ", fnc: " + fnc);

			if (ret) 
            {
				return true;
			}
			
			switch (fnc) 
            {
			case TurnGameProtocol.NOTIFY_CURRENT_PLAYER:
                JesterLogger.log("NOTIFY_CURRENT_PLAYER ");
				int clientId = message.ReadInt16();
                JesterLogger.log("clientId: " + clientId);
				Client client = clientsMap[clientId];
                JesterLogger.log("client: " + client);
				notifyCurrentPlayer(client, callback);
                return true;
			
			case TurnGameProtocol.NOTIFY_CURRENT_PLAYER_ROBOT:
                JesterLogger.log("TurnGameSpace.NOTIFY_CURRENT_PLAYER_ROBOT");
				int robotIndex = message.ReadByte();
                JesterLogger.log("TurnGameSpace.RobotsMap sz: " + robotsMap.Count + ", robotIdx: " + robotIndex);
                List<int> dellist = new List<int>(robotsMap.Keys);
                for (int i = 0; i < dellist.Count; i++)
                {
                    JesterLogger.log("TurnGameSpace.RobotsMap, i: " + i + ", key: " + dellist[i]);
                }
				RobotInterface robot = robotsMap[robotIndex];
                JesterLogger.log("TurnGameSpace.RobotsMap 2....");
				notifyCurrentPlayerRobot(robot, callback);
                return true;
			
			case TurnGameProtocol.TURN_STARTED:
                List<Object> objects = new List<Object>();
                objects.Add(message);
                eventQueue.addFunction(new Event(TurnGameProtocol.TURN_STARTED, objects));
                return true;

			case TurnGameProtocol.TURN_FINISHED:
                objects = new List<Object>();
                objects.Add(message);
                eventQueue.addFunction(new Event(TurnGameProtocol.TURN_FINISHED, objects));
                return true;

			case TurnGameProtocol.MATCH_STARTED:
				objects = new List<Object>();
                objects.Add(message);
                eventQueue.addFunction(new Event(TurnGameProtocol.MATCH_STARTED, objects));
                return true;
			
			case TurnGameProtocol.MATCH_FINISHED:
				objects = new List<Object>();
                objects.Add(message);
                eventQueue.addFunction(new Event(TurnGameProtocol.MATCH_FINISHED, objects));
                return true;

			case TurnGameProtocol.SET_SELECTABLES_BY_PLAYER:
				int playerId = message.ReadInt32();
				bool selectable = message.ReadBoolean();
				GamePlayer gamePlayer = gamePlayersMap[id];
				objects = new List<Object>();
                objects.Add(gamePlayer);
                objects.Add(selectable);
                eventQueue.addFunction(new Event(TurnGameProtocol.SET_SELECTABLES_BY_PLAYER, objects));
                return true;
			
			case TurnGameProtocol.SET_THIS_PLAYER_SELECTABLES:
				selectable = message.ReadBoolean();
				objects = new List<Object>();
                objects.Add(thisPlayer);
                objects.Add(selectable);
                eventQueue.addFunction(new Event(TurnGameProtocol.SET_THIS_PLAYER_SELECTABLES, objects));
                return true;
			}
			
			return false;
		}
		
		public void nextPlayer(int position)
		{
		}

		public void notifyCurrentPlayer(Client client, CompletionCallback callback)
		{
            JesterLogger.log("TurnGameSpace.notifyCurrentPlayer");
            int id = getId();
            JesterLogger.log("getId(): " + id);
            GamePlayer gamePlayer = client.getPlayer(id);
            JesterLogger.log("GamePlayer: " + gamePlayer);
            JesterLogger.log("GamePlayer2: " + gamePlayer.getName());
            this.currentPlayer = (TurnGamePlayer)gamePlayer;
            JesterLogger.log("GamePlayer3: " + this.currentPlayer);
            JesterLogger.log("TurnGameSpace.notifyCurrentPlayer, currentPlayer: " + currentPlayer.getId());

            List<Object> objects = new List<Object>();
            objects.Add(client);
            eventQueue.addFunction(new Event(TurnGameProtocol.NOTIFY_CURRENT_PLAYER, objects));
		}
		
		public void notifyCurrentPlayerRobot(RobotInterface robot, CompletionCallback callback)
		{
            JesterLogger.log("TurnGameSpace.notifyCurrentPlayerRobot(), robot: " + robot);
			TurnGamePlayer robotPlayer = (TurnGamePlayer)robot;
			if (robotPlayer == null) 
            {
				throw new Exception("NullPointerException()");
			}
            JesterLogger.log("TurnGameSpace.notifyCurrentPlayerRobot(), robot:2");
			this.currentPlayer = robotPlayer;
            JesterLogger.log("TurnGameSpace.notifyCurrentPlayerRobot(), robot:3");
            List<Object> objects = new List<Object>();
            objects.Add(robot);
            JesterLogger.log("TurnGameSpace.notifyCurrentPlayerRobot(), robot:4");
            eventQueue.addFunction(new Event(TurnGameProtocol.NOTIFY_CURRENT_PLAYER_ROBOT, objects));
            JesterLogger.log("TurnGameSpace.notifyCurrentPlayerRobot() out");
		}
		
		public bool isThisCurrentPlayer()
		{
			if (currentPlayer == null) 
            {
				return false;
			}
			
			if (currentPlayer.isRobot()) 
            {
                JesterLogger.log("TurnGameSpace.isThisCurrentPlayer(): " + currentPlayer.isRobot());
				return false;
			}

            int id_ = thisPlayer.getClient().getId();
            int currId_ = currentPlayer.getClient().getId();
            JesterLogger.log("TurnGameSpace.isThisCurrentPlayer(), id: " + id_ + ", currId: " + currId_);
			return id_ == currId_;
		}
	}
}