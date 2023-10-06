using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour {
	private Button ChooseModel;
	private Button SetUpVariable;
    private Button StartProgram;
	private Button LeftRotate;
    private Button RightRotate;
	private GameObject go;//记录选中的模型
    private ManagerVars vars;

	private void Awake(){
		Init ();
		EventCenter.AddListener(EventDefine.ShowMainPanel,Show);
		EventCenter.AddListener(EventDefine.HideMainPanel,Hide);
	}
	private void OnDestroy() {
		EventCenter.RemoveListener(EventDefine.ShowMainPanel,Show);
		EventCenter.RemoveListener(EventDefine.HideMainPanel,Hide);
	}
	
	private void Show(){
		transform.Find("FunctionButton").gameObject.SetActive(false);
		gameObject.SetActive(true);
	}

	private void Hide(){
		gameObject.SetActive(false);
	}

	private void Init(){
		ChooseModel=transform.Find("ChooseModel").GetComponent<Button>();
		SetUpVariable=transform.Find("ChooseModel/SetUpVariable").GetComponent<Button>();
        StartProgram = transform.Find("ChooseModel/StartProgram").GetComponent<Button>();

        ChooseModel.onClick.AddListener(OpenModelPanel);
		SetUpVariable.onClick.AddListener(OpenSetVariablePanel);
        StartProgram.onClick.AddListener(OpenStartProgram);

        LeftRotate =transform.Find("FunctionButton/LeftRotate").GetComponent<Button>();
        RightRotate=transform.Find("FunctionButton/RightRotate").GetComponent<Button>();

        LeftRotate.onClick.AddListener(LeftRotateOnClick);
        RightRotate.onClick.AddListener(RightRotateOnClick);

        vars = ManagerVars.GetManagerVars();
    }


	 //点击物体查看其详细信息
    private void Update() {
        if(Input.GetMouseButtonDown(0)){
            Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray,out hit)){

				//平面放置物体
				if(hit.collider.tag=="Model"){
					//将模型的编号放置GameManager的OnClickModelItem中
					go=hit.collider.gameObject;
					go.GetComponentInParent<ModelInformation>().SaveIdInGameManager();
					//存储选中的模型坐标
					GameManager._instance.ModelPoint=go.transform.parent;

					//展示旋转按钮
					transform.Find("FunctionButton").gameObject.SetActive(true);

					//打开信息面板
					if(GameManager._instance.IsSetConditionOrBehaviour==false)
                        EventCenter.Broadcast(EventDefine.ShowInformationPanel);
				}else{
					//如果没有按在信息面板上则隐藏面板
					if(!IsPointerOverGameObject(Input.mousePosition)){
						transform.Find("FunctionButton").gameObject.SetActive(false);
						go=null;
						EventCenter.Broadcast(EventDefine.HidInformationPanel);
					}
				}

			}
        }
    }
    //防止在放置模型时误触UI
    private bool IsPointerOverGameObject(Vector2 mousePoint){
		//创建一个点击事件
		PointerEventData eventData=new PointerEventData(EventSystem.current);
		eventData.position = mousePoint;
		List<RaycastResult> raycastResults = new List<RaycastResult> ();
		//向点击位置发射一条射线，检测是否点击到UI
		EventSystem.current.RaycastAll (eventData, raycastResults);
		return raycastResults.Count > 0;
	}
    //左转按钮事件
    private void LeftRotateOnClick()
    {
        ModelRotate(30);
    }
    //右转按钮事件
    private void RightRotateOnClick()
    {
        ModelRotate(-30);
    }
    //模型转动
    private void ModelRotate(int angle)
    {
        go.transform.parent.eulerAngles += new Vector3(0, angle, 0);
        go.GetComponentInParent<ModelInformation>().startRotate = go.transform.parent.localEulerAngles;
    }

    //打开模型面板
    private void OpenModelPanel(){
		EventCenter.Broadcast(EventDefine.ShowModelPanel);
		gameObject.SetActive(false);
	}
    //打开变量面板
	private void OpenSetVariablePanel(){
		EventCenter.Broadcast(EventDefine.ShowSetVariablePanel);
		gameObject.SetActive(false);
	}

    //开始执行程序
    private void OpenStartProgram()
    {
        ModelInformation[] modelInfo = FindObjectsOfType<ModelInformation>();
        foreach (ModelInformation m in modelInfo)
        {
            m.IsDebugging = true;
            m.IsNotTrigger = true;
            if (m.gameObject.GetComponent<VRTriggerItem>() == null)
                m.gameObject.AddComponent<VRTriggerItem>();
        }
    }

    //private void OpenStartProgram1()
    //{
    //    //关闭所有按钮 反正误触
    //    ChooseModel.gameObject.SetActive(false);

    //    for (int i = 0; i <= GameManager._instance.ModelItem; i++)  //i代表模型的编号
    //    {
    //        //StartCoroutine("Program", i);

    //        //如何访问每个模型的条件
    //        int ModelCondition = GameManager._instance.ReturnBehaviourCount(i);
    //        switch (ModelCondition)
    //        {
    //            case 0://无条件
    //                break;
    //            case 1://进入场景   可直接执行后面行为
    //                break;
    //            case 2://准心悬停   
    //                break;
    //            case 3://延时触发   i代表模型编号 ModelCondition代表模型条件的类型
    //                string ComputeTimeStr = i.ToString() + ModelCondition.ToString() + ConditionKey.ComputeTimeTime;

    //                float time = 0;
    //                float timer = PlayerPrefs.GetFloat(ComputeTimeStr);
    //                time += Time.deltaTime;
    //                if (time >= timer)
    //                {
    //                    //可执行后面行为
    //                }
    //                break;
    //            case 4://数值比较
    //                string CompareVariableKeyStr = i.ToString() + ModelCondition.ToString() + ConditionKey.CompareVariableKey;
    //                string CompareVariableSymbolStr = i.ToString() + ModelCondition.ToString() + ConditionKey.CompareVariableSymbol;
    //                string CompareVariableValueStr = i.ToString() + ModelCondition.ToString() + ConditionKey.CompareVariableValue;
    //                int Value = PlayerPrefs.GetInt(CompareVariableKeyStr);                  //存在数据库中的值
    //                int value = int.Parse(PlayerPrefs.GetString(CompareVariableValueStr));  //需要进行比对的值

    //                switch (PlayerPrefs.GetString(CompareVariableSymbolStr))
    //                {
    //                    case "=":
    //                        if (SetVariablePanel._instance.ReadVariableValue(Value) == value)
    //                        {
    //                            //可执行后面行为
    //                        }
    //                        break;
    //                    case ">":
    //                        if (SetVariablePanel._instance.ReadVariableValue(Value) < value)
    //                        {
    //                            //可执行后面行为
    //                        }
    //                        break;
    //                    case "<":
    //                        if (SetVariablePanel._instance.ReadVariableValue(Value) > value)
    //                        {
    //                            //可执行后面行为
    //                        }
    //                        break;
    //                    default:
    //                        break;
    //                }
    //                break;

    //            default:
    //                break;
    //        }

    //        //如何执行每个模型的行为
    //        int ModelBehaviourCount = GameManager._instance.ReturnBehaviourHaveDataCount(i);
    //        //如若出错则可以考虑将 ReturnBehaviourHaveDataCount 修改为 ReturnBehaviourCount
    //        for (int j = 0; j < ModelBehaviourCount; j++)   //j代表模型的第几个行为
    //        {
    //            int ModelBehaviourItem = GameManager._instance.ReturnModelBehaviourData(i, j) - 1; //每个模型的具体行为
    //            switch (ModelBehaviourItem)
    //            {
    //                case 0://移动
    //                    string MoveSpeedStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.MoveSpeed;
    //                    string MovePosXStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.MovePosX;
    //                    string MovePosYStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.MovePosY;
    //                    string MovePosZStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.MovePosZ;
    //                    //模型移动的点与速度
    //                    Vector3 MovePos = new Vector3(PlayerPrefs.GetFloat(MovePosXStr), PlayerPrefs.GetFloat(MovePosYStr), PlayerPrefs.GetFloat(MovePosZStr));
    //                    int MoveSpeed = (int)PlayerPrefs.GetFloat(MoveSpeedStr);
    //                    //暂未获取模型i   可通过GameManager._instance.ReturnPrefab()获取，暂未通过检测
    //                    //GameObject go=GameManager._instance.ReturnPrefab();
    //                    //DOTO 执行行为

    //                    break;
    //                case 1://转向目标
    //                    string LootAtPosSpeedStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.LootAtSpeed;
    //                    string LootAtPosXStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.LootAtPosX;
    //                    string LootAtPosYStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.LootAtPosY;
    //                    string LootAtPosZStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.LootAtPosZ;
    //                    //模型转向的点与速度
    //                    Vector3 LootAtPos = new Vector3(PlayerPrefs.GetFloat(LootAtPosXStr), PlayerPrefs.GetFloat(LootAtPosYStr), PlayerPrefs.GetFloat(LootAtPosZStr));
    //                    int LootAtPosSpeed = (int)PlayerPrefs.GetFloat(LootAtPosSpeedStr);
    //                    //DOTO 执行行为

    //                    break;
    //                case 2://自转
    //                    string RotationBySelfCircleStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.RotationBySelfCircle;
    //                    string RotationBySelfSpeedStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.RotationBySelfSpeed;
    //                    //模型自转的圈数与速度
    //                    int RotationBySelfCircle = (int)PlayerPrefs.GetFloat(RotationBySelfCircleStr);
    //                    int RotationBySelfSpeed = (int)PlayerPrefs.GetFloat(RotationBySelfSpeedStr);
    //                    //DOTO 执行行为

    //                    break;
    //                case 3://眼前文字
    //                    string DisplayTextStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.DisplayTextText;
    //                    string DisplayTextShowTimeStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.DisplayTextShowTime;
    //                    //眼前文字的内容与展示时间
    //                    string DisplayText = PlayerPrefs.GetString(DisplayTextStr);
    //                    int DisplayTextShowTime = (int)PlayerPrefs.GetFloat(DisplayTextShowTimeStr);
    //                    //DOTO 执行行为

    //                    break;
    //                case 4://模型变化
    //                    string ModelScaleScaleStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.ModelScaleScale.ToString();
    //                    string ModelScaleSpeedStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.ModelScaleSpeed.ToString();
    //                    float ModelScaleScale = PlayerPrefs.GetFloat(ModelScaleScaleStr); //变化大小为 0.5 1 2倍
    //                    int ModelScaleSpeed = (int)PlayerPrefs.GetFloat(ModelScaleSpeedStr);
    //                    //DOTO 执行行为

    //                    break;
    //                case 5://更换模型
    //                    string ModelChangeStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.ModelChangeItem;
    //                    int index = (int)PlayerPrefs.GetFloat(ModelChangeStr);
    //                    //可调用 vars.ModelPrefabs[index] 来获取预制体
    //                    //DOTO 执行行为

    //                    break;
    //                case 6://删除模型
    //                    //DOTO 执行行为

    //                    break;
    //                case 7://数值运算
    //                    string CalculateKeyStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.CalculateKey;
    //                    string CalculateSymbolStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.CalculateSymbol;
    //                    string CalculateValueStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.CalculateValue;

    //                    int Value = PlayerPrefs.GetInt(CalculateKeyStr);                  //存在数据库中的值
    //                    int value = int.Parse(PlayerPrefs.GetString(CalculateValueStr));  //需要进行运算的值
    //                    switch (PlayerPrefs.GetString(CalculateSymbolStr))
    //                    {
    //                        case "+":
    //                            if (SetVariablePanel._instance.ReadVariableValue(Value) == value)
    //                            {
    //                                Value += value;
    //                            }
    //                            break;
    //                        case "-":
    //                            if (SetVariablePanel._instance.ReadVariableValue(Value) < value)
    //                            {
    //                                Value -= value;
    //                            }
    //                            break;
    //                        case "*":
    //                            if (SetVariablePanel._instance.ReadVariableValue(Value) > value)
    //                            {
    //                                Value *= value;
    //                            }
    //                            break;
    //                        case "/":
    //                            if (SetVariablePanel._instance.ReadVariableValue(Value) > value)
    //                            {
    //                                Value /= value;
    //                            }
    //                            break;
    //                        default:
    //                            break;
    //                    }
    //                    //如果仅作为展示可不将值还原，但最好在程序结束后将变量值改为原本的值
    //                    //运行结束后打开所有按钮
    //                    //ChooseModel.gameObject.SetActive(true);
    //                    break;
    //                case 8://时间推迟
    //                    string PutOffTimeStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.PutOffTimeTime;
    //                    float time = 0;
    //                    float timer = PlayerPrefs.GetFloat(PutOffTimeStr);
    //                    time += Time.deltaTime;
    //                    if (time >= timer)
    //                    {
    //                        //可执行后面行为
    //                    }
    //                    break;
    //                case 9://做动作
    //                    string DoToActionItemStr = i.ToString() + j.ToString() + ModelBehaviourItem.ToString() + BehaviourKeyWord.DoToActionItem;
    //                    switch (PlayerPrefs.GetString(DoToActionItemStr))
    //                    {
    //                        case "跳舞":
    //                            //让模型跳舞 Action在Prefabs文件夹下Actions中
    //                            break;
    //                        case "跑步":
                                
    //                            break;
    //                        case "太极":
                                
    //                            break;
    //                        case "讨论":
                                
    //                            break;
    //                        default:
    //                            break;
    //                    }
    //                    break;
    //                default:
    //                    break;
    //            }
    //        }
    //    }
    //}

    //IEnumerator Program(int ModelItem)
    //{
    //    yield return 0;
    //}



}