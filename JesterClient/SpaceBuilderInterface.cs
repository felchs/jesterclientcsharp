using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public interface SpaceBuilderInterface 
	{
        int getInitialLobbyPage();

        int getInitialLobbyToEnter();

        int getDefaultSpaceToEnter();

        int getMaxNumLobbyPages();

        //string getSpaceIdMapping();

        //SpaceGraphicsInterface getGameGraphics(int id);

        //SpaceGraphicsInterface getLobbyGraphics(int id);

		LobbySpace createLobby(Space parentSpace, int id, int lobbyPage, ClientSession eventsHandler);

		MatchMakerSpace createMatchMaker(int id, ClientSession eventsHandler);

		GameSpace createGameMatchMaker(Space parentSpace, int spaceEnum);

		GameSpace createGame(Space parentSpace, int place, int subPlace, int lobbyPage);
		
		int encodeLobbyId(int spaceEnum, int lobbyEnum, int lobbyPage);
		
		int decodeSpaceEnum(int id);
		
		int decodeLobbyEnum(int id);
		
		int decodeLobbyPage(int id);

		int getSpaceID(int spaceEnum, int lobbyEnum, int lobbyPage, int place);
	}
}