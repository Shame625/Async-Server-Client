using System;
using System.Net.Sockets;

namespace Server
{
    class ConnectedClient {
    
        public UInt32 _clientId { get; }
        Socket _socket { get; }
        private string _userName { get; set; }

        public ConnectedClient(UInt32 clientId, Socket socket)
        {
            _clientId = clientId;
            _socket = socket;
        }

        public UInt16 SetUserName(string userName)
        {
            if(userName.Length < Constants.USERNAME_LENGTH_MIN || userName.Length > Constants.USERNAME_LENGTH_MAX)
            {
                return Constants.USERNAME_BAD;
            }

            _userName = userName;
            return Constants.USERNAME_OK;
        }
    }
}
