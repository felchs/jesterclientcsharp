using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public interface GameGraphicsInterface
	{
		void gamePlayerGiveUp(GamePlayer gamePlayer);
		void gamePlayerFall(GamePlayer gamePlayer);
		void gameRobotEnter(RobotInterface gameRobot);
        void gameRobotExit(RobotInterface gameRobot);
		void gameRobotReplacement(Client client, RobotInterface gameRobot);
		void gameStarted(BinaryReader message, CompletionCallback callback);
		void gameStopped(BinaryReader message, CompletionCallback callback);
		void gameFinished(BinaryReader message, CompletionCallback callback);
		void gameResults(BinaryReader message);
		void updateScore(BinaryReader message, CompletionCallback callback);
		void gameCanRestart(bool canRestartFlag);
	}
}
