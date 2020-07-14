using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public class MatchMakerProtocol
	{
		// client output events
		public const int PLAY_NOW = 30;
		public const int PLAY_NOW_WITH_ROBOTS = 31;

		// server input events
		public const int CANNOT_ENTER = 32;
		public const int ENTER_GAME = 33;
		public const int MATCHMAKER_INFO = 34;
		public const int ON_GAME_START = 35;
		public const int ON_GAME_FINISH = 36;
	}
}
