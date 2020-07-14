using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
   	public class Host
	{
		private static Host instance;

		public static Host getInstance()
		{
			if (instance == null) 
            {
				instance = new Host();
			}
			return instance;
		}

        ///////////////////////////////////////////////////////////////////////
		
		private String host = "127.0.0.1";
		private int port = 8007;
		
		private Host()
		{
		}

		public void setHost(String host)
		{
			this.host = host;
		}

		public string getHost()
		{
			return host;
		}
		
		public int getPort()
		{
			return port;
		}
		
		public void setPort(int port)
		{
			this.port = port;
		}
	}
}