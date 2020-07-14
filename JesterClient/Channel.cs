using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkstarSharp;

namespace JesterClient
{
    public class Channel : ClientChannelListener
    {
        private ClientChannel clientChannel;

        private Dictionary<int, MessageInterface> channelListenersMap = new Dictionary<int, MessageInterface>();

        public Channel(ClientChannel clientChannel)
        {
            this.clientChannel = clientChannel;
        }

        public string getName()
        {
            return clientChannel.Name;
        }

        public ClientChannel getClientChannel()
        {
            return clientChannel;
        }

        public void send(string message)
        {
            clientChannel.Send(Encoding.UTF8.GetBytes(message));
        }

        public void channelSend(byte[] message)
        {
            clientChannel.Send(message);
        }

        public void putMessageInterface(MessageInterface msgInterface)
		{
			channelListenersMap.Add(msgInterface.getId(), msgInterface);
		}

		public void removeMessageInterface(MessageInterface msgInterface)
		{
            channelListenersMap.Remove(msgInterface.getId());
		}

        //
        // ClientChannelListener
        //

        public void ReceivedMessage(ClientChannel channel, byte[] message)
        {
            string msgString = Encoding.UTF8.GetString(message);
            List<int> keys = new List<int>(channelListenersMap.Keys);
            foreach (int key in keys)
            {
                channelListenersMap[key].onMessage(this, msgString);
            }
        }

        public void LeftChannel(ClientChannel channel)
        {
            List<int> keys = new List<int>(channelListenersMap.Keys);
            foreach (int key in keys)
            {
                channelListenersMap[key].leftChannel(this);
            }
        }
    }
}