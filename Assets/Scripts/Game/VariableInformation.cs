using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableInformation : MonoBehaviour
{
    private int id;//记录保存的变量编号

    //实例化后应该即可赋值id
    private void Start(){
        id=SetVariablePanel._instance.VariableNumber;
    }

    //保存变量的信息
    public void SaveVariableInformation(){
        //保存信息
        if(transform.Find("Value").GetComponent<InputField>().text!=null&&transform.Find("Key").GetComponent<InputField>().text!=null){
            int value;
            int.TryParse(transform.Find("Value").GetComponent<InputField>().text,out value);
            SetVariablePanel._instance.SaveVariableInformation(id,
                transform.Find("Key").GetComponent<InputField>().text,value
            );
        }else{
            SetVariablePanel._instance.SaveVariableInformation(id,"",0);
        }
        // for(int i=0;i<=id;i++){
        //     Debug.Log("ID = "+id+" Key = "+SetVariablePanel._instance.ReadVariableKey(id));
        // }
    }
    
}
