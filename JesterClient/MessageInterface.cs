using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public interface MessageInterface
	{
		int getId();
        void onMessage(Channel channel, string message);
        void leftChannel(Channel channel);
	}
}
