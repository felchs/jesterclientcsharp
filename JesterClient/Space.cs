using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkstarSharp;

namespace JesterClient
{
    public class Space : EventListener
    {
		protected int id;

        protected Dictionary<string, Channel> channelsMap = new Dictionary<string, Channel>();

		protected Dictionary<int, Client> clientsMap = new Dictionary<int, Client>();
		
        protected EventQueue eventQueue = new EventQueue();

		protected ChatInterface chatInterface;
		
		protected ClientSession clientSession;
		
		protected Space parentSpace;

		protected Client thisClient;
		
		public Space(int id, ClientSession clientSession)
		{
			this.id = id;

			setClientSessionAddThisSpaceAsClientListener(clientSession);

            SpaceMapping.putSpace(this);
		}

        public int getId()
        {
            return id;
        }

        public EventQueue getEventQueue()
        {
            return eventQueue;
        }
		
		public void wait()
		{
			clientSession.wait(getId());
		}
		
		public void release()
		{
			clientSession.release(getId());
		}
		
		public virtual void removeAsEventListener()
		{
			clientSession.removeListener(this);
		}

		public void setChatInterface(ChatInterface chatInterface)
		{
			this.chatInterface = chatInterface;
		}
		
		public ChatInterface getChatInterface()
		{
			return this.chatInterface;
		}

        public Space getParentSpace()
        {
            return parentSpace;
        }

        public void setParentSpace(Space parentSpace)
        {
            this.parentSpace = parentSpace;
        }

		public ClientSession getClientSession()
		{
			return clientSession;
		}

		private void setClientSessionAddThisSpaceAsClientListener(ClientSession clientSession)
		{
			this.clientSession = clientSession;
			clientSession.putEventListener(this);
		}


		public Channel getChatChannel()
		{
            List<string> channelsIdKeys = new List<string>(channelsMap.Keys);
		    foreach (string key in channelsIdKeys)
            {
                Channel channel = channelsMap[key];
                string name = channel.getName();
				if (name.ToLower().Contains("chat")) 
                {
                    return channelsMap[key];
				}
			}
			
			return null;
		}
		
		public Client getClient(int key)
		{
			return clientsMap[key];
		}
		
		public void updateOnThisClientExit()
		{
			removeAsEventListener();

            List<string> channelsIdKeys = new List<string>(channelsMap.Keys);
		    foreach (string key in channelsIdKeys)
            {
				Channel ch = channelsMap[key];
                channelsMap.Remove(ch.getName());
				ChatInterface chatInterface = getChatInterface();
				if (chatInterface != null) 
                {
					chatInterface.onChannelLeave(ch);
				}
			}
		}

		// Input events -------------------------------------------------------
		
		public void channelJoin(Channel channel)
		{
            JesterLogger.log("Channel join: " + channel.getName());
            channelsMap.Add(channel.getName(), channel);
			
			// chat interface
			if (chatInterface != null && channel.getName().ToLower().Contains("chat")) 
            {
				channel.putMessageInterface(chatInterface);
				chatInterface.onJoinChannel(channel);
			}
		}

        public void disconnected(bool forced, string message)
        {
            List<object> objects = new List<object>();
            objects.Add(message);
			// TODO: do disconnection
			// eventQueue.addFunction(new Event(SpaceProtocol.DISCONNECTED, objects));
        }
		
		public void info(BinaryReader message)
		{
            List<object> objects = new List<object>();
            objects.Add(message);
            eventQueue.addFunction(new Event(SpaceProtocol.INFO, objects));
		}
		
		protected Client onExit(BinaryReader message)
		{
			int clientId = message.ReadInt16();
            Client client = clientsMap[clientId];

			// maybe client moved from this space
			if (client != null) 
            {
				updateOnClientExit(client);					
				clientsMap.Remove(clientId);
			}
			
			if (isClientThisClient(client)) 
            {
				updateOnThisClientExit();
			}

			return client;
		}
		
		public void onChildEnter(Space childSpace, Client client)
		{
            List<object> objects = new List<object>();
            objects.Add(childSpace);
            objects.Add(client);
            eventQueue.addFunction(new Event(SpaceProtocol.CHILD_ENTER, objects));
		}
		
		public void onChildExit(Space childSpace, Client client)
		{
            List<object> objects = new List<object>();
            objects.Add(childSpace);
            objects.Add(client);
            eventQueue.addFunction(new Event(SpaceProtocol.CHILD_EXIT, objects));
		}
		
