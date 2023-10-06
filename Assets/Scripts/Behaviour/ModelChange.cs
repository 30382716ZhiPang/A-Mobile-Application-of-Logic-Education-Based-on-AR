using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelChange : MonoBehaviour
{
    public static ModelChange _instance;
    private ManagerVars vars;
    private int index=0;            //记录展示模型编号索引值

    private int ModelItem;
    private int id;
    private int Item;
    private string ModelChangeStr;

    private bool FirstShow=true;    //记录是否第一次展示

    private Button Left;
    private Button Right;

    private void Awake() {        
        _instance=this;
        vars=ManagerVars.GetManagerVars();
        Left=transform.Find("function/LeftButton").GetComponent<Button>();
        Right=transform.Find("function/RightButton").GetComponent<Button>();
        Left.onClick.AddListener(LeftButtonDown);
        Right.onClick.AddListener(RightButtonDown);
        gameObject.SetActive(false);
    }

    //初始化数据
    private void AwakeData(){
        ModelItem=GameManager._instance.OnClickModelItem;
        id=GameManager._instance.NowSelectBehaviour;
        Item=GameManager._instance.ReturnModelBehaviourData(ModelItem,id)-1;
        ModelChangeStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.ModelChangeItem;
    }

    private void LeftButtonDown(){
        if(index!=0){
            index--;
            UpdateModelSkin();
        }
    }
    private void RightButtonDown(){
        if(index<vars.Prefabs.Count-1){
            index++;
            UpdateModelSkin();
        }
    }
    
    //更新模型的图片 并将值返回出去
    private void UpdateModelSkin(){        
        transform.Find("function/Image").GetComponent<Image>().sprite=vars.Prefabs[index];
    }

    //显示面板是调整参数
    public void Show(){
        gameObject.SetActive(true);
        GameManager._instance.IsSetConditionOrBehaviour = true;
        index =0;
        if(FirstShow){
            AwakeData(); 
            PlayerPrefs.SetFloat(ModelChangeStr,0);
            FirstShow=false;
        }
        AwakeData();
        if(PlayerPrefs.HasKey(ModelChangeStr)){
            index=(int)PlayerPrefs.GetFloat(ModelChangeStr);
        }
        UpdateModelSkin();
    }
    public void Show(int item){
        gameObject.SetActive(true);
        index=item;
        UpdateModelSkin();
    }
    
    //获取当前数据
    public int ReturnIndex(){
        return index;
    }
    
}
