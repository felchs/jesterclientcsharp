using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public class TurnGameProtocol
	{
		public const int NOTIFY_CURRENT_PLAYER = 81;
		public const int NOTIFY_CURRENT_PLAYER_ROBOT = 82;
		public const int SET_SELECTABLES_BY_PLAYER = 83;
		public const int SET_THIS_PLAYER_SELECTABLES = 84;
		public const int TURN_STARTED = 85;
		public const int TURN_FINISHED = 86;
		public const int MATCH_STARTED = 87;
		public const int MATCH_FINISHED = 88;
	}
}
