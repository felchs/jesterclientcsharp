using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public class ActionGameSpace : GameSpace
    {
        protected Dictionary<int, Client> playersLocation = new Dictionary<int, Client>();

        public ActionGameSpace(int id, ClientSession clientSession) : base(id, clientSession)
        {
        }

        public float readFloat(BinaryReader reader) 
        {
            byte[] floatBytes = reader.ReadBytes(4);
            Array.Reverse(floatBytes);
            float value = BitConverter.ToSingle(floatBytes, 0);
            return value;
        }

        public override bool callFunction(int fnc, BinaryReader message, CompletionCallback callback = null)
        {
            bool ret = base.callFunction(fnc, message, callback);
            JesterLogger.log("ActionGameSpace.callFunction, ret: " + ret + ", fnc: " + fnc);

            if (ret)
            {
                return true;
            }

            switch (fnc)
            {
                case ActionGameProtocol.SNAPSHOT:
                    JesterLogger.log("ActionGameProtocol.Snapshot");
                    onSnapshot(message);
                    return true;

                case ActionGameProtocol.ACTION:
                     JesterLogger.log("ActionGameProtocol.Action");
                     onAction(message);
                    return true;

                case ActionGameProtocol.MOVE:
                    JesterLogger.log("ActionGameProtocol.Move");
                    onMove(message);
                    return true;
            }

            return false;
        }

        public virtual void onSnapshot(BinaryReader message)
        {
            List<Object> objects = new List<Object>();
            objects.Add(message);
            int numPlayers = message.ReadInt16();
            for (int i = 0; i < numPlayers; i++)
            {
                int clientId = message.ReadInt16();
                JesterLogger.log("clientId: " + clientId);
                ActionBasedGameLocation location = new ActionBasedGameLocation();
                location.clientId = clientId;
                location.posX = readFloat(message);
                location.posY = readFloat(message);
                location.posZ = readFloat(message);
                location.rotX = readFloat(message);
                location.rotY = readFloat(message);
                location.rotZ = readFloat(message);
                objects.Add(location);
            }
            Event event_ = new Event(ActionGameProtocol.SNAPSHOT, objects);
            eventQueue.addFunction(event_);
        }

        public virtual void onAction(BinaryReader message)
        {
            List<Object> objects = new List<Object>();
            objects.Add(message);
            eventQueue.addFunction(new Event(ActionGameProtocol.ACTION, objects));
        }

        public virtual void onMove(BinaryReader message)
        {
            List<Object> objects = new List<Object>();
            objects.Add(message);
            eventQueue.addFunction(new Event(ActionGameProtocol.MOVE, objects));
        }
    }
}