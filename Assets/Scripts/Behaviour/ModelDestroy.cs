using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelDestroy : MonoBehaviour
{
    public static ModelDestroy _instance;
    private int ModelItem;
    private int id;
    private int Item;
    private string ModelDestroyStr;

    private bool FirstShow=true;    //记录是否第一次展示

    private void Awake() {
        _instance=this;
        gameObject.SetActive(false);
    }

    //初始化数据
    private void AwakeData(){
        ModelItem=GameManager._instance.OnClickModelItem;
        id=GameManager._instance.NowSelectBehaviour;
        Item=GameManager._instance.ReturnModelBehaviourData(ModelItem,id)-1;
        ModelDestroyStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.ModelDestroy;
    }

    public void Show(){
        gameObject.SetActive(true);
        GameManager._instance.IsSetConditionOrBehaviour = true;
        if (FirstShow){
            AwakeData(); 
            PlayerPrefs.SetFloat(ModelDestroyStr,1);    //0 0 0 1   模型编号 行为编号 行为类型 是否删除
            FirstShow=false;
        }
    }
}
