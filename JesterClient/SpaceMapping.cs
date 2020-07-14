using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public class SpaceMapping
    {
        private static SpaceMapping instance;

        public static SpaceMapping getInstance()
        {
            if (instance == null)
            {
                instance = new SpaceMapping();
            }
            return instance;
        }

        public static JesterClient.Space getSpace(int id)
        {
            return getInstance().spaceMapping[id];
        }

        public static void putSpace(JesterClient.Space space)
        {
            JesterLogger.log("SpaceMapping.putSpace: " + space.getId());
            getInstance().spaceMapping.Add(space.getId(), space);
            getInstance().spaceStack.Push(space);
        }

        public static void removeSpace(int id)
        {
            JesterLogger.log("SpaceMapping.removeSpace: " + id);
            getInstance().spaceMapping.Remove(id);
            getInstance().spaceStack.Pop();
        }

        public static Space getLastSpaceIn()
        {
            return getInstance().spaceStack.Peek();
        }

        public static Space getPenultimateSpaceIn()
        {
            Space[] arr = getInstance().spaceStack.ToArray();
            return arr[arr.Length - 2];
        }

        ///////////////////////////////////////////////////////////////////////

        private Dictionary<int, JesterClient.Space> spaceMapping = new Dictionary<int, JesterClient.Space>();

        private Stack<JesterClient.Space> spaceStack = new Stack<JesterClient.Space>();

        private SpaceMapping()
        {
        }
    }
}