		protected void updateConnectedClients(BinaryReader message)
		{
			int numClients = message.ReadInt16();
			JesterLogger.log("Update connected clients, numclients: " + numClients);
            for (int i = 0; i < numClients; i++)
            {
				updateSingleClient(message);
			}
		}
		
		public bool isClientThisClient(Client client)
		{
			return thisClient == client;
		}
		
		private Client updateSingleClient(BinaryReader message)
		{
            int clientId = message.ReadInt16();
            JesterLogger.log("Update connected clients, clientId: " + clientId);
			if (thisClient == null || thisClient.getId() != clientId) 
            {
                int count = message.ReadInt16();
                JesterLogger.log("count bytes: " + count);
                byte[] strBytes = message.ReadBytes(count);
                Encoding enc = new UTF8Encoding(true, true);
                string clientServerObjectName = enc.GetString(strBytes);
                JesterLogger.log("Update connected clients, len: " + clientServerObjectName.Length + ", name&email: " + clientServerObjectName);
				int clientStatus = message.ReadByte();
                JesterLogger.log("_Update connected clients, status: " + clientStatus);
       			//string[] nameemail = clientServerObjectName.Split('&');
                //string name = nameemail[0];
                //string email = nameemail[1];
                string name = clientServerObjectName;
                string email = clientServerObjectName;
                JesterLogger.log("updateOnClientEnter 0");
				Client client = new Client(clientId, name, email, clientStatus);
				clientsMap.Add(clientId, client);
                JesterLogger.log("updateOnClientEnter 1");
				updateOnClientEnter(client, message);
                JesterLogger.log("updateOnClientEnter 2");
				return client;
			} 
            else 
            {
				return null;
			}
		}
		
		protected virtual void updateOnClientEnter(Client client, BinaryReader additionalInitInfo)
		{
            JesterLogger.log("updateoncliententer, space, id: " + id);
			if (thisClient == null) 
            {
				thisClient = client;
				if (chatInterface != null) 
                {
					chatInterface.setClient(thisClient);
				}
			}
			
			if (parentSpace != null) 
            {
				parentSpace.onChildEnter(this, client);
			}

            List<object> objects = new List<object>();
            objects.Add(client);
            eventQueue.addFunction(new Event(SpaceProtocol.ENTER, objects));
		}
		
		protected virtual void updateOnClientExit(Client client)
		{
			if (parentSpace != null) 
            {
				parentSpace.onChildExit(this, client);
			}

            List<object> objects = new List<object>();
            objects.Add(client);
            eventQueue.addFunction(new Event(SpaceProtocol.EXIT, objects));
		}
		
		public void setObjId(int objCode, int id)
		{
            List<object> objects = new List<object>();
            objects.Add(objCode);
            objects.Add(id);
            eventQueue.addFunction(new Event(SpaceProtocol.SET_OBJ_ID, objects));
		}

		public void setObjsIds(int[] objCodes, int[] ids)
		{
            int len = objCodes.Length;
			for (int i = 0; i < len; i++) 
            {
				int objCode = objCodes[i];
				int id = ids[i];
				setObjId(objCode, id);
			}
		}

