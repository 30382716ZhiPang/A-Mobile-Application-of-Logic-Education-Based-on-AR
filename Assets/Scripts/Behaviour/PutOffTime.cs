using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PutOffTime : MonoBehaviour
{
    public static PutOffTime _instance;
    private Dropdown SetTime;

    private int Time;           //记录延迟时间

    private int ModelItem;
    private int id;
    private int Item;
    private string PutOffTimeStr;

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
        id = GameManager._instance.NowSelectBehaviour;
        Item = GameManager._instance.ReturnModelBehaviourData(ModelItem, id) - 1;
        PutOffTimeStr = ModelItem.ToString() + id.ToString() + Item.ToString() + BehaviourKeyWord.PutOffTimeTime;
    }

    //显示面板是调整参数
    public void Show()
    {
        gameObject.SetActive(true);
        GameManager._instance.IsSetConditionOrBehaviour = true;
        SetTime.value = 0;
        if (FirstShow)
        {
            AwakeData();
            PlayerPrefs.SetFloat(PutOffTimeStr, 1);
            FirstShow = false;
        }
        AwakeData();
        if (PlayerPrefs.HasKey(PutOffTimeStr))
        {
            Time = (int)PlayerPrefs.GetFloat(PutOffTimeStr);
            switch (Time)
            {
                case 1:
                    SetTime.value = 0;
                    break;
                case 2:
                    SetTime.value = 1;
                    break;
                case 5:
                    SetTime.value = 2;
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
            case 0:
                Time = 1;
                break;
            case 1:
                Time = 2;
                break;
            case 2:
                Time = 5;
                break;
            default:
                break;
        }
        return Time;
    }
}
