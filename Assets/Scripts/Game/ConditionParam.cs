using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditionParam : MonoBehaviour
{
    public static ConditionParam _instance;

    private Button Delect;
    private Button Save;

    private void Awake() {
        _instance = this;
        Delect = transform.Find("Delect").gameObject.GetComponent<Button>();
        Delect.onClick.AddListener(DelectOnClick);
        Save = transform.Find("Save").gameObject.GetComponent<Button>();
        Save.onClick.AddListener(SaveOnClick);
        //初始化时禁用所有参数面板
        //HideBehaviourPanel();
        Delect.gameObject.SetActive(false);
        Save.gameObject.SetActive(false);

    }
    //传参启用条件面板
    public void ShowConditionPanel(int index) {
        //展示条件面板时隐藏行为面板
        BehaviourParam._instance.HideBehaviourPanel();

        HideConditionPanel();

        ////进入场景 条件 不需要展示面板
        //transform.GetChild(index).gameObject.SetActive(true);

        ////显示保存按钮 
        //if (index != 0)    //如果是 进入场景 则不需要显示保存按钮
        //    transform.GetChild(transform.childCount - 2).gameObject.SetActive(true);
        ////显示删除按钮
        //transform.GetChild(transform.childCount - 1).gameObject.SetActive(true);

        //根据索引值显示面板
        switch (index)
        {
            case 0:EnterScenes._instance.Show();
                DelectParam();
                break;
            case 1:WatchingModel._instance.Show();
                SaveParam();
                DelectParam();
                break;
            case 2:ComputeTime._instance.Show();
                SaveParam();
                DelectParam();
                break;
            case 3:CompareVariable._instance.Show();
                SaveParam();
                DelectParam();
                break;
            default:
                Debug.Log("超出索引值范围");
                break;

        }
    }

    //显示保存按钮
    private void SaveParam()
    {
        Save.gameObject.SetActive(true);
    }
    //显示删除按钮
    private void DelectParam()
    {
        Delect.gameObject.SetActive(true);
    }

    //关闭条件面板
    public void HideConditionPanel() {
        for (int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    //保存按钮点击事件
    private void SaveOnClick() {
        GameManager._instance.IsSetConditionOrBehaviour = false;
        EventCenter.Broadcast(EventDefine.SaveConditionPanel);
    }
    private void DelectOnClick() {
        GameManager._instance.IsSetConditionOrBehaviour = false;
        //清除模型的条件
        int ModelItem = GameManager._instance.OnClickModelItem;
        GameManager._instance.UpdateCondition(ModelItem, 0);
        GameManager._instance.SelectCondition = 0;

        //还原格子样式
        EventCenter.Broadcast(EventDefine.DelectConditionPanel);
    }
}
