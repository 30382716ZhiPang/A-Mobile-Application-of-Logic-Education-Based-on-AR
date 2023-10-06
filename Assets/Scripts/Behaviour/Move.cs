using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
    public static Move _instance;
    private Button SetPoint;
    private Dropdown SetSpeed;

    private int Speed;          //记录速度

    private int ModelItem;
    private int id;
    private int Item;
    private string MoveSpeedStr;

    private bool FirstShow=true;    //记录是否第一次展示

    private void Awake() {
        _instance=this;
        SetPoint=transform.Find("function/func1/Button").GetComponent<Button>();
        SetPoint.onClick.AddListener(SetPointOnClick);
        SetSpeed=transform.Find("function/func2/Dropdown").GetComponent<Dropdown>();
        gameObject.SetActive(false);
    }

    //初始化数据
    private void AwakeData(){
        ModelItem=GameManager._instance.OnClickModelItem;
        id=GameManager._instance.NowSelectBehaviour;
        Item=GameManager._instance.ReturnModelBehaviourData(ModelItem,id)-1;
        MoveSpeedStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.MoveSpeed;
    }

    //显示面板是调整参数
    public void Show(){
        gameObject.SetActive(true);
        GameManager._instance.IsSetConditionOrBehaviour = true;
        SetSpeed.value=0;
        if(FirstShow){
            AwakeData(); 
            PlayerPrefs.SetFloat(MoveSpeedStr,1);
            FirstShow=false;
        }
        AwakeData();
        if(PlayerPrefs.HasKey(MoveSpeedStr)){
            Speed=(int)PlayerPrefs.GetFloat(MoveSpeedStr);
            switch(Speed){
            case 1:SetSpeed.value=0;
                break;
            case 2:SetSpeed.value=1;
                break;
            case 5:SetSpeed.value=2;
                break;
            default:
                break;
            } 
        }
    }

    //添加设置位置按钮点击事件
    private void SetPointOnClick(){
        SetPointPanel._instance.Show();
    }
    
    public int ReturnSpeed(){
        switch(SetSpeed.value){
            case 0:Speed=1;
                break;
            case 1:Speed=2;
                break;
            case 2:Speed=5;
                break;
            default:
                break;
            } 
        return Speed;
    }
}
