using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public interface MatchMakerGraphicsInterface : SpaceGraphicsInterface
	{
		void showEnterGame(int gameId, byte gameEnum);
		void showCannotEnter();
		void onGameStart();
		void onGameFinish();
		
	}

}
