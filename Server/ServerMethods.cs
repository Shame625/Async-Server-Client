using System;
using System.Text;

namespace Server
{
    public static class ServerMethods
    {
        public static byte[] ErrorCodePacket(UInt16 messageNo, UInt16 errorCode)
        {
            byte[] messageB = BitConverter.GetBytes(messageNo);
            byte[] errorC = BitConverter.GetBytes(errorCode);
            byte[] packetLen = BitConverter.GetBytes((UInt16)(sizeof(UInt16) * 3));

            Array.Reverse(messageB);
            Array.Reverse(errorC);
            Array.Reverse(packetLen);

            return new byte[] { messageB[0],
                                messageB[1],
                                packetLen[0],
                                packetLen[1],
                                errorC[0],
                                errorC[1]};
        }



        public static string PrintBytes(this byte[] byteArray)
        {
            var sb = new StringBuilder("new byte[] { ");
            for (var i = 0; i < byteArray.Length; i++)
            {
                var b = byteArray[i];
                sb.Append(b);
                if (i < byteArray.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(" }");
            return sb.ToString();
        }
    }
}
