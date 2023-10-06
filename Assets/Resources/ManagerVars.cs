using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName="ManagerVarsContainer")]
public class ManagerVars : ScriptableObject {
	public static ManagerVars GetManagerVars(){
		return Resources.Load<ManagerVars> ("ManagerVarsContainer");
	}
	
	//模型的选择,暂用Sprite代替GameObject
	public List<Sprite> Prefabs = new List<Sprite> ();			//存放模型面板的展示
	public GameObject[] ModelPrefabs;		//用于存放放置的模型
	public GameObject ModelChooseItemPre;	//存放选中的模型预制体
	public GameObject BehaviourPre;			//存放行为的预制体
	public GameObject SetPointPre;			//存放坐标的预制体
	public GameObject VariablePre;			//存放变量块的预制体
	public List<string> ModelNameList = new List<string> ();	//存放模型的名称集


	public List<string> conditionList = new List<string> ();	//存放条件的类型
	public List<string> behaviourList = new List<string> ();	//存放行为的类型

	public List<Sprite> conditionSprite = new List<Sprite>();	//存放条件的样式
	public List<Sprite> behaviourSprite = new List<Sprite>();	//存放行为的样式

    public GameObject ModelParent; //模型父物体
}
