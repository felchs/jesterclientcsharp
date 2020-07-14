using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public interface TurnGameGraphicsInterface
	{
		void notifyCurrentPlayer(Client client, CompletionCallback callback);
		void notifyCurrentPlayerRobot(RobotInterface robot, CompletionCallback callback);
		void turnStarted(BinaryReader message, CompletionCallback callback);
        void turnFinished(BinaryReader message, CompletionCallback callback);
        void matchStarted(BinaryReader message, CompletionCallback callback);
        void matchFinished(BinaryReader message, CompletionCallback callback);
		void setSelectablesByPlayer(GamePlayer player, bool selectable);
        void onAfterPlay(object playInfo = null);
	}
}
