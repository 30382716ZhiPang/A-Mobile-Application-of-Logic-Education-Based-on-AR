using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class CloudServer : MonoBehaviour
{
    private static CloudServer s_instance;
    
    private static readonly int listenCount = 100;
    private static readonly int dataLength = 1024;

    public string m_ipAddress = "";
    public int portID = 10086;

    public List<TcpSocket> clients;

    private Socket m_server;

    private bool m_isLoopAccept = true;

    public static CloudServer Instance
    {
        get
        {
            return s_instance;
        }
    }


    private void Awake()
    {
        s_instance = this;
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(m_ipAddress))
            return;
        m_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPAddress ip = IPAddress.Parse(m_ipAddress);
        IPEndPoint EP = new IPEndPoint(ip, portID);
        m_server.Bind(EP);

        m_server.Listen(listenCount);

        Thread listenThread = new Thread(ReceiveClientRequest);

        listenThread.Start();

        listenThread.IsBackground = true;

        clients = new List<TcpSocket>();
    }


    private void ReceiveClientRequest()
    {
        while(m_isLoopAccept)
        {
            m_server.BeginAccept(AcceptClient, null);

            Debug.Log("client connectting");

            Thread.Sleep(1000);
        }
    }

    private void AcceptClient(IAsyncResult result)
    {
        Socket client = m_server.EndAccept(result);

        TcpSocket clientSocket = new TcpSocket(client, dataLength, true);
        clientSocket.ClientReceived = OnReceived;

        clients.Add(clientSocket);

        Debug.Log("connect success");

    }

    private void Update()
    {
        if(clients != null && clients.Count > 0)
        {
            for(int i = 0; i < clients.Count; i++)
            {
                clients[i].ClientReceive();
            }
        }
    }

    private void OnReceived(string data)
    {
        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i].ClientConnected)
            {
                clients[i].ClientSend(System.Text.Encoding.UTF8.GetBytes(data));
                Debug.Log("server send message:" + data);
            }
        }
    }

    private void OnApplicationQuit()
    {
        m_isLoopAccept = false;

        if(m_server != null && m_server.Connected)
        {
            m_server.Close();
        }
    }


}
