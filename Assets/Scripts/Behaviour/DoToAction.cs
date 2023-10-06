using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoToAction : MonoBehaviour
{
    public static DoToAction _instance;
    private Dropdown SetItem;
    
    private string action = "跳舞";            //记录动作的索引值

    private int ModelItem;
    private int id;
    private int Item;
    private string DoToActionItemStr;

    private bool FirstShow = true;    //记录是否第一次展示

    private void Awake()
    {
        _instance = this;
        SetItem = transform.Find("function/func/Dropdown").GetComponent<Dropdown>();
        gameObject.SetActive(false);
    }

    //初始化数据
    private void AwakeData()
    {
        ModelItem = GameManager._instance.OnClickModelItem;
        id = GameManager._instance.NowSelectBehaviour;
        Item = GameManager._instance.ReturnModelBehaviourData(ModelItem, id) - 1;
        DoToActionItemStr = ModelItem.ToString() + id.ToString() + Item.ToString() + BehaviourKeyWord.DoToActionItem;
    }

    //显示面板是调整参数
    public void Show()
    {
        gameObject.SetActive(true);
        GameManager._instance.IsSetConditionOrBehaviour = true;
        action = "跳舞";
        if (FirstShow)
        {
            AwakeData();
            PlayerPrefs.SetString(DoToActionItemStr, "跳舞");
            FirstShow = false;
        }
        AwakeData();
        if (PlayerPrefs.HasKey(DoToActionItemStr))
        {
            action = PlayerPrefs.GetString(DoToActionItemStr);
            switch (action)
            {
                case "跳舞":
                    SetItem.value = 0;
                    break;
                case "跑步":
                    SetItem.value = 1;
                    break;
                case "太极":
                    SetItem.value = 2;
                    break;
                case "讨论":
                    SetItem.value = 3;
                    break;
                default:
                    break;

            }
        }
    }

    //获取当前数据
    public string ReturnAction()
    {
        switch (SetItem.value)
        {
            case 0:
                action = "跳舞";
                break;
            case 1:
                action = "跑步";
                break;
            case 2:
                action = "太极";
                break;
            case 3:
                action = "讨论";
                break;
            default:
                break;
        }
        return action;
    }

}
