using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calculate : MonoBehaviour
{
    public static Calculate _instance;

    private Dropdown SetKey;
    private Dropdown SetSymbol;
    private InputField SetValue;

    private int Key;         //记录键
    private string Symbol;      //记录运算符号
    private string Value;       //记录值
    private int index;          //记录Key的索引值

    private int ModelItem;
    private int id;
    private int Item;

    private string CalculateKeyStr;
    private string CalculateSymbolStr;
    private string CalculateValueStr;

    private bool FirstShow=true;    //记录是否第一次展示

    private void Awake() {
        _instance=this;
        SetKey=transform.Find("function/variable").GetComponent<Dropdown>();
        SetSymbol=transform.Find("function/symbol").GetComponent<Dropdown>();
        SetValue=transform.Find("function/InputField").GetComponent<InputField>();
        SetValue.onEndEdit.AddListener(EndValue);
        gameObject.SetActive(false);
    }

    //输入框结束时调用 将值取出
    private void EndValue(string value){
        Value=value;
    }

    private void UpdateDropDownItem(){
        index=SetVariablePanel._instance.VariableNumber;
        //Debug.Log("index = "+index);
        if(index!=0){
            SetKey.options.Clear();
            Dropdown.OptionData keyData;
            for(int i=0;i<=index;i++){
                keyData = new Dropdown.OptionData();
                keyData.text=SetVariablePanel._instance.ReadVariableKey(i);
                //Debug.Log(SetVariablePanel._instance.ReadVariableKey(i));
                SetKey.options.Add(keyData);
            }
            //初始选项的显示    空指针
            //SetKey.captionText.text=SetVariablePanel._instance.ReadVariableKey(0);
        }
    }

    //初始化数据
    private void AwakeData(){
        ModelItem=GameManager._instance.OnClickModelItem;
        id=GameManager._instance.NowSelectBehaviour;
        Item=GameManager._instance.ReturnModelBehaviourData(ModelItem,id)-1;
        CalculateKeyStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.CalculateKey;
        CalculateSymbolStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.CalculateSymbol;
        CalculateValueStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.CalculateValue;
    }

    //显示面板是调整参数
    public void Show(){
        gameObject.SetActive(true);
        GameManager._instance.IsSetConditionOrBehaviour = true;
        SetKey.value=0;
        SetSymbol.value=0;
        SetValue.text="";
        UpdateDropDownItem();
        if(FirstShow){
            AwakeData(); 
            // if(index==0)PlayerPrefs.SetInt(CalculateKey,0);
            // else{
            //     SetKey.value=0;
            // }
            PlayerPrefs.SetInt(CalculateKeyStr,0);
            PlayerPrefs.SetString(CalculateSymbolStr,"+");
            PlayerPrefs.SetString(CalculateValueStr,"");
            FirstShow=false;
        }
        AwakeData();
        if(PlayerPrefs.HasKey(CalculateKeyStr)){
            Key=PlayerPrefs.GetInt(CalculateKeyStr);
            SetKey.value=Key;            
        }
        if(PlayerPrefs.HasKey(CalculateSymbolStr)){
            Symbol=PlayerPrefs.GetString(CalculateSymbolStr);
            switch(Symbol){
            case "+":SetSymbol.value=0;
                break;
            case "-":SetSymbol.value=1;
                break;
            case "*":SetSymbol.value=2;
                break;
            case "/":SetSymbol.value=3;
                break;
            default:
                break;
            } 
        }
        if(PlayerPrefs.HasKey(CalculateValueStr)){
            Value=PlayerPrefs.GetString(CalculateValueStr);
            SetValue.text=Value;
        }
    }

    public int ReturnKey(){//存储索引值
        Key=SetKey.value;
        return Key;
    }
    public string ReturnSymbol(){
        switch(SetSymbol.value){
            case 0:Symbol="+";
                break;
            case 1:Symbol="-";
                break;
            case 2:Symbol="*";
                break;
            case 3:Symbol="/";
                break;
            default:
                break;
            } 
        return Symbol;
    }
    public string ReturnValue(){
        return Value;
    }
}

