using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

    public struct Node
    //用结构体来存储每个模型的 基本信息与行为个数 
    //Node go1=ScenesModel[0];
    //go1.id=0;
    //ScenesModel[0]=go1;
    {
        public GameObject Prefab;              //用于存放模型

        public string ModelName;               //展示在信息面板上的字符串
        public Sprite ModelSkin;               //展示在信息面板上的图片
        public int Condition;                  //展示在信息面板上的条件内容    从1开始计数 0则代表没有行为
        public int BehaviourCount;             //展示在信息面板上的行为个数
        public int BehaviourHaveDataCount;     //展示在信息面板上的有内容的行为个数
        public List<int> Behaviours;           //展示在信息面板上的行为内容
    }

public class GameManager : MonoBehaviour {
	public static GameManager _instance;

    [HideInInspector]public bool isSelectModel =false;//记录是否正选中模型 
    [HideInInspector]public int ModelSkin = 0;//记录即将放置模型的类别 0 1 2 3

    
    [HideInInspector]public int ModelItem = 0;//用来存放场景中模型的数量 用ModelItem来代表每个物体的编号
    [HideInInspector]public int OnClickModelItem = 0;//记录即将出现在信息面板上模型的编号
    [HideInInspector]public int OnClickBehaviourItem = 0;//记录选中的行为编号


    //利用 SelectCondition 和 SelectBehaviour 来记录当前选中的条件或行为中的哪一个 0则代表没有选中
    [HideInInspector]public int SelectCondition;
    [HideInInspector]public int SelectBehaviour;


    [HideInInspector]public int NowSelectBehaviour=0;          //当前选中的行为编号

    [HideInInspector]public Transform ModelPoint;              //选中的模型坐标，用于制作LineRenderer

    [HideInInspector]public bool IsSetConditionOrBehaviour=false;      //判断是否正在修改参数


    List<Node> ScenesModel = new List<Node>();//用来存放场景中的模型  

    //将Model的模型 类别 和 名称 的数据存储在ScenesModel结构体中的
    public void SaveModelInformation(int item,string name,Sprite skin){
        ScenesModel.Add(new Node());
        Node data = ScenesModel[item];
        data.ModelName=name;
        data.ModelSkin=skin;
        ScenesModel[item]=data;
    }
    //重写多一个预制体
    public void SaveModelInformation(int item, string name, Sprite skin,GameObject pre)
    {
        ScenesModel.Add(new Node());
        Node data = ScenesModel[item];
        data.ModelName = name;
        data.ModelSkin = skin;
        data.Prefab = pre;
        ScenesModel[item] = data;
    }
    //返回预制体
    public GameObject ReturnPrefab(int item)
    {
        return ScenesModel[item].Prefab;
    }

    private void Awake() {
        _instance=this;   
    }
    
    //添加模型行为的个数
    public void UpdateBehaviourCount(int item,int count){
        Node data= ScenesModel[item];
        data.BehaviourCount=count;
        ScenesModel[item]=data;
    }
    //删除模型行为的个数
    public void DelectBehaviourCount(int item){
        Node data= ScenesModel[item];
        data.BehaviourCount--;
        ScenesModel[item]=data;
    }
    //返回模型行为的个数
    public int ReturnBehaviourCount(int item){
        return ScenesModel[item].BehaviourCount;
    }
    
    //添加模型行为的个数（含有数据）
    public void UpdateBehaviourHaveDataCount(int item,int count){
        Node data= ScenesModel[item];
        data.BehaviourHaveDataCount=count;
        ScenesModel[item]=data;
    }
    //返回模型行为的个数（含有数据的个数）
    public int ReturnBehaviourHaveDataCount(int item){
        return ScenesModel[item].BehaviourHaveDataCount;
    }
    //添加模型的某个具体行为进结构体 List<int>Behaviours中
    public void AddBehaviourData(int item,int id){          //这里的id特指键值对的值    按顺序存储值，从0开始
        Node data= ScenesModel[item];        
        if(data.BehaviourHaveDataCount==0){
            data.Behaviours=new List<int>();
        }
        data.Behaviours.Add(id);
        //Debug.Log("id = "+data.BehaviourHaveDataCount+"Behaviours[id] = "+data.Behaviours[data.BehaviourHaveDataCount]);
        data.BehaviourHaveDataCount++;
        ScenesModel[item]=data;
    }
    //删除模型的某个具体行为出结构体 List<int>Behaviours中
    public void ReduceBehaviourData(int item,int pos){
        Node data= ScenesModel[item];
        // for(int i=pos;pos<=data.BehaviourHaveDataCount;i++){
        //     data.Behaviours[i]=data.Behaviours[i+1];
        // }
        data.Behaviours.RemoveAt(pos);
        data.BehaviourHaveDataCount--;
        ScenesModel[item]=data;
    }
    //返回模型的某个具体行为编号
    public int ReturnModelBehaviourData(int item,int id){   //这里的id特指键值对的键    通过键返回值
        //防止还未添加行为而报空指针
        if(ScenesModel[item].BehaviourHaveDataCount>id){
            return ScenesModel[item].Behaviours[id];
        }
            return 0;
    }


    //添加模型条件的内容
    public void UpdateCondition(int item,int id){
        Node data= ScenesModel[item];
        data.Condition=id;
        ScenesModel[item]=data;
    }
    //返回对应编号的模型条件
    public int ReturnModelCondition(int item){
        return ScenesModel[item].Condition;
    }





    //记录场景中模型的个数
    public void AddModelItem(){
        ModelItem++;
    }

    //返回对应编号的模型名称
    public string ReturnModelName(int item){
        return ScenesModel[item].ModelName;
    }

    //返回对应编号的模型图片
    public Sprite ReturnModelSkin(int item){
        return ScenesModel[item].ModelSkin;
    }

    //返回对应编号模型ModelInformation组件
    public ModelInformation ReturnModelInformation(int item)
    {
        ModelInformation[] mi = FindObjectsOfType<ModelInformation>();
        for (int i = 0; i < ModelItem; i++)
        {
            foreach (ModelInformation modelInfo in mi)
            {
                if (modelInfo.ReturnId() == item)
                {
                    Debug.Log(modelInfo.gameObject);
                    return modelInfo;
                }
            }
        }
        return null;
    }
    //开始调试
    public void starttogame()
    {
        ModelInformation[] modelInfo = FindObjectsOfType<ModelInformation>();
        foreach (ModelInformation m in modelInfo)
        {
            m.IsDebugging = true;
            m.IsNotTrigger = true;
            if (m.gameObject.GetComponent<VRTriggerItem>() == null)
                m.gameObject.AddComponent<VRTriggerItem>();
        }
    }

    //清空数据
    private void OnApplicationQuit() {
        PlayerPrefs.DeleteAll();
    }


    public bool DisPlane = false;//是否有平面

}
