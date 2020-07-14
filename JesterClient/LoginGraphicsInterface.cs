using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public interface LoginGraphicsInterface
    {
        void loginSuccess(byte[] reconnectKey, LoginInterfaceImpl loginSpace);
        void loginFailed(string reason);
        void loginRedirect(BinaryReader message);
    }
}
