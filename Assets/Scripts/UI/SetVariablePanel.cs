using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class Variable
    //用类来存储变量的Key和Value
    {
        public string Key;
        public int Value;
    }

public class SetVariablePanel : MonoBehaviour
{
    public static SetVariablePanel _instance;
    List<Variable> variables=new List<Variable>();
    public int VariableNumber=0;                                 //用来记录变量的编号与数量
    private Transform parent;
    private Button AddVariable;
    private Button ClosePanel;
    private Button SaveVariable;

    private ManagerVars vars;

    //存储变量
    public void SaveVariableInformation(int id,string key,int value){
        if(id<VariableNumber+1)
        variables.Add(new Variable());
        variables[id].Key=key;
        variables[id].Value=value;
    }
    //增加变量块
    public void AddVariableCount(){
        VariableNumber++;
    }
    //读取变量
    public string ReadVariableKey(int item){
        return variables[item].Key;
    }
    public int ReadVariableValue(int item){
        return variables[item].Value;
    }

    private void Awake() {
        _instance=this;
        vars=ManagerVars.GetManagerVars();
        
        parent=transform.Find("Panel/function/ScrollRect/Parent").transform;
        AddVariable=transform.Find("AddVariable").GetComponent<Button>();
        ClosePanel=transform.Find("ClosePanel").GetComponent<Button>();
        SaveVariable=transform.Find("SaveVariable").GetComponent<Button>();
        AddVariable.onClick.AddListener(AddVariableOnClick);
        ClosePanel.onClick.AddListener(ClosePanelOnClick);
        SaveVariable.onClick.AddListener(SaveVariableOnClick);
        EventCenter.AddListener(EventDefine.ShowSetVariablePanel,Show);

        gameObject.SetActive(false);
    }
    
    private void OnDestroy() {
        EventCenter.RemoveListener(EventDefine.ShowSetVariablePanel,Show);
    }

    private void Show(){
        gameObject.SetActive(true);
        if(parent.childCount==0){
            Instantiate(vars.VariablePre,parent);
        }
        EventCenter.Broadcast(EventDefine.HidInformationPanel);
        EventCenter.Broadcast(EventDefine.HideMainPanel);
        if(ConditionSwitch._instance.isOpen==true){
            ConditionSwitch._instance.SwitchButtonClick();
        }
    }

    //添加变量按钮点击事件
    private void AddVariableOnClick(){
        //DOTO 如果变量块都不为空 则添加新的变量块
        AddVariableCount();
        GameObject go = Instantiate(vars.VariablePre,parent);
        go.transform.localPosition-=new Vector3(0,45*VariableNumber,0);
    }

    //保存变量按钮点击事件
    private void SaveVariableOnClick(){
        //DOTO 对数据进行保存
        for(int i=0;i<=VariableNumber;i++){
            parent.GetChild(i).GetComponent<VariableInformation>().SaveVariableInformation();
        }
    }

    //关闭面板按钮点击事件
    private void ClosePanelOnClick(){
        EventCenter.Broadcast(EventDefine.ShowMainPanel);
        //关闭时进行保存数据
        SaveVariableOnClick();
        gameObject.SetActive(false);
    }
}
