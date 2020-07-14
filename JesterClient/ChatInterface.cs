using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public interface ChatInterface : MessageInterface
	{
		void setClient(Client client);
		void onJoinChannel(Channel channel);
		void onChannelLeave(Channel channel);
		void minimize();
		void maximize();
		void setVisible(bool state);
        void clear();
	}
}
