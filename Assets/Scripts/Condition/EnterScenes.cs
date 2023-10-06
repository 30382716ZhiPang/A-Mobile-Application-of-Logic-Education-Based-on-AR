using UnityEngine;

public class EnterScenes : MonoBehaviour
{
    public static EnterScenes _instance;
    private int ModelItem;
    private int Item;
    private string EnterScenesStr;

    private bool FirstShow = true;      //记录是否第一次展示

    private void Awake()
    {
        _instance = this;
        gameObject.SetActive(false);
    }

    //初始化数据
    private void AwakeData()
    {
        ModelItem = GameManager._instance.OnClickModelItem;
        //Item=0 则代表暂未添加条件 1代表进入场景 2代表准心悬停 3代表延时触发 4代表数值比较
        Item = GameManager._instance.ReturnModelCondition(ModelItem);
        EnterScenesStr = ModelItem.ToString() + Item.ToString() + ConditionKey.EnterScenes;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        GameManager._instance.IsSetConditionOrBehaviour = true;
        if (FirstShow)
        {
            AwakeData();
            PlayerPrefs.SetFloat(EnterScenesStr,1);     //0 1 1 模型编号 条件类型 是否删除
            FirstShow = false;
        }
    }
}
