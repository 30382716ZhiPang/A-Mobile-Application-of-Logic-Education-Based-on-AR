using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourParam : MonoBehaviour
{
    public static BehaviourParam _instance;
    
    private Button Delect;
    private Button Save;
    
    private void Awake() {
        _instance=this;
        Delect=transform.Find("Delect").gameObject.GetComponent<Button>();
        Delect.onClick.AddListener(DelectOnClick);
        Save=transform.Find("Save").gameObject.GetComponent<Button>();
        Save.onClick.AddListener(SaveOnClick);
        //初始化时禁用所有参数面板
        //HideBehaviourPanel();
        Delect.gameObject.SetActive(false);
        Save.gameObject.SetActive(false);
    }

    //传参启用行为面板
    public void ShowBehaviourPanel(int index){
        //展示行为面板时隐藏条件面板
        ConditionParam._instance.HideConditionPanel();

        HideBehaviourPanel();

        //transform.GetChild(index).gameObject.SetActive(true);
        // //显示保存按钮
        // if(index!=6)    //如果是 删除模型 则不需要显示保存按钮
        // transform.GetChild(transform.childCount-2).gameObject.SetActive(true);
        // //显示删除按钮
        // transform.GetChild(transform.childCount-1).gameObject.SetActive(true);

        //根据索引值显示面板
        switch(index){
            case 0:Move._instance.Show();
            SaveParam();
            DelectParam();
            break;
            case 1:LootAtPos._instance.Show();
            SaveParam();
            DelectParam();
            break;
            case 2:RotationBySelf._instance.Show();
            SaveParam();
            DelectParam();
            break;
            case 3:DisplayText._instance.Show();
            SaveParam();
            DelectParam();
            break;
            case 4:ModelScale._instance.Show();
            SaveParam();
            DelectParam();
            break;
            case 5:ModelChange._instance.Show();
            SaveParam();
            DelectParam();
            break;
            case 6:ModelDestroy._instance.Show();
            DelectParam();
            break;
            case 7:Calculate._instance.Show();
            SaveParam();
            DelectParam();
            break;
            case 8:PutOffTime._instance.Show();
            SaveParam();
            DelectParam();
            break;
            case 9:DoToAction._instance.Show();
            SaveParam();
            DelectParam();
            break;
            default:Debug.Log("超出索引值范围");
            break;
        }
    }

    //显示保存按钮
    private void SaveParam(){
        Save.gameObject.SetActive(true);
    }
    //显示删除按钮
    private void DelectParam(){
        Delect.gameObject.SetActive(true);
    }

    //关闭条件面板
    public void HideBehaviourPanel(){
        for(int i=0;i<transform.childCount;i++){
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    //保存按钮点击事件
    private void SaveOnClick(){
        GameManager._instance.IsSetConditionOrBehaviour = false;
        EventCenter.Broadcast(EventDefine.SaveBehaviourPanel,true);
        
    }
    //删除按钮点击事件
    private void DelectOnClick(){
        GameManager._instance.IsSetConditionOrBehaviour = false;
        EventCenter.Broadcast(EventDefine.SaveBehaviourPanel,false);
        EventCenter.Broadcast(EventDefine.DelectBehaviourPanel);
        EventCenter.Broadcast(EventDefine.SaveBehaviourPanel,false);
    }


}
