using System;
using System.IO;
using System.Collections.Generic;
using DarkstarSharp;

namespace JesterClient
{
    public class ClientSession : SimpleClientListener
    {
        private bool loggedIn;

        private string userName = null;

        private string password = null;

        private SimpleClient client = null;

        private LoginInterface login;

        private Dictionary<int, EventListener> eventListenersMap;

        public ClientSession(LoginInterface login)
        {
            this.login = login;
            this.eventListenersMap = new Dictionary<int, EventListener>();
        }

        public void Login(string userName, string password, string host, int port)
        {
            this.userName = userName;
            this.password = password;
            client = new SimpleClient(this);
            JesterLogger.log("EventsHandler.Login(), userName: " + userName + ", host: " + host + ", port: " + port);
            client.login(host, port);
        }

        public bool isLoggedIn()
        {
            return loggedIn;
        }

        public void wait(int id)
        {
        }

        public void release(int id)
        {
        }

        public void sessionSend(MemoryStream message)
		{
            byte[] bytesMessage = null;
            //byte[] buffer = new byte[1024];
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    int read;
            //    while ((read = message.Read(buffer, 0, buffer.Length)) > 0)
            //    {
            //        ms.Write(buffer, 0, read);
            //    }
            //    bytesMessage = ms.ToArray();
            //}
            bytesMessage = message.ToArray();
            JesterLogger.log("EventsHandler.sessionSend(), Count: " + bytesMessage.Length);
            client.SessionSend(bytesMessage);
		}

		public void putEventListener(EventListener eventListener)
		{
            JesterLogger.log("EventsHandler.putEventListener(), eventListener: " + eventListener + ",,id: "+ eventListener.getId());
			int id = eventListener.getId();
            if (!eventListenersMap.ContainsKey(id))
            {
                eventListenersMap.Add(id, eventListener);
            }
            else
            {
                JesterLogger.log("EventsHandler.putEventListener() WARNING, adding event as listener again!");
            }
		}
		
		public void removeListener(EventListener eventListener)
		{
            int id = eventListener.getId();
            eventListenersMap.Remove(id);
		}
		
		public EventListener getEventListener(int id)
		{
            return eventListenersMap[id];
		}
        
        #region SimpleClientListener

        public void LoggedIn(byte[] reconnectKey)
        {
            loggedIn = true;
            JesterLogger.log("EventsHandler.LoggedIn()");

            login.loginSuccess(reconnectKey);

            //List<int> keys = new List<int>(eventListenersMap.Keys);
            //foreach (int key in keys)
            //{
            //    EventListener eventListener = eventListenersMap[key];
            //    eventListener.loginSuccess(reconnectKey);
            //}
        }

        public void LoginFailed(string reason)
        {
            loggedIn = false;
            JesterLogger.log("EventsHandler.loginFailed, reason: " + reason);

            login.loginFailed(reason);
            //List<int> keys = new List<int>(eventListenersMap.Keys);
            //foreach (int key in keys)
            //{
            //    EventListener eventListener = eventListenersMap[key];
            //    eventListener.loginFailed(reason);
            //}
        }

        public void ReceivedMessage(byte[] message)
        {
            JesterLogger.log("EventsHandler.ReceivedMessage_, message: " + message.Length);
            MemoryStream m = new MemoryStream();
            m.SetLength(message.Length);
            
            BinaryWriter bw = new BinaryWriter(m);
            bw.Write(message);
            bw.Flush();
            m.Position = 0;
            
            JavaBinaryReader br = new JavaBinaryReader(m);
            int id = br.ReadInt32();
            int fnc = br.ReadByte();
            JesterLogger.log("EventsHandler.ReceivedMessage, fnc: " + fnc + ", id: " + id);

            EventListener eventListener = eventListenersMap[id];
            JesterLogger.log("EventsHandler.ReceivedMessage, eventListener: " + eventListener.getId());
            eventListener.callFunction(fnc, br);
        }

        public void Disconnected(bool forced, string message)
        {
            JesterLogger.log("EventsHandler.Disconnected, forced: " + forced + ", message: " + message);
            loggedIn = false;

            List<int> keys = new List<int>(eventListenersMap.Keys);
            foreach (int key in keys)
            {
                EventListener eventListener = eventListenersMap[key];
                eventListener.disconnected(forced, message);
            }
        }

        public PasswordAuthentication GetPasswordAuthentication()
        {
            return new PasswordAuthentication(userName, password);
        }

        public ClientChannelListener JoinedChannel(ClientChannel clientChannel)
        {
            JesterLogger.log("EventsHandler.JoinedChannel, channelName: " + clientChannel.Name);
            Channel channel = new Channel(clientChannel);

            List<int> keys = new List<int>(eventListenersMap.Keys);
            foreach (int key in keys)
            {
                EventListener eventListener = eventListenersMap[key];
                eventListener.channelJoin(channel);
            }
            return channel;
        }

        #endregion
    }
}