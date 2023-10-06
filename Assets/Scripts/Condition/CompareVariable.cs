using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompareVariable : MonoBehaviour
{
    public static CompareVariable _instance;

    private Dropdown SetKey;
    private Dropdown SetSymbol;
    private InputField SetValue;

    private int Key;         //记录键
    private string Symbol;      //记录运算符号
    private string Value;       //记录值
    private int index;          //记录Key的索引值

    private int ModelItem;
    private int Item;

    private string CompareVariableKeyStr;
    private string CompareVariableSymbolStr;
    private string CompareVariableValueStr;

    private bool FirstShow = true;      //记录是否第一次展示

    private void Awake()
    {
        _instance = this;
        SetKey = transform.Find("function/variable").GetComponent<Dropdown>();
        SetSymbol = transform.Find("function/symbol").GetComponent<Dropdown>();
        SetValue = transform.Find("function/InputField").GetComponent<InputField>();
        SetValue.onEndEdit.AddListener(EndValue);
        gameObject.SetActive(false);
    }

    //输入框结束时调用 将值取出
    private void EndValue(string value)
    {
        Value = value;
        Debug.Log(Value);
    }

    private void UpdateDropDownItem()
    {
        index = SetVariablePanel._instance.VariableNumber;
        //Debug.Log("index = "+index);
        if (index != 0)
        {
            SetKey.options.Clear();
            Dropdown.OptionData keyData;
            for (int i = 0; i <= index; i++)
            {
                keyData = new Dropdown.OptionData();
                keyData.text = SetVariablePanel._instance.ReadVariableKey(i);
                //Debug.Log(SetVariablePanel._instance.ReadVariableKey(i));
                SetKey.options.Add(keyData);
            }
            //初始选项的显示    空指针
            //SetKey.captionText.text=SetVariablePanel._instance.ReadVariableKey(0);
        }
    }

    //初始化数据
    private void AwakeData()
    {
        ModelItem = GameManager._instance.OnClickModelItem;
        //Item=0 则代表暂未添加条件 1代表进入场景 2代表准心悬停 3代表延时触发 4代表数值比较
        Item = GameManager._instance.ReturnModelCondition(ModelItem);
        CompareVariableKeyStr = ModelItem.ToString() + Item.ToString() + ConditionKey.CompareVariableKey;
        CompareVariableSymbolStr = ModelItem.ToString() + Item.ToString() + ConditionKey.CompareVariableSymbol;
        CompareVariableValueStr = ModelItem.ToString() + Item.ToString() + ConditionKey.CompareVariableValue;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        GameManager._instance.IsSetConditionOrBehaviour = true;
        SetKey.value = 0;
        SetSymbol.value = 0;
        SetValue.text = "";
        UpdateDropDownItem();
        if (FirstShow)
        {
            AwakeData();
            PlayerPrefs.SetInt(CompareVariableKeyStr, 0);
            PlayerPrefs.SetString(CompareVariableSymbolStr, "=");
            PlayerPrefs.SetString(CompareVariableValueStr, "");
            FirstShow = false;
        }
        AwakeData();
        if (PlayerPrefs.HasKey(CompareVariableKeyStr))
        {
            Key = PlayerPrefs.GetInt(CompareVariableKeyStr);
            SetKey.value = Key;
        }
        if (PlayerPrefs.HasKey(CompareVariableSymbolStr))
        {
            Symbol = PlayerPrefs.GetString(CompareVariableSymbolStr);
            switch (Symbol)
            {
                case "=":
                    SetSymbol.value = 0;
                    break;
                case ">":
                    SetSymbol.value = 1;
                    break;
                case "<":
                    SetSymbol.value = 2;
                    break;
                default:
                    break;
            }
        }
        if (PlayerPrefs.HasKey(CompareVariableValueStr))
        {
            Value = PlayerPrefs.GetString(CompareVariableValueStr);
            SetValue.text = Value;
        }
    }

    public int ReturnKey()
    {//存储索引值
        Key = SetKey.value;
        return Key;
    }
    public string ReturnSymbol()
    {
        switch (SetSymbol.value)
        {
            case 0:
                Symbol = "=";
                break;
            case 1:
                Symbol = ">";
                break;
            case 2:
                Symbol = "<";
                break;
            default:
                break;
        }
        return Symbol;
    }
    public string ReturnValue()
    {
        return Value;
    }
}
