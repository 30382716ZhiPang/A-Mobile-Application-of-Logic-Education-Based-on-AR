using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARStand;
using UnityEngine.EventSystems;
using System.Text;
using System.Net.Sockets;

public class CloudReferencePointCreaeteNetManager : MonoBehaviour
{
    private static readonly string IDS = "NetCloudIDs";
    private static readonly int dataLength = 1024;

    [HideInInspector]
    public List<string> cloudAnchorIds;

    private List<string> m_resolveWattingList;

    private CloudClient m_cloudClient;

    [SerializeField]
    private Button loadButton;

    [SerializeField]
    private Button clearButton;

    [SerializeField]
    private Text StateText;

    [SerializeField]
    private Text ErrorText;

    [SerializeField]
    private string IPAddress;

    [SerializeField]
    private int PortID;

    private ARSessionOrigin m_origin;
    private ARReferencePointManager m_anchorManager;
    private ARRaycastManager m_raycastManager;
    private ARCloudReferencePointManager m_cloudManager;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    private List<ARCloudReferencePoint> m_cloudAnchors = new List<ARCloudReferencePoint>();

    private TcpSocket tcpClient;

    private Coroutine loadCoroutine;


    private void Awake()
    {
        cloudAnchorIds = new List<string>();
        m_resolveWattingList = new List<string>();
        m_origin = GetComponent<ARSessionOrigin>();
        m_anchorManager = GetComponent<ARReferencePointManager>();
        m_raycastManager = GetComponent<ARRaycastManager>();
        m_cloudManager = GetComponent<ARCloudReferencePointManager>();
        loadCoroutine = null;
    }

    private void OnEnable()
    {
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        tcpClient = new TcpSocket(client, dataLength, false);

        tcpClient.ClientReceived += OnReceived;
    }

    private void Start()
    {
        ARCloudReferencePointManager.AddCloudAnchorSuccess += OnCloudAnchorAdded;
        ARCloudReferencePointManager.AddCloudAnchorFail += OnCloudAnchorAddFailed;

        loadButton.onClick.AddListener(LoadCloudAnchor);
        clearButton.onClick.AddListener(ClearCatch);

        parseIDString();


    }

    private void Update()
    {
        if (tcpClient != null && tcpClient.ClientConnected)
        {
            tcpClient.ClientReceive();
        }
        else
        {
            return;
        }

        if (loadCoroutine == null && m_resolveWattingList.Count > 0)
        {
            loadCoroutine = StartCoroutine(ResolveCloudAnchor());
        }

        UpdateUI();

        if (Input.touchCount == 0)
            return;

        var touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            return;
        }

        if (m_cloudManager.cloudAnchorInProgress)
            return;

        if (m_raycastManager.Raycast(touch.position, s_Hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
        {
            Pose hitpose = s_Hits[0].pose;
            ARReferencePoint anchor = m_anchorManager.AddReferencePoint(hitpose);
            if (anchor == null)
                return;
            m_cloudManager.AddCloudReferncePoint(anchor.trackableId);
        }
    }


    private void OnCloudAnchorAdded(ARCloudReferencePoint cloudAnchor)
    {
        m_cloudAnchors.Add(cloudAnchor);
        if (!cloudAnchorIds.Contains(cloudAnchor.cloudPointId))
        {
            cloudAnchorIds.Add(cloudAnchor.cloudPointId);
            PlayerPrefs.SetString(IDS, getIDString());
            if(cloudAnchor.isHost)
                SendToServer(cloudAnchor.cloudPointId);
            
        }

    }

    private void OnCloudAnchorAddFailed(string error)
    {
        ErrorText.text = error;
    }

    private string getIDString()
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < cloudAnchorIds.Count; i++)
        {
            builder.Append(cloudAnchorIds[i]);
            if (i != cloudAnchorIds.Count - 1)
            {
                builder.Append("|");
            }
        }
        string final = builder.ToString();
        return final;
    }

    private void parseIDString()
    {
        if (cloudAnchorIds == null)
            return;
        string str = PlayerPrefs.GetString(IDS);
        string[] arr = str.Split('|');
        cloudAnchorIds.Clear();
        cloudAnchorIds.AddRange(arr);
    }

    private void LoadCloudAnchor()
    {
        m_resolveWattingList.AddRange(cloudAnchorIds);
        loadCoroutine = StartCoroutine(ResolveCloudAnchor());
    }

    /// <summary>
    /// The signal of wheather the server is dealing with a cloudAnchor
    /// the sensear server can only deal one cloudanchor in the same time.
    /// please one by one.
    /// </summary>
    /// <returns></returns>
    IEnumerator ResolveCloudAnchor()
    {
        Debug.Log("ResolveCloudAnchor start");
        yield return null;
        while (m_resolveWattingList.Count > 0)
        {
            if (m_cloudManager.cloudAnchorInProgress)
                yield return null;
            else
            {
                m_cloudManager.ResolveCloudRefenecePoint(m_resolveWattingList[0]);
                m_resolveWattingList.RemoveAt(0);
            }
        }

        Debug.Log("ResolveCloudAnchor end");
        loadCoroutine = null;

    }

    private void ClearCatch()
    {
        m_cloudManager.Reset();
        
        PlayerPrefs.DeleteKey(IDS);
        m_resolveWattingList.Clear();
        cloudAnchorIds.Clear();
    }

    private void UpdateUI()
    {
        StateText.text = string.Format("cloudAnchorInProgress: {0}", m_cloudManager.cloudAnchorInProgress);
        StateText.color = m_cloudManager.cloudAnchorInProgress ? Color.red : Color.green;

    }

    public void ConnectServer()
    {
        if (tcpClient != null)
        {
            tcpClient.ClientConnect(IPAddress, PortID);
        }
    }

    /// <summary>
    /// received id from server
    /// </summary>
    /// <param name="id"></param>
    void OnReceived(string id)
    {
        if (!cloudAnchorIds.Contains(id))
        {
            m_resolveWattingList.Add(id);
        }
    }

    public void SendToServer(string id)
    {
        if (tcpClient != null && tcpClient.ClientConnected && !string.IsNullOrEmpty(id))
        {
            tcpClient.ClientSend(System.Text.Encoding.UTF8.GetBytes(id));
        }
    }

    public void ChangeIP(string value)
    {
        IPAddress = value;
    }

    public void ChangePortID(string value)
    {
        PortID = int.Parse(value);
    }


    private void OnDisable()
    {
        tcpClient.DisConnect();
    }
}
