using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ConditionSwitch : MonoBehaviour
{
    public static ConditionSwitch _instance;
    private Button Cswitch;
    public bool isOpen=false;
    private const int SwitchisOpen = 377;
    public const int SwitchisClose = 525;

    // Start is called before the first frame update
    void Awake()
    {
        _instance=this;
        Cswitch=transform.Find("Switch").GetComponent<Button>();
        Cswitch.onClick.AddListener(SwitchButtonClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchButtonClick(){
        if (isOpen==true){
            transform.DOLocalMoveX(SwitchisClose,0.5f);
            GameObject.Find("Switch").GetComponent<Transform>().Rotate(new Vector3(0,0,180));
            isOpen=false;
        }else{
            transform.DOLocalMoveX(SwitchisOpen,0.5f);
            GameObject.Find("Switch").GetComponent<Transform>().Rotate(new Vector3(0,0,180));
            isOpen=true;
        }
    }
}
