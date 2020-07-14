using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public interface LobbySceneSwitcher : SceneSwitcher
	{
		void onShowLobbyFromGame(bool playingGame);
		void onShowGameFromLobby(bool playingGame);
	}
}
