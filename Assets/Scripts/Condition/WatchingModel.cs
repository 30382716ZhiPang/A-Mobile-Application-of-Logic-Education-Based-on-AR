using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WatchingModel : MonoBehaviour
{
    public static WatchingModel _instance;
    private Dropdown SetTime;

    private float Time;           //记录悬停时间

    private int ModelItem;
    private int Item;
    private string WatchingModelTimeStr;

    private bool FirstShow = true;      //记录是否第一次展示

    private void Awake()
    {
        _instance = this;
        SetTime = transform.Find("function/func/Dropdown").GetComponent<Dropdown>();
        gameObject.SetActive(false);
    }

    //初始化数据
    private void AwakeData()
    {
        ModelItem = GameManager._instance.OnClickModelItem;
        //Item=0 则代表暂未添加条件 1代表进入场景 2代表准心悬停 3代表延时触发 4代表数值比较
        Item = GameManager._instance.ReturnModelCondition(ModelItem);
        WatchingModelTimeStr = ModelItem.ToString() + Item.ToString() + ConditionKey.WatchingModelTime;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        GameManager._instance.IsSetConditionOrBehaviour = true;
        SetTime.value = 0;
        if (FirstShow)
        {
            AwakeData();
            PlayerPrefs.SetFloat(WatchingModelTimeStr, 0.5f);
            FirstShow = false;
        }
        AwakeData();
        if (PlayerPrefs.HasKey(WatchingModelTimeStr))
        {
            Time = PlayerPrefs.GetFloat(WatchingModelTimeStr);
            switch (Time)
            {
                case 0.5f:SetTime.value = 0;
                    break;
                case 1:SetTime.value = 1;
                    break;
                case 2:SetTime.value = 2;
                    break;
                default:
                    break;
            }
        }
    }

    public float ReturnTime()
    {
        switch (SetTime.value)
        {
            case 0:Time = 0.5f;
                break;
            case 1:Time = 1;
                break;
            case 2:Time = 2;
                break;
            default:
                break;
        }
        return Time;
    }
}
