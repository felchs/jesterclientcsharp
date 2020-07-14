using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public class Client
    {
		protected int id;
		
		protected string name;
		
		protected string email;
		
		protected int status;
		
		protected Dictionary<int, GamePlayer> playersMap = new Dictionary<int, GamePlayer>();
		
		public Client(int id, string name, string email, int status)
		{
			this.id = id;
			this.name = name;
            this.email = email;
			this.status = status;
		}

        public int getId()
        {
            return id;
        }

        public string getName()
        {
            return name;
        }

        public string getEmail()
        {
            return email;
        }

        public int getStatus()
        {
            return status;
        }
		
		public void addPlayer(GamePlayer player)
		{
			playersMap.Add(player.getSpaceId(), player);
		}
		
		public void removePlayer(GamePlayer player)
		{
			playersMap.Remove(player.getSpaceId());
		}
		
		public GamePlayer getPlayer(int spaceId)
		{
			return playersMap[spaceId];
		}
	}
}
