using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public class MessagesBundle
	{
		private Locale locale = Locale.PORTUGUESE;
		
		private List<object> xmlList = new List<object>();
		
		public static void addXMLToList(string path)
		{
			//MessagesBundle.xmlList.push(xml);
		}
		
		public static void checkInitialize()
		{
		}

		public static string getMessage(string key_)
		{
			checkInitialize();

            int count = 0;// xmlList.Count;
			for (int i = 0; i < count; i++)
            {
				//for each (var message:XML in messageList[0].message) 
                //{
                    string message = null;
					if (message.Equals(key_)) 
                    {
                        string retStr = message;
						if (retStr.Length == 0) 
                        {
							throw new Exception("Error: key: " + key_ + " is empty. Check the messagesBundle.xml for more details.");
						}
						return message;
					}
				//}
			}
			
			throw new Exception("Error: key: " + key_ + " does not exit. Check the messagesBundle.xml for more details.");
		}
	}
}
