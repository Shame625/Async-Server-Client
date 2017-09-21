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

            Array.Reverse(messageB);
            Array.Reverse(errorC);
            

            byte[] data = new byte[] { messageB[1],
                                messageB[0],
                                0x00,
                                0x00,
                                errorC[1],
                                errorC[0]};

            CalculateLen(ref data);
            return data;
        }

        public static void CalculateLen(ref byte[] data)
        {
            byte[] packetLen = BitConverter.GetBytes((UInt16)(data.Length));
            Array.Reverse(packetLen);

            data[2] = packetLen[0];
            data[2] = packetLen[1];
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
