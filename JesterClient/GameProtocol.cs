using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public class GameProtocol
    {
		// input events
		public const int GAME_STARTED = 50;	
		public const int GAME_STOPPED = 51;
		public const int GAME_RESULTS = 52;
		public const int GAME_FINISHED = 53;
		public const int UPDATE_SCORE = 54;
		public const int GAME_PLAYER_GIVE_UP = 55;
		public const int GAME_PLAYER_FALL = 56;
		public const int GAME_CAN_RESTART = 57;
		
		// output events
		public const int GAME_RESTART = 58;
		public const int GAME_FILL_WITH_ROBOTS = 59;
		public const int GAME_ROBOT_ENTER = 60;
		public const int GAME_ROBOT_REPLACEMENT = 61;
		public const int GAME_ROBOT_EXIT = 62;

    }
}
