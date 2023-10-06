using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RotationBySelf : MonoBehaviour
{
    public static RotationBySelf _instance;

    private Dropdown SetCircle;
    private Dropdown SetSpeed;

    private int Circle;         //记录圈数
    private int Speed;          //记录速度

    private int ModelItem;
    private int id;
    private int Item;
    private string RotationBySelfCircleStr;
    private string RotationBySelfSpeedStr;

    private bool FirstShow=true;    //记录是否第一次展示
    private void Awake() {
        _instance=this;
        SetCircle=transform.Find("function/func1/Dropdown").GetComponent<Dropdown>();
        SetSpeed=transform.Find("function/func2/Dropdown").GetComponent<Dropdown>();
        gameObject.SetActive(false);
    }

    //初始化数据
    private void AwakeData(){
        ModelItem=GameManager._instance.OnClickModelItem;
        id=GameManager._instance.NowSelectBehaviour;
        Item=GameManager._instance.ReturnModelBehaviourData(ModelItem,id)-1;
        RotationBySelfCircleStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.RotationBySelfCircle.ToString();
        RotationBySelfSpeedStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.RotationBySelfSpeed.ToString();
    }

    //显示面板是调整参数
    public void Show(){
        gameObject.SetActive(true);
        GameManager._instance.IsSetConditionOrBehaviour = true;
        SetCircle.value=0;
        SetSpeed.value=0;
        if(FirstShow){
            AwakeData(); 
            PlayerPrefs.SetFloat(RotationBySelfCircleStr,1);
            PlayerPrefs.SetFloat(RotationBySelfSpeedStr,1);
            FirstShow=false;
        }
        AwakeData(); 
        if(PlayerPrefs.HasKey(RotationBySelfCircleStr)){
            Circle=(int)PlayerPrefs.GetFloat(RotationBySelfCircleStr);
            switch(Circle){
            case 1:SetCircle.value=0;
                break;
            case 2:SetCircle.value=1;
                break;
            case 5:SetCircle.value=2;
                break;
            default:
                break;
            }
        }
        if(PlayerPrefs.HasKey(RotationBySelfSpeedStr)){
            Speed=(int)PlayerPrefs.GetFloat(RotationBySelfSpeedStr);
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
    
    //获取当前数据
    public int ReturnCircle(){
        switch(SetCircle.value){
            case 0:Circle=1;
                break;
            case 1:Circle=2;
                break;
            case 2:Circle=5;
                break;
            default:
                break;
            } 
        return Circle;
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
