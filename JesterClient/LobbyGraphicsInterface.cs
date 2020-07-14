using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public interface LobbyGraphicsInterface : SpaceGraphicsInterface, LobbySceneSwitcher
	{
		void onUpdatePlace(int place, int subPlace, int placeState, Client client);
		void showCannotEnter();
		void onGameStart();
		void onGameFinish();
		LobbySpace activateLobbyPage(int pageNumber);
	}
}
