using DarkstarSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace JesterClient
{
	public class MatchMakerSpace : Space
	{
		protected GameSpace gameSpace;

		protected Timer matchInfoTimer = new Timer(5000);

		private bool playingGame = false;

		public MatchMakerSpace(int id, ClientSession clientSession) : base(id, clientSession)
		{
		}

		public GameSpace getGameSpace()
		{
			return gameSpace;
		}

		private string getChannelChatName()
		{
			return "MatchMaker_" + id;
		}

		new public void channelJoin(Channel channel)
		{
			/*
			base.channelJoin(channel);

			if (channel.getName().Equals(getChannelChatName()))
			{
				chatCh = channel;
			}
			*/
		}

		// Input events ------------------------------------------------------------

		protected override void updateOnClientEnter(Client client, BinaryReader additionalInitInfo)
		{
			base.updateOnClientEnter(client, additionalInitInfo);

			matchInfoTimer.Elapsed += new ElapsedEventHandler(getMatchMakerInfoEvent);
			matchInfoTimer.Enabled = true;

			//getCurrLobbyInfo();
		}

		protected override void updateOnClientExit(Client client)
		{
			base.updateOnClientExit(client);
		}

		public override void removeAsEventListener()
		{
			base.removeAsEventListener();

			if (matchInfoTimer != null)
			{
				matchInfoTimer.Stop();
			}
		}

		public override bool callFunction(int fnc, System.IO.BinaryReader message, CompletionCallback callback = null)
		{
			bool ret = base.callFunction(fnc, message, callback);

			if (ret)
			{
				return true;
			}

			switch (fnc)
			{
				case MatchMakerProtocol.CANNOT_ENTER:
					List<Object> objects = new List<Object>();
					eventQueue.addFunction(new Event(MatchMakerProtocol.CANNOT_ENTER, objects));
					return true;

				case MatchMakerProtocol.ENTER_GAME:
					int gameId = message.ReadInt16();
					byte gameEnum = message.ReadByte();

					objects = new List<Object>();
					objects.Add(gameId);
					objects.Add(gameEnum);
					eventQueue.addFunction(new Event(MatchMakerProtocol.ENTER_GAME, objects));
					return true;

				case MatchMakerProtocol.MATCHMAKER_INFO:
					/*
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
					*/
					return true;

				case MatchMakerProtocol.ON_GAME_START:
					objects = new List<Object>();
					eventQueue.addFunction(new Event(MatchMakerProtocol.ON_GAME_START, objects));
					return true;

				case MatchMakerProtocol.ON_GAME_FINISH:
					objects = new List<Object>();
					eventQueue.addFunction(new Event(MatchMakerProtocol.ON_GAME_FINISH, objects));
					return true;
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

			playingGame = true;

			MemoryStream m = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(m);
			int id_ = getId();
			bw.Write(Converter.GetBigEndian(id_));
			byte event_ = MatchMakerProtocol.PLAY_NOW;
			bw.Write(event_);
			clientSession.sessionSend(m);
		}

		public void sendPlayNowWithRobots()
		{
			if (playingGame)
			{
				return;
			}

			playingGame = true;

			MemoryStream m = new MemoryStream();
			BinaryWriter bw = new BinaryWriter(m);
			int id_ = getId();
			bw.Write(Converter.GetBigEndian(id_));
			byte event_ = MatchMakerProtocol.PLAY_NOW_WITH_ROBOTS;
			bw.Write(event_);
			clientSession.sessionSend(m);
		}


		private void getMatchMakerInfoEvent(object sender, ElapsedEventArgs e)
		{
			/*
			MemoryStream m = new MemoryStream();
			m.SetLength(4 + 1);
			BinaryWriter bw = new BinaryWriter(m);
			int id_ = getId();
			bw.Write(Converter.GetBigEndian(id_));
			byte event_ = LobbyProtocol.GET_LOBBY_INFO;
			bw.Write(event_);
			eventsHandler.sessionSend(m);
			*/
		}

		/*
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
			eventsHandler.sessionSend(m);
		}
		*/
	}
}
