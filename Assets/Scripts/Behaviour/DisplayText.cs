using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayText : MonoBehaviour
{
    public static DisplayText _instance;

    private InputField Display;
    private Dropdown SetShowTime;

    private string Text;        //记录字符串
    private int ShowTime;       //记录展示时间
    private int ModelItem;
    private int id;
    private int Item;
    private string DisplayTextStr;
    private string DisplayTextShowTimeStr;

    private bool FirstShow=true;    //记录是否第一次展示

    private void Awake() {
        _instance=this;
        Display=transform.Find("function/func1/InputField").GetComponent<InputField>();
        Display.onEndEdit.AddListener(EndValue);
        SetShowTime=transform.Find("function/func2/Dropdown").GetComponent<Dropdown>();
        gameObject.SetActive(false);
    }

    //输入框结束时调用 将值取出
    private void EndValue(string value){
        Text=value;
    }

    //初始化数据
    private void AwakeData(){
        ModelItem=GameManager._instance.OnClickModelItem;
        id=GameManager._instance.NowSelectBehaviour;
        Item=GameManager._instance.ReturnModelBehaviourData(ModelItem,id)-1;
        DisplayTextStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.DisplayTextText;
        DisplayTextShowTimeStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.DisplayTextShowTime;
    }


    //显示面板是调整参数
    public void Show(){
        gameObject.SetActive(true);
        GameManager._instance.IsSetConditionOrBehaviour = true;
        Display.text="";
        SetShowTime.value=0;
        if(FirstShow){
            AwakeData(); 
            PlayerPrefs.SetString(DisplayTextStr,"");
            PlayerPrefs.SetFloat(DisplayTextShowTimeStr,1);
            FirstShow=false;
        }
        AwakeData(); 
        if(PlayerPrefs.HasKey(DisplayTextStr)){
            Text=PlayerPrefs.GetString(DisplayTextStr);
            Display.text=Text;
        }
        if(PlayerPrefs.HasKey(DisplayTextShowTimeStr)){
            ShowTime=(int)PlayerPrefs.GetFloat(DisplayTextShowTimeStr);
            switch(ShowTime){
                case 1:SetShowTime.value=0;
                    break;
                case 2:SetShowTime.value=1;
                    break;
                case 5:SetShowTime.value=2;
                    break;
                default:
                    break;
            }
        }
    }
    
    //获取当前数据
    public string ReturnText(){
        return Text;
    }
    
    public int ReturnShowTime(){
        switch(SetShowTime.value){
            case 0:ShowTime=1;
                break;
            case 1:ShowTime=2;
                break;
            case 2:ShowTime=5;
                break;
            default:
                break;
            } 
        return ShowTime;
    }
}
