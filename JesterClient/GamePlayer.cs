using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public class GamePlayer
    {
		protected Client client;
		
		protected int spaceId;
		
		protected Achievements achievements = new Achievements();
		
		protected List<Object> gameObjects = new List<Object>();
		
		public GamePlayer(Client client, int spaceId)
		{
			this.client = client;
			this.spaceId = spaceId;
			achievements.loadAchievements(client);
		}
		
		public virtual int getId()
		{
			if (isRobot())
			{
                throw new Exception("This instance is a robot one. The method GamePlayer.getId() must be overrided");
			}
			
			return client.getId();
		}

        public Client getClient()
        {
            return client;
        }

        public int getSpaceId()
        {
            return spaceId;
        }
		
		public virtual string getName()
		{
			return client.getName();
		}
		
		public string getEmail()
		{
			return client != null ? client.getEmail() : "robot";
		}
		
		public virtual bool isRobot()
		{
			return false;
		}
		
		public void addGameObject(object obj)
		{
			gameObjects.Add(obj);
		}

		public object getObject(int index)
		{
			return gameObjects[index];
		}
    }
}