		public virtual bool callFunction(int fnc, BinaryReader message, CompletionCallback callback = null)
		{
			switch (fnc) 
            {
                case SpaceProtocol.INIT:
                    List<Object> objects = new List<Object>();
                    objects.Add(message);
                    eventQueue.addFunction(new Event(SpaceProtocol.INIT, objects));
                    return true;
				
				case SpaceProtocol.ENTER:
					updateSingleClient(message);
                    return true;
				
				case SpaceProtocol.EXIT:
					onExit(message);
                    return true;
				
				case SpaceProtocol.INFO:
					info(message);
                    objects = new List<Object>();
                    objects.Add(message);
                    eventQueue.addFunction(new Event(SpaceProtocol.INFO, objects));
                    return true;
				
				case SpaceProtocol.UPDATE_CONNECTED_CLIENTS:
					updateConnectedClients(message);
                    return true;
				
				case SpaceProtocol.BYTE_ENABLE:
                    int objectId = message.ReadByte();
                    bool enabled_ = message.ReadBoolean();
                    objects = new List<Object>();
                    objects.Add(objectId);
                    objects.Add(enabled_);
                    eventQueue.addFunction(new Event(SpaceProtocol.ENABLE, objects));
                    return true;
				
				case SpaceProtocol.BYTE_ENABLES:
					int len = message.ReadByte();
					for (int i = 0; i < len; i++) 
                    {
						objectId = message.ReadByte();
						enabled_ = message.ReadBoolean();
                        objects = new List<Object>();
                        objects.Add(objectId);
                        objects.Add(enabled_);
                        eventQueue.addFunction(new Event(SpaceProtocol.ENABLE, objects));
					}
                    return true;
				
				case SpaceProtocol.SHORT_ENABLE:
					objectId = message.ReadInt16();
					enabled_ = message.ReadBoolean();
                    objects = new List<Object>();
                    objects.Add(objectId);
                    objects.Add(enabled_);
                    eventQueue.addFunction(new Event(SpaceProtocol.ENABLE, objects));
                    return true;
				
				case SpaceProtocol.SHORT_ENABLES:
					enabled_ = message.ReadBoolean();
					len = message.ReadInt16();
					for (int i = 0; i < len; i++) 
                    {
						objectId = message.ReadInt16();
                        objects = new List<Object>();
                        objects.Add(objectId);
                        objects.Add(enabled_);
                        eventQueue.addFunction(new Event(SpaceProtocol.ENABLE, objects));
					}
                    return true;
				
				case SpaceProtocol.BYTE_VISIBLE:
					objectId = message.ReadByte();
					bool visible_ = message.ReadBoolean();
                    objects = new List<Object>();
                    objects.Add(objectId);
                    objects.Add(visible_);
                    eventQueue.addFunction(new Event(SpaceProtocol.VISIBLE, objects));
                    return true;
				
				case SpaceProtocol.BYTE_VISIBLES:
					visible_ = message.ReadBoolean();
					len = message.ReadByte();
					for (int i = 0; i < len; i++) 
                    {
						objectId = message.ReadByte();
                        objects = new List<Object>();
                        objects.Add(objectId);
                        objects.Add(visible_);
                        eventQueue.addFunction(new Event(SpaceProtocol.VISIBLE, objects));
					}
                    return true;
				
				case SpaceProtocol.SHORT_VISIBLE:
					objectId = message.ReadInt16();
					visible_ = message.ReadBoolean();
                    objects = new List<Object>();
                    objects.Add(objectId);
                    objects.Add(visible_);
                    eventQueue.addFunction(new Event(SpaceProtocol.VISIBLE, objects));
                    return true;
				
				case SpaceProtocol.SHORT_VISIBLES:
					visible_ = message.ReadBoolean();
					len = message.ReadInt16();
					for (int i = 0; i < len; i++) 
                    {
						objectId = message.ReadInt16();
                        objects = new List<Object>();
                        objects.Add(objectId);
                        objects.Add(visible_);
                        eventQueue.addFunction(new Event(SpaceProtocol.VISIBLE, objects));
					}
                    return true;

				case SpaceProtocol.BYTE_SELECTION:
					objectId = message.ReadInt16();
					bool selected_ = message.ReadBoolean();
                    objects = new List<Object>();
                    objects.Add(objectId);
                    objects.Add(selected_);
                    eventQueue.addFunction(new Event(SpaceProtocol.SELECTION, objects));
                    return true;

				case SpaceProtocol.BYTE_SELECTIONS:
					selected_ = message.ReadBoolean();
					len = message.ReadByte();
					for (int i = 0; i < len; i++) 
                    {
						objectId = message.ReadByte();
                        objects = new List<Object>();
                        objects.Add(objectId);
                        objects.Add(selected_);
                        eventQueue.addFunction(new Event(SpaceProtocol.SELECTION, objects));
					}
                    return true;
				
				case SpaceProtocol.SHORT_SELECTION:
					objectId = message.ReadInt16();
					selected_ = message.ReadBoolean();
                    objects = new List<Object>();
                    objects.Add(objectId);
                    objects.Add(selected_);
                    eventQueue.addFunction(new Event(SpaceProtocol.SELECTION, objects));
                    return true;

				case SpaceProtocol.SHORT_SELECTIONS:
					selected_ = message.ReadBoolean();
					len = message.ReadInt16();
					for (int i = 0; i < len; i++) 
                    {
						objectId = message.ReadInt16();
                        objects = new List<Object>();
                        objects.Add(objectId);
                        objects.Add(selected_);
                        eventQueue.addFunction(new Event(SpaceProtocol.SELECTION, objects));
					}
                    return true;
				
				case SpaceProtocol.BYTE_SELECTABLE:
					objectId = message.ReadByte();
					selected_ = message.ReadBoolean();
                    objects = new List<Object>();
                    objects.Add(objectId);
                    objects.Add(selected_);
                    eventQueue.addFunction(new Event(SpaceProtocol.SELECTABLE, objects));
                    return true;
				
				case SpaceProtocol.BYTE_SELECTABLES:
					selected_ = message.ReadBoolean();
					len = message.ReadByte();
					for (int i = 0; i < len; i++) 
                    {
						objectId = message.ReadByte();
                        objects = new List<Object>();
                        objects.Add(objectId);
                        objects.Add(selected_);
                        eventQueue.addFunction(new Event(SpaceProtocol.SELECTABLE, objects));
					}
                    return true;

				case SpaceProtocol.SHORT_SELECTABLE:
					objectId = message.ReadInt16();
					selected_ = message.ReadBoolean();
                    objects = new List<Object>();
                    objects.Add(objectId);
                    objects.Add(selected_);
                    eventQueue.addFunction(new Event(SpaceProtocol.SELECTABLE, objects));
                    return true;
				
				case SpaceProtocol.SHORT_SELECTABLES:
					selected_ = message.ReadBoolean();
					len = message.ReadInt16();
					for (int i = 0; i < len; i++) 
                    {
						objectId = message.ReadInt16();
                        objects = new List<Object>();
                        objects.Add(objectId);
                        objects.Add(selected_);
                        eventQueue.addFunction(new Event(SpaceProtocol.SELECTABLE, objects));
					}
                    return true;
				
			case SpaceProtocol.SET_BYTE_OBJ_BYTE_ID:
				int objCode = message.ReadByte();
				int id = message.ReadByte();
				setObjId(objCode, id);
                return true;
			
			case SpaceProtocol.SET_BYTE_OBJS_BYTE_IDS:
				int numObjs = message.ReadByte();
   				int[] objCodes = new int[numObjs];
                int[] ids = new int[numObjs];
				for (int i = 0; i < numObjs; i++)
                {
					objCode = message.ReadByte();
                    objCodes[i] = objCode;
				}
				for (int i = 0; i < numObjs; i++) 
                {
					int objId = message.ReadByte();
                    ids[i] = objId;
				}
				setObjsIds(objCodes, ids);
                return true;

			case SpaceProtocol.SET_SHORT_OBJ_BYTE_ID:
				objCode = message.ReadInt16();
				id = message.ReadByte();
				setObjId(objCode, id);
                return true;
			
			case SpaceProtocol.SET_SHORT_OBJS_BYTE_IDS:
				numObjs = message.ReadInt16();
				objCodes = new int[numObjs];
                ids = new int[numObjs];
				for (int i = 0; i < numObjs; i++)
                {
					objCode = message.ReadInt16();
                    objCodes[i] = objCode;
				}
				for (int i = 0; i < numObjs; i++)
                {
					int objId = message.ReadByte();
                    ids[i] = objId;
				}
				setObjsIds(objCodes, ids);
                return true;
			
			case SpaceProtocol.SET_SHORT_OBJ_SHORT_ID:
				objCode = message.ReadInt16();
				id = message.ReadInt16();
				setObjId(objCode, id);
                return true;
			
			case SpaceProtocol.SET_SHORT_OBJS_SHORT_IDS:
				numObjs = message.ReadInt16();
				objCodes = new int[numObjs];
                ids = new int[numObjs];
				for (int i = 0; i < numObjs; i++) 
                {
					objCode = message.ReadInt16();
                    objCodes[i] = objCode;
				}
				for (int i = 0; i < numObjs; i++) 
                {
					int objId = message.ReadInt16();
                    ids[i] = objId;
                }
				setObjsIds(objCodes, ids);
                return true;
			}

            return false;
		}
		
        //
		// Output events
        //

		public void channelSend(string channelName, MemoryStream message)
		{
            byte[] bytesMessage = null;
            using (var ms = new MemoryStream())
            {
                //message.CopyTo(ms);
                bytesMessage = ms.ToArray();
            }

            Channel channel = channelsMap[channelName];
			channel.channelSend(bytesMessage);
		}
		
		public void sessionSend(MemoryStream message)
		{
			clientSession.sessionSend(message);
		}
		
		public void sendEnterSpace()
		{
            MemoryStream m = new MemoryStream();
            m.SetLength(4 + 1);
            BinaryWriter bw = new BinaryWriter(m);
            int id = getId();
            bw.Write(Converter.GetBigEndian(id));
            byte event_ = SpaceProtocol.ENTER;
			bw.Write(event_);
			clientSession.sessionSend(m);
		}
		
		public void sendExitSpace()
		{
            MemoryStream m = new MemoryStream();
            m.SetLength(4 + 1);
            BinaryWriter bw = new BinaryWriter(m);
            int id = getId();
            bw.Write(Converter.GetBigEndian(id));
            byte event_ = SpaceProtocol.EXIT;
			bw.Write(event_);
			clientSession.sessionSend(m);
		}
	}
}