using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelScale : MonoBehaviour
{
    public static ModelScale _instance;

    private Dropdown SetScale;
    private Dropdown SetSpeed;

    private float Scale;        //记录倍数
    private int Speed;          //记录速度

    private int ModelItem;
    private int id;
    private int Item;
    private string ModelScaleScaleStr;
    private string ModelScaleSpeedStr;

    private bool FirstShow=true;    //记录是否第一次展示

    private void Awake() {
        _instance=this;
        SetScale=transform.Find("function/func1/Dropdown").GetComponent<Dropdown>();
        SetSpeed=transform.Find("function/func2/Dropdown").GetComponent<Dropdown>();
        gameObject.SetActive(false);
    }

    //初始化数据
    private void AwakeData(){
        ModelItem=GameManager._instance.OnClickModelItem;
        id=GameManager._instance.NowSelectBehaviour;
        Item=GameManager._instance.ReturnModelBehaviourData(ModelItem,id)-1;
        ModelScaleScaleStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.ModelScaleScale.ToString();
        ModelScaleSpeedStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.ModelScaleSpeed.ToString();
    }

    //显示面板是调整参数
    public void Show(){
        gameObject.SetActive(true);
        GameManager._instance.IsSetConditionOrBehaviour = true;
        SetScale.value=0;
        SetSpeed.value=0;
        if(FirstShow){
            AwakeData(); 
            PlayerPrefs.SetFloat(ModelScaleScaleStr,0.5f);
            PlayerPrefs.SetFloat(ModelScaleSpeedStr,1);
            FirstShow=false;
        }
        AwakeData(); 
        if(PlayerPrefs.HasKey(ModelScaleScaleStr)){
            Scale=PlayerPrefs.GetFloat(ModelScaleScaleStr);
            switch(Scale){
                case 0.5f:SetScale.value=0;
                    break;
                case 1:SetScale.value=1;
                    break;
                case 2:SetScale.value=2;
                    break;
                default:
                    break;
            }
        }
        if(PlayerPrefs.HasKey(ModelScaleSpeedStr)){
            Speed=(int)PlayerPrefs.GetFloat(ModelScaleSpeedStr);
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
    public float ReturnScale(){
        switch(SetScale.value){
            case 0:Scale=0.5f;
                break;
            case 1:Scale=1;
                break;
            case 2:Scale=2;
                break;
            default:
                break;
            } 
        return Scale;
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
