using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using DarkstarSharp;

namespace JesterClient
{
	public class LobbySpace : Space
	{
		private Channel chatCh;
		
		protected int lobbyPage;
		protected int lobbyEnum;
		protected int spaceEnum;

        protected int placeId;
        protected int subPlaceId;

        private bool getLobbyInfoEnabled = true;

		protected GameSpace gameSpace;
		
		private bool playingGame;
        
        protected Timer lobbyInfoTimer = new Timer(5000);
		
		public LobbySpace(int id, int lobbyPage, int spaceEnum, int lobbyEnum, ClientSession clientSession) : base (id, clientSession)
		{
			this.lobbyPage = lobbyPage;
			this.spaceEnum = spaceEnum;
			this.lobbyEnum = lobbyEnum;
		}

        public int getLobbyPage()
        {
            return lobbyPage;
        }

        public int getLobbyEnum()
        {
            return lobbyEnum;
        }

        public int getSpaceEnum()
        {
            return spaceEnum;
        }

        public int getPlaceId()
        {
            return placeId;
        }

        public int getSubPlaceId()
        {
            return subPlaceId;
        }

        public GameSpace getGameSpace()
        {
            return gameSpace;
        }

        public bool isGetLobbyInfoEnabled()
        {
            return getLobbyInfoEnabled;
        }

        public void setGetLobbyInfoEnabled(bool getLobbyInfoEnabled)
        {
            this.getLobbyInfoEnabled = getLobbyInfoEnabled;
        }
		
		private string getChannelChatName()
		{
			return "LobbyChat" + lobbyEnum + "_" + spaceEnum;
		}
		
		new public void channelJoin(Channel channel)
		{
			base.channelJoin(channel);
			
			if (channel.getName().Equals(getChannelChatName())) {
				chatCh = channel;
			}
		}

		/*
		public void onThisClientExitGame(Client client)
		{
			onShowLobbyFromGame(false);
		}
		
        /*
		public void onShowLobbyFromGame(bool playingGame)
		{
			this.playingGame = playingGame;

			getLobbyGraphics().onShowLobbyFromGame(playingGame);
			((LobbySceneSwitcher)gameSpace.getGraphics()).onShowLobbyFromGame(playingGame);
            getLobbyGraphics().switchScene(getId());
		}
		
		public void onShowGameFromLobby(bool playingGame)
		{
			getLobbyGraphics().onShowGameFromLobby(playingGame);
			((SceneSwitcher)getGameGraphics()).switchScene(getId());
		}
        */
		
		// Input events ------------------------------------------------------------
		
        protected override void updateOnClientEnter(Client client, BinaryReader additionalInitInfo)
        {
            //JesterLogger.log("_____updateOnclientEnter,lobby, id: " + getId());

			base.updateOnClientEnter(client, additionalInitInfo);
			
			lobbyInfoTimer.Elapsed += new ElapsedEventHandler(getLobbyInfoEvent);
			lobbyInfoTimer.Enabled = true;
			
			getCurrLobbyInfo();
		}
		
		protected override void updateOnClientExit(Client client)
		{
			base.updateOnClientExit(client);
		}
		
		public override void removeAsEventListener()
		{
			base.removeAsEventListener();
			
			if (lobbyInfoTimer != null)
			{
				lobbyInfoTimer.Stop();
			}
		}

		public override bool callFunction(int fnc, BinaryReader message, CompletionCallback callback = null)
		{
			bool ret = base.callFunction(fnc, message, callback);

			if  (ret)
            {
				return true;
			}

			switch (fnc) 
            {
			case LobbyProtocol.CANNOT_ENTER:
                    List<Object> objects = new List<Object>();
                    eventQueue.addFunction(new Event(LobbyProtocol.CANNOT_ENTER, objects));
                    return true;
			
			case LobbyProtocol.ENTER_GAME:
                this.placeId = message.ReadByte();
                this.subPlaceId = message.ReadByte();
                this.lobbyPage = message.ReadInt16();
				//onShowGameFromLobby(false);
				this.playingGame = true;

                objects = new List<Object>();
                eventQueue.addFunction(new Event(LobbyProtocol.ENTER_GAME, objects));
                return true;
			
			case LobbyProtocol.SIGN_PLACE_TO_ENTER:
				placeId = message.ReadByte();
				subPlaceId = message.ReadByte();
				lobbyPage = message.ReadByte();
                objects = new List<Object>();
                eventQueue.addFunction(new Event(LobbyProtocol.SIGN_PLACE_TO_ENTER, objects));
				
				sendChoosePlace(getId(), placeId, subPlaceId, lobbyPage);
                return true;
			
			case LobbyProtocol.LOBBY_INFO:
                //JesterLogger.log("lobbyinfo added event...1");
				while (message.PeekChar() != -1)
                {
					int mask = 7;
					int buf = message.ReadByte();
					subPlaceId = buf & mask; // 3 bits for chair sub place
					mask = 15; // 4 bits for table place
					placeId = (buf >> 4) & mask;
					int clientId = message.ReadInt16(); // 1 short
					Client client = clientsMap[clientId];

                    objects = new List<Object>();
                    objects.Add(placeId);
                    objects.Add(subPlaceId);
                    objects.Add(LobbyPlaceState.BUSY);
                    objects.Add(client);
                    eventQueue.addFunction(new Event(LobbyProtocol.LOBBY_INFO, objects));
                    //JesterLogger.log("lobbyinfo added event...2");
				}
                return true;
			
			case LobbyProtocol.ON_GAME_START:
                objects = new List<Object>();
                eventQueue.addFunction(new Event(LobbyProtocol.ON_GAME_START, objects));
                return true;
			
			case LobbyProtocol.ON_GAME_FINISH:
                objects = new List<Object>();
                eventQueue.addFunction(new Event(LobbyProtocol.ON_GAME_FINISH, objects));
                return true;
			
			case LobbyProtocol.ON_SWITCH_LOBBY:
				int lobbyId = message.ReadInt32();
				lobbyPage = message.ReadByte();
                Space parent = getParentSpace();
                LobbySpace lobbySpace = SpaceBuilder.get().createLobby(parent, lobbyId, lobbyPage, clientSession);

                objects = new List<Object>();
                eventQueue.addFunction(new Event(LobbyProtocol.ON_SWITCH_LOBBY, objects));
                return true;
			
			case LobbyProtocol.ON_SWITCH_PAGE:
				lobbyId = message.ReadInt32();
				lobbyPage = message.ReadByte();
                objects = new List<Object>();
                eventQueue.addFunction(new Event(LobbyProtocol.ON_SWITCH_PAGE, objects));
                return true;

			//case LobbyProtocol.UPDATE_INDIVIDUAL_RANKING:
			//	var clientID:int = message.readShort();
			//	var client:Client = clientsMap.getValue(clientID);
			//	var points:Number = message.readFloat();
			//	var tiePoints:Number = 0;// message.readFloat();
			//	lobbyUsersInfo.updatePoints(client, points, tiePoints);
			//return new FncRet();
			//
			//case LobbyProtocol.UPDATE_RANKING_POINTS:
			//	var numClients:int = message.readShort();
			//	for (var i:int = 0; i < numClients; i++) {
			//		clientID = message.readShort();
			//		client = clientsMap.getValue(clientID);
			//		points = message.readFloat();
			//		tiePoints = 0;// message.readFloat();
			//		lobbyUsersInfo.updatePoints(client, points, tiePoints);
			//	}
			//return new FncRet();
			}
			
			return false;
		}
		
