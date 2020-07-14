using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public class GenericGameRobot : GamePlayer, RobotInterface
	{
		private int robotIndex;
		
		public GenericGameRobot(int robotIndex, int spaceId) : base (null, spaceId)
		{
			this.robotIndex = robotIndex;
		}
		
		public int getRobotIndex()
		{
			return robotIndex;
		}

        public override int getId()
        {
            return getRobotIndex();
        }
		
		public override string getName()
		{
            return "Computer";
			//return MessagesBundle.getMessage("robot") + " " + robotIndex;
		}

        public override bool isRobot()
        {
            return true;
        }
	}
}
