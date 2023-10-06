using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARReferencePointManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class SetPointPanel : MonoBehaviour
{
    public static SetPointPanel _instance;
    public GameObject BehaviourParam;
    public GameObject ModelInformationPanel;
    private ManagerVars vars;
    private Button Setup;
    private Button Cancel;

    private bool isMouseDown=false;
    private GameObject go=null;//用来存放置的物体

    private LineRenderer line;

    private int ModelItem;
    private int id;
    private int Item;
    private bool FirstShow=true;    //记录是否第一次展示

    //ar
    public Text Ceshi;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    List<ARReferencePoint> m_ReferencePoints;
    ARRaycastManager m_RaycastManager;
    ARReferencePointManager m_ReferencePointManager;

    private void Awake() {
        _instance=this;
        vars=ManagerVars.GetManagerVars();
        gameObject.SetActive(false);
        //添加按钮的监听事件
        Setup=transform.Find("FunctionButton/SetUp").GetComponent<Button>();
        Cancel=transform.Find("FunctionButton/Cancel").GetComponent<Button>();

        Setup.onClick.AddListener(SetUpOnClick);
        Cancel.onClick.AddListener(CancelOnClick);


    }

    public void Show(){
        Init();

        //判断是否有参数
        ModelItem=GameManager._instance.OnClickModelItem;
        id=GameManager._instance.NowSelectBehaviour;
        Item=GameManager._instance.ReturnModelBehaviourData(ModelItem,id)-1;
        string posXStr="";
        string posYStr="";
        string posZStr="";
        if(id==0){//此时为 移动行为 的设置点
            posXStr=BehaviourKeyWord.MovePosX; 
            posYStr=BehaviourKeyWord.MovePosY;
            posZStr=BehaviourKeyWord.MovePosZ;
        }else if(id==1){//此时为 转向目标 的设置点
            posXStr=BehaviourKeyWord.LootAtPosX; 
            posYStr=BehaviourKeyWord.LootAtPosY;
            posZStr=BehaviourKeyWord.LootAtPosZ;
        }else{
            //可进行添加功能
        }
        string PosXStr=ModelItem.ToString()+id.ToString()+Item.ToString()+posXStr;
        string PosYStr=ModelItem.ToString()+id.ToString()+Item.ToString()+posYStr;
        string PosZStr=ModelItem.ToString()+id.ToString()+Item.ToString()+posZStr;
        
        if(PlayerPrefs.HasKey(PosXStr)&&PlayerPrefs.HasKey(PosYStr)&&
            PlayerPrefs.HasKey(PosZStr)){
                isMouseDown=true;
                if(go==null)go = GameObject.Instantiate(vars.SetPointPre,new Vector3(0,0,0),Quaternion.identity);
                go.transform.position=new Vector3(PlayerPrefs.GetFloat(PosXStr),
                    PlayerPrefs.GetFloat(PosYStr),PlayerPrefs.GetFloat(PosZStr));
                ShowLineRenderer(go.transform.position,GameManager._instance.ModelPoint.position);
        }
        if(FirstShow){
            PlayerPrefs.SetFloat(PosXStr,0);
            PlayerPrefs.SetFloat(PosYStr,0);
            PlayerPrefs.SetFloat(PosZStr,0);
            FirstShow=false;
        }
    }

    private void Init(){
        gameObject.SetActive(true);
        isMouseDown=false;
        transform.Find("FunctionButton").gameObject.SetActive(false);
        EventCenter.Broadcast(EventDefine.HideMainPanel);
        //暂时关闭行为参数面板，结束设置位置后打开
        BehaviourParam.SetActive(false);
        ModelInformationPanel.SetActive(false);
    }

    private void Start() {
        //AR
        m_RaycastManager = ARManager.This.GetComponent<ARRaycastManager>();
        m_ReferencePointManager = ARManager.This.GetComponent<ARReferencePointManager>();
        m_ReferencePoints = new List<ARReferencePoint>();
    }

    private void Update() {
        //防止误触UI
        if (IsPointerOverGameObject(Input.mousePosition)) {
				return;
		}
        
        //判断是否开始放置模型位置
        if(GameManager._instance.isSelectModel==true){
            //打开按钮，关闭文字闪烁
            transform.Find("FunctionButton").gameObject.SetActive(true);
        }
        if (GameManager._instance.DisPlane)
        {
            if (Input.touchCount == 0)
                return;
            var touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began)
                return;
            if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                if (GameManager._instance.isSelectModel == false)
                    GameManager._instance.isSelectModel = true;
                Ceshi.text += "'if 1";
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;
                Ceshi.text += "\nhitPose1:" + hitPose;
                var referencePoint = m_ReferencePointManager.AddReferencePoint(hitPose);
                if (referencePoint == null)
                {
                    Debug.Log("Error creating reference point");
                    Ceshi.text += "\nError creating reference point";
                }
                else
                {
                    if (isMouseDown == false)
                    {
                        isMouseDown = true;
                        Ceshi.text += "\nhitPose2:" + hitPose;
                        go = GameObject.Instantiate(vars.SetPointPre, new Vector3(0, 0, 0), Quaternion.identity);
                        go.transform.position = hitPose.position;
                        //ARManager.This.gameObject.GetComponent<ARReferencePointManager>().referencePointPrefab = go;
                        Ceshi.text += "\nreferencepointPre.name:" + ARManager.This.gameObject.GetComponent<ARReferencePointManager>().referencePointPrefab.name;
                        m_ReferencePoints.Add(referencePoint);
                        ShowLineRenderer(go.transform.position, GameManager._instance.ModelPoint.position);
                    }
                    else
                    {
                        go.transform.position = hitPose.position;
                        ShowLineRenderer(go.transform.position, GameManager._instance.ModelPoint.position);
                    }

                }
            }
        }
        else
        {
            Debug.Log("Nothing creating plane");
            Ceshi.text += "\nNothing creating plane";
        }
        //if (Input.GetMouseButtonDown(0)){
        //    if(GameManager._instance.isSelectModel==false)
        //        GameManager._instance.isSelectModel=true;

        //    //开始点击场景进行放置模型
        //    if(isMouseDown==false){//如果场景还没有放置该模型，则进行实例化
        //        //创建一条射线
        //        Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
        //        RaycastHit hit;
        //        //射线碰撞到位置时
        //        if(Physics.Raycast(ray,out hit)){  
        //            go = GameObject.Instantiate(vars.SetPointPre,new Vector3(0,0,0),Quaternion.identity);
        //            go.transform.position=hit.point;
        //            //DOTO 提供两个坐标进行画线
        //            ShowLineRenderer(go.transform.position,GameManager._instance.ModelPoint.position);
        //        }

        //        isMouseDown=true;
        //    }else{//场景内已有模型，进行修改位置
        //        Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
        //        RaycastHit hit;
        //        if(Physics.Raycast(ray,out hit)){
        //            go.transform.position=hit.point;
        //            ShowLineRenderer(go.transform.position,GameManager._instance.ModelPoint.position);
        //        }
        //    }
        //}
    }


    //防止在放置模型时误触UI
    private bool IsPointerOverGameObject(Vector2 mousePoint){
		//创建一个点击事件
		PointerEventData eventData=new PointerEventData(EventSystem.current);
		eventData.position = mousePoint;
		List<RaycastResult> raycastResults = new List<RaycastResult> ();
		//向点击位置发射一条射线，检测是否点击到UI
		EventSystem.current.RaycastAll (eventData, raycastResults);
		return raycastResults.Count > 0;
	}


    //放置按钮事件
    private void SetUpOnClick(){
        SetUpModel();
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        EventCenter.Broadcast(EventDefine.ShowInformationPanel);
        
        //DOTO 保存数据
        string posXStr="";
        string posYStr="";
        string posZStr="";
        if(id==0){//此时为 移动行为 的设置点
            posXStr=BehaviourKeyWord.MovePosX; 
            posYStr=BehaviourKeyWord.MovePosY;
            posZStr=BehaviourKeyWord.MovePosZ;
        }else if(id==1){//此时为 转向目标 的设置点
            posXStr=BehaviourKeyWord.LootAtPosX; 
            posYStr=BehaviourKeyWord.LootAtPosY;
            posZStr=BehaviourKeyWord.LootAtPosZ;
        }else{
            //可进行添加功能
        }
        string PosXStr=ModelItem.ToString()+id.ToString()+Item.ToString()+posXStr;
        string PosYStr=ModelItem.ToString()+id.ToString()+Item.ToString()+posYStr;
        string PosZStr=ModelItem.ToString()+id.ToString()+Item.ToString()+posZStr;
        //进行新建或保存数据持久化

        PlayerPrefs.SetFloat(PosXStr,go.transform.position.x);
        PlayerPrefs.SetFloat(PosYStr,go.transform.position.y);
        PlayerPrefs.SetFloat(PosZStr,go.transform.position.z); 
    }

    //取消按钮事件
    private void CancelOnClick(){
        SetUpModel();
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        EventCenter.Broadcast(EventDefine.ShowInformationPanel);
    }

    //完成放置，退出当前面板
    private void SetUpModel(){
        isMouseDown=false;
        ClearLineRenderer();
        BehaviourParam.SetActive(true);
        ModelInformationPanel.SetActive(true);
        //清除坐标
        Destroy(go);
        

        //打开主面板，关闭当前SetUp面板
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        gameObject.SetActive(false);
    }


    //提供两个坐标进行画线
    [System.Obsolete]
    public void ShowLineRenderer(Vector3 go,Vector3 ModelPoint){
        line=transform.GetComponent<LineRenderer>();
        line.SetWidth(0.01f, 0.01f);
        line.SetPosition(0,ModelPoint);
        line.SetPosition(1,go);
    }

    private void ClearLineRenderer(){
        line.SetPosition(0,Vector3.zero);
        line.SetPosition(1,Vector3.zero);
    }

}

