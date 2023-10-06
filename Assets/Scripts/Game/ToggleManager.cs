using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleManager : MonoBehaviour
{
    
    public enum ConditionToggle{
        Condition_EnterScenes = 1,     //进入场景
        Condition_WatchingModel = 2,   //准心悬停
        Condition_ComputeTime = 3,     //计时触发
        Condition_CompareVariable = 4, //数值比较
    }
    public enum BehaviourToggle{
        Behaviour_Move = 1,             //移动
        Behaviour_LookAtPos = 2,        //转向目标
        Behaviour_RotationBySelf = 3,   //自转
        Behaviour_DisplayText = 4,      //眼前文字
        Behaviour_ModelScale = 5,       //模型变化
        Behaviour_ModelChange = 6,      //模型更换
        behaviour_ModelDestroy = 7,     //删除模型
        Behaviour_Calculate = 8,        //数值运算

        Behaviour_PutOffTime = 9,       //时间推迟
        Behaviour_DoTheAction=  10,     //做动作
    }


    private ConditionToggle condition;   //记录选中的条件
    private BehaviourToggle behaviour;   //记录选中的行为

    private ManagerVars vars;
    private void Awake() {
        vars=ManagerVars.GetManagerVars();
    }

    //条件Toggle
    public void condition_EnterScenesToggle(){
        condition=ConditionToggle.Condition_EnterScenes;
        GameManager._instance.SelectCondition=(int)condition;
        GameManager._instance.SelectBehaviour=0;
        ShowConditionTips();
    }

    public void condition_WatchingModelToggle(){
        condition=ConditionToggle.Condition_WatchingModel;
        GameManager._instance.SelectCondition=(int)condition;
        GameManager._instance.SelectBehaviour=0;
        ShowConditionTips();
    }

    public void condition_ComputeTimeToggle(){
        condition=ConditionToggle.Condition_ComputeTime;
        GameManager._instance.SelectCondition=(int)condition;
        GameManager._instance.SelectBehaviour=0;
        ShowConditionTips();
    }

    public void condition_CompareVariableToggle(){
        condition=ConditionToggle.Condition_CompareVariable;
        GameManager._instance.SelectCondition=(int)condition;
        GameManager._instance.SelectBehaviour=0;
        ShowConditionTips();
    }

    //行为Toggle
    public void behaviour_MoveToggle(){
        behaviour=BehaviourToggle.Behaviour_Move;
        GameManager._instance.SelectBehaviour=(int)behaviour;
        GameManager._instance.SelectCondition=0;
        ShowBehaviourTips();
    }

    public void behaviour_LookAtPosToggle(){
        behaviour=BehaviourToggle.Behaviour_LookAtPos;
        GameManager._instance.SelectBehaviour=(int)behaviour;
        GameManager._instance.SelectCondition=0;
        ShowBehaviourTips();
    }


    public void behaviour_RotationBySelfToggle(){
        behaviour=BehaviourToggle.Behaviour_RotationBySelf;
        GameManager._instance.SelectBehaviour=(int)behaviour;
        GameManager._instance.SelectCondition=0;
        ShowBehaviourTips();
    }


    public void behaviour_DisplayTextToggle(){
        behaviour=BehaviourToggle.Behaviour_DisplayText;
        GameManager._instance.SelectBehaviour=(int)behaviour;
        GameManager._instance.SelectCondition=0;
        ShowBehaviourTips();
    }


    public void behaviour_ModelScaleToggle(){
        behaviour=BehaviourToggle.Behaviour_ModelScale;
        GameManager._instance.SelectBehaviour=(int)behaviour;
        GameManager._instance.SelectCondition=0;
        ShowBehaviourTips();
    }


    public void behaviour_ModelChangeToggle(){
        behaviour=BehaviourToggle.Behaviour_ModelChange;
        GameManager._instance.SelectBehaviour=(int)behaviour;
        GameManager._instance.SelectCondition=0;
        ShowBehaviourTips();
    }


    public void behaviour_ModelDestroyToggle(){
        behaviour=BehaviourToggle.behaviour_ModelDestroy;
        GameManager._instance.SelectBehaviour=(int)behaviour;
        GameManager._instance.SelectCondition=0;
        ShowBehaviourTips();
    }


    public void behaviour_CalculateToggle(){
        behaviour=BehaviourToggle.Behaviour_Calculate;
        GameManager._instance.SelectBehaviour=(int)behaviour;
        GameManager._instance.SelectCondition=0;
        ShowBehaviourTips();
    }

    public void behaviour_PutOffTimeToggle()
    {
        behaviour = BehaviourToggle.Behaviour_PutOffTime;
        GameManager._instance.SelectBehaviour = (int)behaviour;
        GameManager._instance.SelectCondition = 0;
        ShowBehaviourTips();
    }

    public void behaviour_DoTheActionToggle()
    {
        behaviour = BehaviourToggle.Behaviour_DoTheAction;
        GameManager._instance.SelectBehaviour = (int)behaviour;
        GameManager._instance.SelectCondition = 0;
        ShowBehaviourTips();
    }


    private void ShowConditionTips(){
        ConditionParam._instance.HideConditionPanel();
        BehaviourParam._instance.HideBehaviourPanel();
        EventCenter.Broadcast(EventDefine.ShowTipsPanel,vars.conditionList[(int)condition-1]);
    }

    private void ShowBehaviourTips(){
        ConditionParam._instance.HideConditionPanel();
        BehaviourParam._instance.HideBehaviourPanel();
        EventCenter.Broadcast(EventDefine.ShowTipsPanel,vars.behaviourList[(int)behaviour-1]);
    }



}
