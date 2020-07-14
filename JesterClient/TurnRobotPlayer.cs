using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public class TurnRobotPlayer : TurnGamePlayer, RobotInterface
    {
        private int robotIndex;

        public TurnRobotPlayer(int robotIndex, int spaceId, int screenPos) : base(null, spaceId, screenPos)
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
		
        public override bool isRobot()
		{
			return true;
		}

        public override string getName()
        {
            return "Robot: " + getId();
        }
    }
}
