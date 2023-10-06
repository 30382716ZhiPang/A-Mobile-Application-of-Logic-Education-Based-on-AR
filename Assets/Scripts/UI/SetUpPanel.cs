using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARReferencePointManager))]
[RequireComponent(typeof(ARRaycastManager))]
public class SetUpPanel : MonoBehaviour
{
    private ManagerVars vars;
    private Button Setup;
    private Button Cancel;
    private Button LeftRotate;
    private Button RightRotate;

    private bool isMouseDown = false;
    private GameObject go = null;//用来存放置的物体
    
    private int SelectModelIndex = 99;//用来记录模型类别的编号 0 1 2 3
    private int ModelItem = 0;//用来记录每个模型实例化的顺序 顺序即为模型的编号

    private ModelInformation ModelInformationPre;
    
    //ar
    public Text Ceshi;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    List<ARReferencePoint> m_ReferencePoints;
    ARRaycastManager m_RaycastManager;
    ARReferencePointManager m_ReferencePointManager;

    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        gameObject.SetActive(false);
        EventCenter.AddListener(EventDefine.ShowSetUpPanel, Show);
        //添加按钮的监听事件
        Setup = transform.Find("FunctionButton/SetUp").GetComponent<Button>();
        Cancel = transform.Find("FunctionButton/Cancel").GetComponent<Button>();
        LeftRotate = transform.Find("FunctionButton/LeftRotate").GetComponent<Button>();
        RightRotate = transform.Find("FunctionButton/RightRotate").GetComponent<Button>();

        Setup.onClick.AddListener(SetUpOnClick);
        Cancel.onClick.AddListener(CancelOnClick);
        LeftRotate.onClick.AddListener(LeftRotateOnClick);
        RightRotate.onClick.AddListener(RightRotateOnClick);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowSetUpPanel, Show);
    }
    private void Show()
    {
        gameObject.SetActive(true);
        transform.Find("FunctionButton").gameObject.SetActive(false);

        //更新当前需要放置模型的类别编号
        SelectModelIndex = GameManager._instance.ModelSkin;
        //更新每个模型实例化的顺序
        ModelItem = GameManager._instance.ModelItem;
    }

    private void Start()
    {
        //AR
        m_RaycastManager = ARManager.This.GetComponent<ARRaycastManager>();
        m_ReferencePointManager = ARManager.This.GetComponent<ARReferencePointManager>();
        m_ReferencePoints = new List<ARReferencePoint>();
    }

    private void Update()
    {
        //防止误触UI
        if (IsPointerOverGameObject(Input.mousePosition))
        {
            return;
        }

        //判断是否开始放置模型位置
        if (GameManager._instance.isSelectModel == true)
        {
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
                        go = GameObject.Instantiate(vars.ModelParent, new Vector3(0, 0, 0), Quaternion.identity);
                        GameObject model = Instantiate(vars.ModelPrefabs[SelectModelIndex], new Vector3(0, 0, 0), Quaternion.identity);
                        model.transform.SetParent(go.transform);
                        go.transform.position = hitPose.position;
                        go.transform.eulerAngles = new Vector3(0, 180, 0);

                        //ARManager.This.gameObject.GetComponent<ARReferencePointManager>().referencePointPrefab = go;
                        Ceshi.text += "\nreferencepointPre.name:" + ARManager.This.gameObject.GetComponent<ARReferencePointManager>().referencePointPrefab.name;
                        m_ReferencePoints.Add(referencePoint);
                    }
                    else
                    {
                        go.transform.position = hitPose.position;
                    }

                }
            }
        }
        else
        {
            Debug.Log("Nothing creating plane");
            Ceshi.text += "\nNothing creating plane";
        }
        /*
         if (Input.GetMouseButtonDown(0)){
             if(GameManager._instance.isSelectModel==false)
                 GameManager._instance.isSelectModel=true;

             //开始点击场景进行放置模型
             if(isMouseDown==false){//如果场景还没有放置该模型，则进行实例化
                 //创建一条射线
                 Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
                 RaycastHit hit;
                 //射线碰撞到位置时
                 if(Physics.Raycast(ray,out hit)){                    
                     go = GameObject.Instantiate(vars.ModelParent,new Vector3(0,0,0),Quaternion.identity);
                     GameObject model = Instantiate(vars.ModelPrefabs[SelectModelIndex], new Vector3(0, 0, 0), Quaternion.identity);
                     model.transform.SetParent(go.transform);
                     go.transform.position=hit.point;
                     go.transform.eulerAngles=new Vector3(0,180,0);
                 }

                 isMouseDown=true;
             }else{//场景内已有模型，进行修改位置
                 Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
                 RaycastHit hit;
                 if(Physics.Raycast(ray,out hit)){
                     go.transform.position=hit.point;
                 }
             }
             */
    }


    //防止在放置模型时误触UI
    private bool IsPointerOverGameObject(Vector2 mousePoint)
    {
        //创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePoint;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //向点击位置发射一条射线，检测是否点击到UI
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults.Count > 0;
    }


    //放置按钮事件
    private void SetUpOnClick()
    {
        Ceshi.text += "\nSetUpOnClick()";

        //将数据存进GameManager中
        if (go != null)
        {
            Ceshi.text += "\nSetUpOnClick()_1";

            //ModelInformationPre=go.GetComponent<ModelInformation>();
            //给实例化的模型创建一个编号，存储在ModelInformation中
            //ModelInformation.id=ModelItem;
            go.GetComponent<ModelInformation>().SetId(ModelItem);

            GameManager._instance.SaveModelInformation(ModelItem,
                vars.ModelNameList[SelectModelIndex].ToString(), vars.Prefabs[SelectModelIndex], go);
            //GameManager._instance.SaveModelInformation(ModelItem,
            //    vars.ModelNameList[SelectModelIndex].ToString(), vars.Prefabs[SelectModelIndex]);

            //将模型信息存储至GameManager的结构体中
            GameManager._instance.AddModelItem();

            Ceshi.text += "\nSetUpOnClick()_2";
            //DOTO 模型条件和行为的存储
            //初始化模型的行为个数
            GameManager._instance.UpdateBehaviourCount(ModelItem, 1);
            //初始化模型的行为个数（实际有存储数据的个数）
            GameManager._instance.UpdateBehaviourHaveDataCount(ModelItem, 0);

            //初始化模型的条件内容  初始化为0
            GameManager._instance.UpdateCondition(ModelItem, 0);
            Ceshi.text += "\nSetUpOnClick()_3";
        }
        if (ConditionSwitch._instance.isOpen == false)
        {
            Ceshi.text += "\nSetUpOnClick()_4";
            ConditionSwitch._instance.SwitchButtonClick();
        }
        SetUpModel();
    }

    //取消按钮事件
    private void CancelOnClick()
    {
        Destroy(go);
        SetUpModel();
    }

    //完成放置，退出当前面板
    private void SetUpModel()
    {
        //将数据清除
        isMouseDown = false;
        GameManager._instance.ModelSkin = 99;
        GameManager._instance.isSelectModel = false;

        //打开主面板，关闭当前SetUp面板
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        gameObject.SetActive(false);
    }

    //左转按钮事件
    private void LeftRotateOnClick()
    {
        ModelRotate(30);
    }
    //右转按钮事件
    private void RightRotateOnClick()
    {
        ModelRotate(-30);
    }
    //模型转动
    private void ModelRotate(int angle)
    {
        go.transform.eulerAngles += new Vector3(0, angle, 0);
    }

}
