using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class TcpSocket
{
    public Action<string> ClientReceived;

    private Socket m_socket;
    private byte[] m_data;
    private bool m_isServer;

    public TcpSocket(Socket socket, int dataLength, bool isServer)
    {
        m_socket = socket;
        m_data = new byte[dataLength];
        m_isServer = isServer;
    }


    public void ClientReceive()
    {
        if (ClientConnected)
            m_socket.BeginReceive(m_data, 0, m_data.Length, SocketFlags.None, new AsyncCallback(ClientEndReceiver), null);
    }

    public void ClientEndReceiver(IAsyncResult result)
    {
        int recevieLength = m_socket.EndReceive(result);

        string dataStr = System.Text.Encoding.UTF8.GetString(m_data, 0, recevieLength);

        if (ClientReceived != null)
            ClientReceived(dataStr);

    }


    public void ClientSend(byte[] data)
    {
        m_socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(ClientSendEnd), null);
    }

    public void ClientSendEnd(IAsyncResult result)
    {
        m_socket.EndSend(result);
    }

    public void ClientConnect(string ip, int port)
    {
        m_socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), new AsyncCallback(ClientEndConnect), null);
    }

    public void ClientEndConnect(IAsyncResult result)
    {
        if (result.IsCompleted)
        {
            Debug.Log("client connect success");
        }

        m_socket.EndConnect(result);
    }

    public void DisConnect()
    {
        //m_socket.Shutdown();
        m_socket.Disconnect(true);
        m_socket.Close();

    }

    public bool ClientConnected
    {
        get
        {
            return m_socket.Connected && !m_socket.Poll(10, SelectMode.SelectRead);
        }
    }

}
