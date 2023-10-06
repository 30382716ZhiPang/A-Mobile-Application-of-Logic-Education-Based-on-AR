using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TipsPanel : MonoBehaviour
{
    private GameObject Tips;
    private void Awake() {
        EventCenter.AddListener<string>(EventDefine.ShowTipsPanel,Show);
        Tips=transform.GetChild(0).gameObject;

        gameObject.SetActive(false);
    }
    
    private void OnDestroy() {
        EventCenter.RemoveListener<string>(EventDefine.ShowTipsPanel,Show);
    }

    private void Show(string tips){
        gameObject.SetActive(true);
        Tips.GetComponent<Text>().text=tips.ToString();
        Tips.transform.localPosition=new Vector3(0,-100,0);
        Tips.transform.DOLocalMoveY(0,0.5f);
        Invoke("Hide",1);
    }

    private void Hide(){
        gameObject.SetActive(false);
    }

}
