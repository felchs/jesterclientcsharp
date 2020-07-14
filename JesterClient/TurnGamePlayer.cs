using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public class TurnGamePlayer : GamePlayer
	{
		public int screenPos;

        public TurnGamePlayer(Client client, int spaceId, int screenPos) : base(client, spaceId)
		{
			this.screenPos = screenPos;
		}
	}
}
