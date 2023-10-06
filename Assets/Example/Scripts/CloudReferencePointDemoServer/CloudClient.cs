using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Text;

public class CloudClient : MonoBehaviour
{
    public Text ShowText;
    private static readonly int dataLength = 1024;

    public string InputStr { get; set; }

    public string ReceiveStr { get; set; }

    public string IPAddress { get; set; }

    public int PortID { get; set; }

    private TcpSocket tcpClient;

    private bool isConnected;
    StringBuilder builder;

    string message;


    private void Awake()
    {
        IPAddress = "10.86.34.119";
        PortID = 10086;
        builder = new StringBuilder();
    }

    private void OnEnable()
    {
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        tcpClient = new TcpSocket(client, dataLength, false);

        tcpClient.ClientReceived += OnReceived;

        ConnectServer();

    }
    // Use this for initialization
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        if(tcpClient != null && tcpClient.ClientConnected)
        {
            tcpClient.ClientReceive();
        }
        else
        {
            ConnectServer();
        }

        builder.Clear();
        builder.Append("connected:").Append(tcpClient.ClientConnected).Append("\n")
            .Append("message:").Append(message);
        ShowText.text = builder.ToString();
    }

    public void ConnectServer()
    {
        if(tcpClient != null)
        {
            tcpClient.ClientConnect(IPAddress, PortID);
        }
    }

    void OnReceived(string mes)
    {
        Debug.Log(mes);
        message = mes;
    }

    public void SendToServer(string m)
    {
        if(tcpClient != null && tcpClient.ClientConnected && !string.IsNullOrEmpty(m))
        {
            Debug.Log("message:" + m);
            tcpClient.ClientSend(System.Text.Encoding.UTF8.GetBytes(m));
        }
    }

    private void OnDisable()
    {
        if(tcpClient.ClientConnected)
            tcpClient.DisConnect();
    }

    private void OnApplicationQuit()
    {
        if (tcpClient.ClientConnected)
            tcpClient.DisConnect();
    }

}
