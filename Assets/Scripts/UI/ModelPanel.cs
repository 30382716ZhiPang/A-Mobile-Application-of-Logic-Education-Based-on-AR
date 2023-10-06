using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class ModelPanel : MonoBehaviour {
	private ManagerVars vars;
	private Transform parent;
	private Button Back;//模型面板中的返回按钮
	private Button ChooseModel;//模型面板中的选中按钮
	private Text txt_Name;
	private int selectIndex;

	private void Awake(){
		gameObject.SetActive(false);
		parent = transform.Find("ScrollRect/Parent");
		txt_Name = transform.Find("ModelName").GetComponent<Text>();
		vars = ManagerVars.GetManagerVars();

		EventCenter.AddListener(EventDefine.ShowModelPanel,Show);

		Init();
	}
	private void OnDestroy() {
		EventCenter.RemoveListener(EventDefine.ShowModelPanel,Show);
	}
	private void Show(){
		gameObject.SetActive(true);
		//隐藏信息面板
		EventCenter.Broadcast(EventDefine.HidInformationPanel);
	}

	private void Init(){
		//打开模型选择面板时隐藏功能面板
		ConditionSwitch._instance.isOpen=false;
		
		//设置返回按钮事件
		Back = transform.Find("Back").GetComponent<Button>();
		Back.onClick.AddListener(BackOnClick);
		//设置选择按钮事件
		ChooseModel = transform.Find("ChooseModel").GetComponent<Button>();
		ChooseModel.onClick.AddListener(ChooseModelOnClick);
		
		//将模型从ManagerVars中实例化至面板中
		for(int i=0;i<vars.Prefabs.Count;i++){
			GameObject go = Instantiate(vars.ModelChooseItemPre,parent);
			go.GetComponentInChildren<Image>().sprite=vars.Prefabs[i];
			go.transform.localPosition=new Vector3((160*i-240),0,0);
		}
	}

	//设置模型面板的平滑移动
	private void Update() {
		selectIndex = (int)Mathf.Round((parent.transform.localPosition.x-200)/-160.0f);
		if(selectIndex<0)//防止数组越界
			selectIndex=0;
		if(selectIndex>parent.childCount-1)
			selectIndex=parent.childCount-1;
		if(Input.GetMouseButtonUp(0)){
			parent.GetComponent<RectTransform>().anchoredPosition=new Vector2(710+(selectIndex*-160),0);
		}
		SetItemSize(selectIndex);
		RefreshName(selectIndex);
	}

	//设置模型面板的模型尺寸
	private void SetItemSize(int selectIndex){
		for(int i =0;i<parent.childCount;i++){
			if(selectIndex==i){
				parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta=new Vector2(160,160);
			}else{
				parent.GetChild(i).GetChild(0).GetComponent<RectTransform>().sizeDelta=new Vector2(80,80);
			}
		}
	}

	//更新模型面板模型对应的名称
	private void RefreshName(int selectIndex){
		txt_Name.text=vars.ModelNameList[selectIndex];
	}

	//返回按钮事件
	private void BackOnClick(){
		EventCenter.Broadcast(EventDefine.ShowMainPanel);
		gameObject.SetActive(false);
	}
	//选择按钮事件
	private void ChooseModelOnClick(){
		GameManager._instance.ModelSkin=selectIndex;
		EventCenter.Broadcast(EventDefine.ShowSetUpPanel);
		gameObject.SetActive(false);
	}


}
