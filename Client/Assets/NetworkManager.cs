using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public sealed class NetworkManager : MonoBehaviour
{
    private static readonly NetworkManager instance = new NetworkManager();
    private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private byte[] _buffer;

    public bool LoopConnect()
    {

        var endPoint = new IPEndPoint(IPAddress.Loopback, 100);
        _clientSocket.BeginConnect(endPoint, ConnectCallback, null);

        return true;
    }

    private void ConnectCallback(IAsyncResult AR)
    {
        try
        {
            _clientSocket.EndConnect(AR);
            _buffer = new byte[1024];
            _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }
        catch (SocketException ex)
        {
            Debug.Log(ex.Message);
        }
        catch (ObjectDisposedException ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void SendPacket(byte[] data)
    {
        _clientSocket.Send(data);
        _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), this);
    }

    public static NetworkManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void ReceiveCallback(IAsyncResult AR)
    {
        try
        {
            int received = _clientSocket.EndReceive(AR);

            if (received == 0)
            {
                return;
            }

            byte[] dataBuff = new byte[received];
            Array.Copy(_buffer, dataBuff, received);

            Debug.Log(PrintBytes(dataBuff));

            _clientSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }
        catch (SocketException ex)
        {
            Debug.Log(ex.Message);
        }
        catch (ObjectDisposedException ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private string PrintBytes(byte[] byteArray)
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