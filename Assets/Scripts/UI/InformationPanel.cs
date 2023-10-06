using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationPanel : MonoBehaviour
{
    private ManagerVars vars;               //调用资源管理器容器
    
    private GameObject BehaviourPrefab;     //用于实例化的行为预制体
    private Button AddBehaviour;            //添加新行为的按钮
    private Button ReduceBehaviour;         //删除行为的按钮
    private Transform Function;             //记录生成行为预制体的父物体


    private Button addModelCondition;       //给模型添加条件
    private Button addModelBehaviour;       //给模型添加行为


    //用于信息面板上的信息
    private Text txt_name;
    private Image img_model;

    private int ModelItem=0;                //记录生成的模型编号
    private int Item;                       //记录生成的条件类型
    private int BehaviourCount=1;           //面板上模型的行为个数 默认有1个

    private void Awake() {
        //进入场景后隐藏面板
        gameObject.SetActive(false); 

        //初始化资源管理器容器  Information面板中需要调用行为的预制体
        vars=ManagerVars.GetManagerVars();

        //添加显示隐藏面板的事件
        EventCenter.AddListener(EventDefine.ShowInformationPanel,Show);
        EventCenter.AddListener(EventDefine.HidInformationPanel,Hide);
        EventCenter.AddListener(EventDefine.AddBehaviourButtonDown,AddBehaviourOnClick);
        EventCenter.AddListener(EventDefine.DelectConditionPanel,DelectConditionOnClick);
        EventCenter.AddListener(EventDefine.SaveConditionPanel, SaveConditionOnClick);

        //赋值
        BehaviourPrefab=vars.BehaviourPre;
        Function=transform.Find("ScrollRect/Function");

        //为添加行为按钮添加事件监听
        AddBehaviour=transform.Find("AddBehaviour").GetComponent<Button>();
        AddBehaviour.onClick.AddListener(AddBehaviourOnClick);
        ReduceBehaviour=transform.Find("ReduceBehaviour").GetComponent<Button>();
        ReduceBehaviour.onClick.AddListener(ReduceBehaviourOnClick);
        addModelCondition=transform.Find("Condition").GetComponent<Button>();
        addModelCondition.onClick.AddListener(AddModelCondition);


        txt_name=transform.Find("Model/txt_name").GetComponent<Text>();
        img_model=transform.Find("Model/img_model").GetComponent<Image>();

        
    }
    private void OnDestroy() {
        EventCenter.RemoveListener(EventDefine.ShowInformationPanel,Show);
        EventCenter.RemoveListener(EventDefine.HidInformationPanel,Hide);
        EventCenter.RemoveListener(EventDefine.AddBehaviourButtonDown,AddBehaviourOnClick);
        EventCenter.RemoveListener(EventDefine.DelectConditionPanel,DelectConditionOnClick);
        EventCenter.RemoveListener(EventDefine.SaveConditionPanel, SaveConditionOnClick);
    }

    //显示面板时
    private void Show(){
        //读取模型编号下的  图片、名称、行为个数，并将行为个数实例化至面板上Function下
        ModelItem=GameManager._instance.OnClickModelItem;
        Item = GameManager._instance.ReturnModelCondition(ModelItem);
        BehaviourCount =GameManager._instance.ReturnBehaviourCount(ModelItem);

        //展示信息
        txt_name.text=GameManager._instance.ReturnModelName(ModelItem).ToString();
        img_model.sprite=GameManager._instance.ReturnModelSkin(ModelItem);
        
        //每次加载前先清空面板上的条件数据
        addModelCondition.transform.Find("txt_name").gameObject.SetActive(true);
        addModelCondition.transform.Find("Image").gameObject.SetActive(false);
        if (GameManager._instance.ReturnModelCondition(ModelItem)!=0){
            addModelCondition.transform.Find("txt_name").gameObject.SetActive(false);
            addModelCondition.transform.Find("Image").gameObject.SetActive(true);
            addModelCondition.transform.Find("Image").GetChild(0).gameObject.GetComponent<Image>().sprite
                =vars.conditionSprite[GameManager._instance.ReturnModelCondition(ModelItem)-1];
        }

        //实例化面板上所有的行为数量
        ClearAllBehaviours();
        UpdateBehaviourPanel();

        gameObject.SetActive(true);
    }

    //隐藏面板时
    private void Hide(){
        ClearAllBehaviours();
        //清空功能面板的索引值
        GameManager._instance.SelectBehaviour=0;
        GameManager._instance.SelectCondition=0;

        //关闭条件参数面板
        ConditionParam._instance.HideConditionPanel();
        BehaviourParam._instance.HideBehaviourPanel();

        GameManager._instance.IsSetConditionOrBehaviour = false;
        gameObject.SetActive(false);
    }



    //添加行为按钮点击事件
    private void AddBehaviourOnClick(){
        //行为个数只能大于含有数据的行为个数1个
        if(BehaviourCount>=GameManager._instance.ReturnBehaviourHaveDataCount(ModelItem)+1){
            Debug.Log("请对空行为添加数据再进行添加新行为");
        }
        if(BehaviourCount<GameManager._instance.ReturnBehaviourHaveDataCount(ModelItem)+1){            
            BehaviourCount++;
            //修改行为的个数
            GameManager._instance.UpdateBehaviourCount(ModelItem,BehaviourCount);
            UpdateBehaviourPanel();
      }

    }
    //删除行为按钮点击事件
    private void ReduceBehaviourOnClick(){
        //如果行为只剩下一个 或 行为内含有数据   则无法删除 
        if(BehaviourCount<=GameManager._instance.ReturnBehaviourHaveDataCount(ModelItem)+1){
            Debug.Log("当前最后行为含有数据或只剩最后一个，无法进行删除");
        }
        if(BehaviourCount>1&&BehaviourCount>GameManager._instance.ReturnBehaviourHaveDataCount(ModelItem)+1)BehaviourCount--;
        GameManager._instance.UpdateBehaviourCount(ModelItem,BehaviourCount);
        UpdateBehaviourPanel();
    }



    //更新面板上的行为数量
    private void UpdateBehaviourPanel(){
        ClearAllBehaviours();
        for(int i=0;i<BehaviourCount;i++){
            GameObject go=GameManager.Instantiate(BehaviourPrefab,Function);
            go.transform.localPosition=new Vector3((65*i-209),0,0);
            //基于每个生成的行为编号
            go.GetComponent<BehaviourInformation>().SaveData(ModelItem,i);
        }


        //DOTO 将具体行为显示出来
    }

    //清除所有的行为
    private void ClearAllBehaviours(){
        for(int i=0;i<Function.transform.childCount;i++){
            Destroy(Function.GetChild(i).gameObject);
        }
    }

    //添加模型的条件
    private void AddModelCondition(){
        //DOTO 进行判断模型条件是否为空，如为空则添加条件，否则显示条件参数

        //DOTO  显示条件参数
        if(GameManager._instance.ReturnModelCondition(ModelItem)!=0){
            ConditionParam._instance.ShowConditionPanel(
                GameManager._instance.ReturnModelCondition(ModelItem)-1);
        }
        
        //添加条件
        if(GameManager._instance.SelectBehaviour==0&&GameManager._instance.SelectCondition!=0){

            ModelInformation modelInfo = GameManager._instance.ReturnModelInformation(ModelItem);
            modelInfo.AddCondition(GameManager._instance.SelectCondition);

            //将条件存进模型的结构体中
            GameManager._instance.UpdateCondition(ModelItem,GameManager._instance.SelectCondition);
            
            addModelCondition.transform.Find("txt_name").gameObject.SetActive(false);
            addModelCondition.transform.Find("Image").gameObject.SetActive(true);
            addModelCondition.transform.Find("Image").GetChild(0).gameObject.GetComponent<Image>().sprite
                =vars.conditionSprite[GameManager._instance.ReturnModelCondition(ModelItem)-1];
                
            //清空功能面板的索引值
            GameManager._instance.SelectBehaviour=0;
            GameManager._instance.SelectCondition=0;

            //隐藏条件参数面板
            ConditionParam._instance.HideConditionPanel();
        }

        
    }

    //保存条件面板上的条件
    private void SaveConditionOnClick()
    {
        //Item=0 则代表暂未添加条件 1代表进入场景 2代表准心悬停 3代表延时触发 4代表数值比较
        //0 0 0 模型编号 条件类型 条件参数
        Item = GameManager._instance.ReturnModelCondition(ModelItem);
        switch (Item)
        {
            case 0://无数据

                break;
            case 1://进入场景
                string EnterEnterScenesStr = ModelItem.ToString() + Item.ToString() + ConditionKey.EnterScenes;
                PlayerPrefs.SetFloat(EnterEnterScenesStr, 1);
                break;
            case 2://准心悬停
                string WatchingModelTimeStr = ModelItem.ToString() + Item.ToString() + ConditionKey.WatchingModelTime;
                PlayerPrefs.SetFloat(WatchingModelTimeStr, WatchingModel._instance.ReturnTime());
                break;
            case 3://延时触发
                string ComputeTimeStr = ModelItem.ToString() + Item.ToString() + ConditionKey.ComputeTimeTime;
                PlayerPrefs.SetFloat(ComputeTimeStr, ComputeTime._instance.ReturnTime());
                break;
            case 4://数值比较
                string CompareVariableKeyStr = ModelItem.ToString()  + Item.ToString() + ConditionKey.CompareVariableKey;
                string CompareVariableSymbolStr = ModelItem.ToString() + Item.ToString() + ConditionKey.CompareVariableSymbol;
                string CompareVariableValueStr = ModelItem.ToString() + Item.ToString() + ConditionKey.CompareVariableValue;
                PlayerPrefs.SetInt(CompareVariableKeyStr, CompareVariable._instance.ReturnKey());
                PlayerPrefs.SetString(CompareVariableSymbolStr, CompareVariable._instance.ReturnSymbol());
                PlayerPrefs.SetString(CompareVariableValueStr, CompareVariable._instance.ReturnValue());
                break;
        }
    }

    //清除条件面板上的条件
    private void DelectConditionOnClick(){
        addModelCondition.transform.Find("txt_name").gameObject.SetActive(true);
        addModelCondition.transform.Find("Image").gameObject.SetActive(false);

        //隐藏条件参数面板
        ConditionParam._instance.HideConditionPanel();
    }

}
