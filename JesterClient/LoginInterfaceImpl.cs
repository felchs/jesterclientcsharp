using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
    public abstract class LoginInterfaceImpl : LoginInterface
    {
        private readonly string userName;

        private readonly string password;
		
		private readonly bool anonymous;

        private readonly ClientSession clientSession;

        public LoginInterfaceImpl(string userName, string password, bool anonymous, ClientSession clientSession)
		{
            this.userName = userName;
            this.password = password;
            this.anonymous = anonymous;
            this.clientSession = clientSession;
		}

        public string getUserName()
        {
            return userName;
        }

        public string getPassword()
        {
            return password;
        }

        public bool isAnynomous()
        {
            return anonymous;
        }

        public ClientSession getClientSession()
        {
            return clientSession;
        }

        public void doLogin()
        {
            Host host = Host.getInstance();
            string host_ = host.getHost();
            int port_ = host.getPort();
            clientSession.Login(userName, password, host_, port_);
        }

        public void doAuthenticate(string user, string pass)
        {
            JesterLogger.log("LoginSpace.doAuthenticate(), user: " + user + ", pass: " + pass);

            MemoryStream m = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(m);
            int idMessageReceiver = -1;
            bw.Write(idMessageReceiver); // loggin space message receiver id == 0

            //continuar daqui ver onde vai colocar o usuario q agora esta conectado 
            byte event_ = LoginProtocol.AUTHENTICATE;
            bw.Write(event_);
            byte len_ = (byte)user.Length;
            JesterLogger.log("LoginSpace.sendEnterLobby(), len_: " + len_);
            bw.Write(len_); // payload
            byte[] userNameUTF = System.Text.Encoding.UTF8.GetBytes(userName);
            JesterLogger.log("LoginSpace.sendEnterLobby(), userNameUTF len_: " + userNameUTF.Length);
            bw.Write(userNameUTF);
            byte anonymous_ = (byte)(anonymous ? 1 : 0);
            bw.Write(anonymous_);

            clientSession.sessionSend(m);
        }

        public void sendEnterLobby(int lobbyId, string user, string pass, bool anonymous)
		{
            /*
            JesterLogger.log("LoginSpace.sendEnterLobby(), lobbyId: " + lobbyId + ", user: " + user + ", pass: " + pass + ", anonymous: " + anonymous);

			MemoryStream m = new MemoryStream();
            //m.SetLength(4 + 1 + 1 + 
            BinaryWriter bw = new BinaryWriter(m);
			int idMessageReceiver = -1;
			bw.Write(idMessageReceiver); // loggin space message receiver id == 0
            byte event_ = LoginProtocol.LOGIN;
			bw.Write(event_);
            byte len_ = (byte)user.Length;
            JesterLogger.log("LoginSpace.sendEnterLobby(), len_: " + len_);
			bw.Write(len_); // payload
            byte[] userNameUTF = System.Text.Encoding.UTF8.GetBytes(userName);
            JesterLogger.log("LoginSpace.sendEnterLobby(), userNameUTF len_: " + userNameUTF.Length);
            bw.Write(userNameUTF);
            byte anonymous_ = (byte)(anonymous ? 1 : 0);
			bw.Write(anonymous_);
            event_ = LoginProtocol.ENTER_LOBBY;
			bw.Write(event_);
			bw.Write(lobbyId);
			clientSession.sessionSend(m);
            */
		}

        public void sendEnterMatchMaker(int matchMakerId = -1)
        {
            JesterLogger.log("LoginSpace.sendEnterLobby(), lobbyId: " + matchMakerId);

            MemoryStream m = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(m);
            int idMessageReceiver = -1; // On Server by default LoginMessageReceiver id == -1
            bw.Write(idMessageReceiver); 
            byte event_ = LoginProtocol.ENTER_MATCH_MAKER;
            bw.Write(event_);
            bw.Write(matchMakerId);
            clientSession.sessionSend(m);
        }


        #region LoginInterface

        public abstract void loginSuccess(byte[] reconnectKey);

        public abstract void loginFailed(string reason);

        public abstract void loginRedirect(BinaryReader message);

        public abstract void reconnectSuccess(BinaryReader message);

        public abstract void reconnectFailure(BinaryReader message);

        public abstract void logoutSuccess(BinaryReader message);

        #endregion
         
    }
}
