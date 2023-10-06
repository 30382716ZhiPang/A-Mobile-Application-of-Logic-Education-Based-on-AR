using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARStand;

[RequireComponent(typeof(ARCloudReferencePointManager))]
public class CloudReferencePointCreaeteManager : MonoBehaviour
{
    private static readonly string IDS = "CloudIDs";

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

    private ARSessionOrigin m_origin;
    private ARReferencePointManager m_anchorManager;
    private ARRaycastManager m_raycastManager;
    private ARCloudReferencePointManager m_cloudManager;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    private List<ARCloudReferencePoint> m_cloudAnchors = new List<ARCloudReferencePoint>();



    private void Awake()
    {
        cloudAnchorIds = new List<string>();
        m_resolveWattingList = new List<string>();
        m_origin = GetComponent<ARSessionOrigin>();
        m_anchorManager = GetComponent<ARReferencePointManager>();
        m_raycastManager = GetComponent<ARRaycastManager>();
        m_cloudManager = GetComponent<ARCloudReferencePointManager>();

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
        if(!cloudAnchorIds.Contains(cloudAnchor.cloudPointId))
        {
            cloudAnchorIds.Add(cloudAnchor.cloudPointId);
            PlayerPrefs.SetString(IDS, getIDString());
        }

    }

    private void OnCloudAnchorAddFailed(string error)
    {
        ErrorText.text = error;
    }

    private string getIDString()
    {
        StringBuilder builder = new StringBuilder();
        for(int i = 0; i < cloudAnchorIds.Count; i++ )
        {
            builder.Append(cloudAnchorIds[i]);
            if(i != cloudAnchorIds.Count - 1)
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
        StartCoroutine(ResolveCloudAnchor());
    }

    /// <summary>
    /// The signal of wheather the server is dealing with a cloudAnchor
    /// the sensear server can only deal one cloudanchor in the same time.
    /// please one by one.
    /// </summary>
    /// <returns></returns>
    IEnumerator ResolveCloudAnchor()
    {
        while(m_resolveWattingList.Count > 0)
        {
            if (m_cloudManager.cloudAnchorInProgress)
                yield return null;
            else
            {
                m_cloudManager.ResolveCloudRefenecePoint(m_resolveWattingList[0]);
                m_resolveWattingList.RemoveAt(0);
            }
        }
        
    }

    private void ClearCatch()
    {
        m_cloudManager.Reset();
        PlayerPrefs.DeleteKey(IDS);
        cloudAnchorIds.Clear();
        m_resolveWattingList.Clear();
    }

    private void UpdateUI()
    {

        StateText.text = string.Format("cloudAnchorInProgress: {0}", m_cloudManager.cloudAnchorInProgress);
        StateText.color = m_cloudManager.cloudAnchorInProgress ? Color.red : Color.green;
    }

}
