using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public interface LoginInterface
	{
        void loginSuccess(byte[] reconnectKey);
        void loginFailed(string reason);
		void loginRedirect(BinaryReader message);
		void reconnectSuccess(BinaryReader message);
		void reconnectFailure(BinaryReader message);
		void logoutSuccess(BinaryReader message);
	}

}
