using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public class LobbyProtocol
	{
		// client output events
		public const int PLAY_NOW = 30;
		public const int CHOOSE_PLACE = 31;
		public const int GET_LOBBY_INFO = 32;
		public const int SWITCH_PAGE = 33;
		public const int SWITCH_LOBBY = 34;

		// server input events
		public const int CANNOT_ENTER = 35;
		public const int ENTER_GAME = 36;
		public const int SIGN_PLACE_TO_ENTER = 37;
		public const int LOBBY_INFO = 38;
		public const int ON_GAME_START = 39;
		public const int ON_GAME_FINISH = 40;
		public const int ON_SWITCH_LOBBY = 41;
		public const int ON_SWITCH_PAGE = 42;
	}
}
