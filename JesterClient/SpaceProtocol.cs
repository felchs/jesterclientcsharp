using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public class SpaceProtocol 
	{
        public const int CHILD_ENTER = -8;
        public const int CHILD_EXIT = -9;

		public const int ENTER = 1;
		public const int EXIT = 2;
		public const int INIT = 3;
		public const int INFO = 4;
		public const int UPDATE_CONNECTED_CLIENTS = 5;

        public const int ENABLE = 6;
		public const int BYTE_ENABLE = 6;
		public const int BYTE_ENABLES = 7;
		public const int SHORT_ENABLE = 8;
		public const int SHORT_ENABLES = 9;

        public const int VISIBLE = 10;
		public const int BYTE_VISIBLE = 10;
		public const int BYTE_VISIBLES = 11;
		public const int SHORT_VISIBLE = 12;
		public const int SHORT_VISIBLES = 13;

        public const int SELECTION = 14;
		public const int BYTE_SELECTION = 14;
		public const int BYTE_SELECTIONS = 15;
		public const int SHORT_SELECTION = 16;
		public const int SHORT_SELECTIONS = 17;

        public const int SELECTABLE = 15;
		public const int BYTE_SELECTABLE = 18;
		public const int BYTE_SELECTABLES = 19;
		public const int SHORT_SELECTABLE = 20;
		public const int SHORT_SELECTABLES = 21;

        public const int SET_OBJ_ID = 22;
		public const int SET_BYTE_OBJ_BYTE_ID = 22;
		public const int SET_BYTE_OBJS_BYTE_IDS = 23;
		public const int SET_SHORT_OBJ_BYTE_ID = 24;
		public const int SET_SHORT_OBJS_BYTE_IDS = 25;
		public const int SET_SHORT_OBJ_SHORT_ID = 26;
		public const int SET_SHORT_OBJS_SHORT_IDS = 27;
	}
}
