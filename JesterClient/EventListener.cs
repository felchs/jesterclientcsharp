using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public interface EventListener 
    {
        int getId();
        ClientSession getClientSession();
        bool callFunction(int fnc, BinaryReader msg, CompletionCallback callback = null);
        void channelJoin(Channel channel);
        void disconnected(bool forced, string message);
    }
}