		private void removeSpaceAsChatReceiver()
		{
			// remove chat channels from chat interface
            List<string> keys = new List<string>(channelsMap.Keys);
			foreach (string key in keys)
            {
				Channel channel = channelsMap[key];
				if (channel.getName().ToLower().Contains("chat"))
                {
					getChatInterface().onChannelLeave(channel);
				}
			}
		}
		
		// Output events ------------------------------------------------------------
		
		public void sendPlayNow()
		{
			if (playingGame) 
            {
				return;
			}

			MemoryStream m = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(m);
            int id_ = getId();
			bw.Write(Converter.GetBigEndian(id_));
            byte event_ = LobbyProtocol.PLAY_NOW;
			bw.Write(event_);
			clientSession.sessionSend(m);
		}
		
		public void sendChoosePlace(int lobbyId, int placeId, int subPlaceId, int lobbyPage)
		{
            //JesterLogger.log("_____LobbySpace.sendChoosePlace, lobbyId: " + lobbyId + ", placeId: " + placeId + ", subPlaceId: " + subPlaceId + ", lobbyPage: " + lobbyPage);
			this.gameSpace = SpaceBuilder.get().createGame(this, placeId, subPlaceId, lobbyPage);
			//gameSpace.setChatInterface(chatInterface);
			//gameSpace.wait();
			
            MemoryStream m = new MemoryStream();
            m.SetLength(4 + 1 + 1 + 1);
			BinaryWriter bw = new BinaryWriter(m);
            int id_ = getId();
			bw.Write(Converter.GetBigEndian(id_));
            byte choosePlace_ = LobbyProtocol.CHOOSE_PLACE;
			bw.Write(choosePlace_);
            byte placeId_ = (byte)placeId;
			bw.Write(placeId_);
            byte subPlaceId_ = (byte)subPlaceId;
			bw.Write(subPlaceId_);
			clientSession.sessionSend(m);
		}

		private void getLobbyInfoEvent(object sender, ElapsedEventArgs e)
		{
            if (getLobbyInfoEnabled)
            {
                getCurrLobbyInfo();
            }
		}
		
		public void getCurrLobbyInfo()
		{
			getLobbyInfo(lobbyPage);
		}

		public void getLobbyInfo(int lobbyPage)
		{
            MemoryStream m = new MemoryStream();
            m.SetLength(4 + 1);
            BinaryWriter bw = new BinaryWriter(m);
            int id_ = getId();
			bw.Write(Converter.GetBigEndian(id_));
            byte event_ = LobbyProtocol.GET_LOBBY_INFO;
			bw.Write(event_);
			clientSession.sessionSend(m);
		}

		public void sendSwitchPage(int lobbyId)
		{
            MemoryStream m = new MemoryStream();
            m.SetLength(4 + 1 + 4);
            BinaryWriter bw = new BinaryWriter(m);
            int id_ = getId();
            bw.Write(Converter.GetBigEndian(id_));
            byte event_ = LobbyProtocol.SWITCH_PAGE;
			bw.Write(event_);
			bw.Write(Converter.GetBigEndian(lobbyId));
			clientSession.sessionSend(m);
		}
		
		public void sendSwitchLobby(int lobbyId)
		{
            MemoryStream m = new MemoryStream();
            m.SetLength(4 + 1 + 4);
            BinaryWriter bw = new BinaryWriter(m);
            int id_ = getId();
            bw.Write(Converter.GetBigEndian(id_));
            byte event_ = LobbyProtocol.SWITCH_LOBBY;
			bw.Write(event_);
			bw.Write(Converter.GetBigEndian(lobbyId));
			clientSession.sessionSend(m);
		}
	}
}