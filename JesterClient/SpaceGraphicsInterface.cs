using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public interface SpaceGraphicsInterface : ObjectGraphicsInterface
	{
		int getId();
		void info(BinaryReader message);		
        //void putSpace(Space space);
		Space getSpace();
		void initGraphics(BinaryReader message);
        void chatMessage(Channel channel, String message);
        void channelJoin(Channel channel);
		void clientEnter(Client client);
		void clientExit(Client client);
		void childEnter(Space childSpace, Client client);
		void childExit(Space childSpace, Client client);
        void setObjId(int objCode, int id);
        void disconnected(bool forced, string message);
	}
}
