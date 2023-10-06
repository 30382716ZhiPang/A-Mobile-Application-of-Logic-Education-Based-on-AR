using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ModelInformation : MonoBehaviour
{
    private int id;//记录生成的模型编号

    private List<Func<object, string>> modelBehavioursList;
    private List<object> bhParameterList;

    public List<Func<object, string>> ModelBehavioursList { get => modelBehavioursList; set => modelBehavioursList = value; }
    public List<object> BhParameterList { get => bhParameterList; set => bhParameterList = value; }

    private Func<bool> ConditionFunc = null;
    

    //触发状态属性
    public bool IsNotTrigger = false;
    //调试状态属性
    public bool IsDebugging = false;

    CancellationTokenSource token;

    bool isDestory = false;

    //当前要执行的行为索引
    int behaviourCount = 0;
    //调试开始计时器
    float debugtime = 0f;

    //ConcurrentStack<Func<object, string>> stack;

    private void Awake()
    {
        ModelBehavioursList = new List<Func<object, string>>();
        BhParameterList = new List<object>();
        token = new CancellationTokenSource();
        startRotate = new Vector3(0, 180, 0);
        //stack = new ConcurrentStack<Func<object, string>>();
    }

    //查询model编号
    public int ReturnId(){
        return id;
    }

    public void SetId(int item){
        id=item;
    }

    //将模型的编号提取至InformationPanel中
    public void SaveIdInGameManager(){
        GameManager._instance.OnClickModelItem=id;
    }

    #region 条件状态方法
    //静态方法 开始状态
    private bool StartCondition()
    {
        Debug.Log("进入场景");
        return true;
    }
    //静态方法 准心触发
    private bool ZXTrigger()
    {
        return transform.GetChild(0).gameObject == Ring.instance.GetZXModel();
    }
    //静态方法 计时触发
    private bool TimeTrigger()
    {
        string ComputeTimeStr = ReturnId().ToString() + "3" + ConditionKey.ComputeTimeTime;
        
        float timer = PlayerPrefs.GetFloat(ComputeTimeStr);
        if (debugtime >= timer)
        {
            debugtime = timer;
            return true;
        }
        else
        {
            debugtime += Time.deltaTime;
            return false;
        }

    }
    //静态方法 数值比较
    private bool NumComparison()
    {
        Debug.Log("数值比较");
        string CompareVariableKeyStr = ReturnId().ToString() + "4" + ConditionKey.CompareVariableKey;
        string CompareVariableSymbolStr = ReturnId().ToString() + "4" + ConditionKey.CompareVariableSymbol;
        string CompareVariableValueStr = ReturnId().ToString() + "4" + ConditionKey.CompareVariableValue;
        int key = PlayerPrefs.GetInt(CompareVariableKeyStr);
        string symbol = PlayerPrefs.GetString(CompareVariableSymbolStr);
        int value = int.Parse(PlayerPrefs.GetString(CompareVariableValueStr));
        //Debug.Log(value);
        switch (symbol)
        {
            case "=":
                if (SetVariablePanel._instance.ReadVariableValue(key) == value)
                {
                    return true;
                }
                break;
            case ">":
                if (SetVariablePanel._instance.ReadVariableValue(key) > value)
                {
                    return true;
                }
                break;
            case "<":
                if (SetVariablePanel._instance.ReadVariableValue(key) < value)
                {
                    return true;
                }
                break;
            default:
                break;
        }
        return false;
    }
    public Func<bool> addCondition(int index)
    {
        switch (index)
        {
            case 1:
                return StartCondition;
            case 2:
                return ZXTrigger;
            case 3:
                return TimeTrigger;
            case 4:
                return NumComparison;
            default:
                return null;
        }
    }
    public void AddCondition(int GetConditionID)
    {
        if (ConditionFunc == null)
            ConditionFunc += addCondition(GetConditionID);
    }
    #endregion

    #region 模型行为方法
    //移动行为
    private string Move(object data)
    {
        Loom.RunAsync(() => {
            //Run some code on the main thread  
            //to update the mesh  
            Loom.QueueOnMainThread(() => {
                string MoveSpeedStr = ReturnId().ToString() + behaviourCount.ToString() + "0" + BehaviourKeyWord.MoveSpeed;
                string PosXStr = ReturnId().ToString()  + behaviourCount.ToString() + "0" + BehaviourKeyWord.MovePosX;
                string PosYStr = ReturnId().ToString()  + behaviourCount.ToString() + "0" + BehaviourKeyWord.MovePosY;
                string PosZStr = ReturnId().ToString()  + behaviourCount.ToString() + "0" + BehaviourKeyWord.MovePosZ;
                float x = PlayerPrefs.GetFloat(PosXStr);
                float y = PlayerPrefs.GetFloat(PosYStr);
                float z = PlayerPrefs.GetFloat(PosZStr);
                float speed = PlayerPrefs.GetFloat(MoveSpeedStr);
                Vector3 target = new Vector3(x, y, z);
                transform.forward = (target - transform.position).normalized;
                transform.GetComponentInChildren<Animator>().SetInteger("state", 1);
                transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed * 0.07f);
                if(transform.position == target)
                {
                    transform.GetComponentInChildren<Animator>().SetInteger("state", 0);
                    AddBehavioursCount();
                }
            });
        });
        return "移动完成";
    }
    //转向目标行为
    private string LookAtPos(object data)
    {
        Loom.RunAsync(() => {
            //Run some code on the main thread  
            //to update the mesh  
            Loom.QueueOnMainThread(() => {
                string LootAtPosXStr = ReturnId().ToString()  + behaviourCount.ToString() + "1" + BehaviourKeyWord.LootAtPosX;
                string LootAtPosYStr = ReturnId().ToString()  + behaviourCount.ToString() + "1" + BehaviourKeyWord.LootAtPosX;
                string LootAtPosZStr = ReturnId().ToString()  + behaviourCount.ToString() + "1" + BehaviourKeyWord.LootAtPosX;
                string LootAtPosSpeedStr = ReturnId().ToString()  + behaviourCount.ToString() + "1" + BehaviourKeyWord.LootAtSpeed;
                float x = PlayerPrefs.GetFloat(LootAtPosXStr);
                float y = PlayerPrefs.GetFloat(LootAtPosYStr);
                float z = PlayerPrefs.GetFloat(LootAtPosZStr);
                float speed = PlayerPrefs.GetFloat(LootAtPosSpeedStr);
                Vector3 target = new Vector3(x, y, z);
                transform.forward = Vector3.MoveTowards(transform.forward, (target - transform.position).normalized, Time.deltaTime * speed);
                if (Mathf.Acos(Vector3.Dot(transform.forward,(target - transform.position).normalized)) < 0.01f)
                {
                    AddBehavioursCount();
                }
                    
            });
        });
        return "OK";
    }
    //自转行为
    int circle = 0;
    public Vector3 startRotate;
    private string RotationBySelf(object data)
    {
        Loom.RunAsync(() => {
            //Run some code on the main thread  
            //to update the mesh  
            Loom.QueueOnMainThread(() => {
                string RotationBySelfCircleStr = ReturnId().ToString() + behaviourCount.ToString() + "2" + BehaviourKeyWord.RotationBySelfCircle;
                string RotationBySelfSpeedStr = ReturnId().ToString() + behaviourCount.ToString() + "2" + BehaviourKeyWord.RotationBySelfSpeed;
                int RotationBySelfCircle = (int)PlayerPrefs.GetFloat(RotationBySelfCircleStr);
                int RotationBySelfSpeed = (int)PlayerPrefs.GetFloat(RotationBySelfSpeedStr);
                transform.Rotate(Vector3.up * RotationBySelfSpeed);
                if (Mathf.Abs(transform.localEulerAngles.y - startRotate.y) <0.1f)
                    circle++;
                if(circle >= RotationBySelfCircle)
                    AddBehavioursCount();
            });
        });
        return "OK";
    }
    //眼前文字行为
    Text text = null;
    float texttime = 0f;
    private string DisplayText(object data)
    {
        Debug.Log("眼前文字进行");
        Loom.RunAsync(() => {
            //Run some code on the main thread  
            //to update the mesh  
            Loom.QueueOnMainThread(() => {
                string DisplayTextStr = ReturnId().ToString() + behaviourCount.ToString() + "3" + BehaviourKeyWord.DisplayTextText;
                string DisplayTextShowTimeStr = ReturnId().ToString() + behaviourCount.ToString() + "3" + BehaviourKeyWord.DisplayTextShowTime;
                //眼前文字的内容与展示时间
                string DisplayText = PlayerPrefs.GetString(DisplayTextStr);
                int DisplayTextShowTime = (int)PlayerPrefs.GetFloat(DisplayTextShowTimeStr);
                GameObject canvasObj = GameObject.Find("Canvas/ConditionParammeter");
                if (text == null)
                {
                    text = new GameObject("clone", typeof(Text)).GetComponent<Text>();
                    text.transform.parent = canvasObj.transform;
                    text.font = Resources.Load<Font>("Font/造字工房悦圆");
                    text.text = DisplayText;
                    text.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, canvasObj.transform.position.z);
                    text.fontSize = 60;
                    text.color = Color.black;
                    text.alignment = TextAnchor.MiddleCenter;
                    RectTransform rect = text.GetComponent<RectTransform>();
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1000);
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 800);
                }
                if (text != null)
                {
                    texttime += Time.deltaTime;
                    if (texttime > DisplayTextShowTime) {
                        Destroy(text.gameObject);
                        text = null;
                        AddBehavioursCount();
                    }
                }

            });
        });
        return "OK";
    }
    //模型变化行为
    private string ModelScale(object data)
    {
        Loom.RunAsync(() => {
            //Run some code on the main thread  
            //to update the mesh  
            Loom.QueueOnMainThread(() => {
                string ModelScaleScaleStr = ReturnId().ToString() + behaviourCount.ToString() + "4" + BehaviourKeyWord.ModelScaleScale.ToString();
                string ModelScaleSpeedStr = ReturnId().ToString() + behaviourCount.ToString() + "4" + BehaviourKeyWord.ModelScaleSpeed.ToString();
                float ModelScaleScale = PlayerPrefs.GetFloat(ModelScaleScaleStr); //变化大小为 0.5 1 2倍
                int ModelScaleSpeed = (int)PlayerPrefs.GetFloat(ModelScaleSpeedStr);
                transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(ModelScaleScale, ModelScaleScale, ModelScaleScale), Time.deltaTime * ModelScaleSpeed);
                if(transform.localScale == new Vector3(ModelScaleScale, ModelScaleScale, ModelScaleScale))
                {
                    AddBehavioursCount();
                }
            });
        });
        return "OK";
    }
    //模型更换行为
    private string ModelChange(object data)
    {
        Debug.Log("模型更换进行");
        Loom.RunAsync(() => {
            //Run some code on the main thread  
            //to update the mesh  
            Loom.QueueOnMainThread(() => {
                string ModelChangeStr = ReturnId().ToString() + behaviourCount.ToString() + "5" + BehaviourKeyWord.ModelChangeItem;
                int index = (int)PlayerPrefs.GetFloat(ModelChangeStr);
                ManagerVars vars = ManagerVars.GetManagerVars();
                //可调用 vars.ModelPrefabs[index] 来获取预制体
                Destroy(transform.GetChild(0).gameObject);
                GameObject model = Instantiate(vars.ModelPrefabs[index], transform.position, transform.rotation);
                model.transform.SetParent(transform);
                AddBehavioursCount();
            });
        });
        return "OK";
    }
    //删除模型行为
    private string ModelDestroy(object data)
    {
        isDestory = true;
        Debug.Log("删除模型进行");
        Loom.RunAsync(() => {
            //Run some code on the main thread  
            //to update the mesh  
            Loom.QueueOnMainThread(() => {
                Destroy(gameObject);
                AddBehavioursCount();
            });

        });
        return "OK";
    }
    //数值运算行为
    private string Calculate(object data)
    {
        Debug.Log("数值运算进行");
        Loom.RunAsync(() => {
            //Run some code on the main thread  
            //to update the mesh  
            Loom.QueueOnMainThread(() => {
                string CalculateKeyStr = ReturnId().ToString() + behaviourCount.ToString() + "7" + BehaviourKeyWord.CalculateKey;
                string CalculateSymbolStr = ReturnId().ToString() + behaviourCount.ToString() + "7" + BehaviourKeyWord.CalculateSymbol;
                string CalculateValueStr = ReturnId().ToString() + behaviourCount.ToString() + "7" + BehaviourKeyWord.CalculateValue;
                int Value = PlayerPrefs.GetInt(CalculateKeyStr);                  //存在数据库中的值
                int value = int.Parse(PlayerPrefs.GetString(CalculateValueStr));  //需要进行运算的值
                string symbol = PlayerPrefs.GetString(CalculateSymbolStr);
                switch (symbol)
                {
                    case "+":
                        SetVariablePanel._instance.SaveVariableInformation(Value, CalculateKeyStr, SetVariablePanel._instance.ReadVariableValue(Value) + value);
                        break;
                    case "-":
                        SetVariablePanel._instance.SaveVariableInformation(Value, CalculateKeyStr, SetVariablePanel._instance.ReadVariableValue(Value) - value);
                        break;
                    case "*":
                        SetVariablePanel._instance.SaveVariableInformation(Value, CalculateKeyStr, SetVariablePanel._instance.ReadVariableValue(Value) * value);
                        break;
                    case "/":
                        SetVariablePanel._instance.SaveVariableInformation(Value, CalculateKeyStr, SetVariablePanel._instance.ReadVariableValue(Value) / value);
                        break;
                    default:
                        break;
                }
                AddBehavioursCount();
            });
        });
        return "OK";
    }
    float putofftimer = 0f;
    //时间推迟行为
    private string PutOffTime(object data)
    {
        Loom.RunAsync(() => {
            //Run some code on the main thread  
            //to update the mesh  
            Loom.QueueOnMainThread(() => {
                string PutOffTimeStr = ReturnId().ToString() + behaviourCount.ToString() + "8" + BehaviourKeyWord.PutOffTimeTime;
                float timer = PlayerPrefs.GetFloat(PutOffTimeStr);
                putofftimer += Time.deltaTime;
                if(putofftimer > timer)
                {
                    AddBehavioursCount();
                }
                
            });
        });
        return "OK";
    }
    float animtimer = 0f;
    float cliptime = 0f;
    //做动作行为
    private string DoTheAction(object data)
    {
        Loom.RunAsync(() => {
            //Run some code on the main thread  
            //to update the mesh  
            Loom.QueueOnMainThread(() => {
                string DoToActionItemStr = ReturnId().ToString() + behaviourCount.ToString() + "9" + BehaviourKeyWord.DoToActionItem;
                switch (PlayerPrefs.GetString(DoToActionItemStr))
                {
                    case "跳舞":
                        transform.GetComponentInChildren<Animator>().SetInteger("state", 2);
                        cliptime = 15.967f;
                        break;
                    case "跑步":
                        transform.GetComponentInChildren<Animator>().SetInteger("state", 3);
                        cliptime = 5f;
                        break;
                    case "太极":
                        transform.GetComponentInChildren<Animator>().SetInteger("state", 4);
                        cliptime = 7.467f;
                        break;
                    case "讨论":
                        transform.GetComponentInChildren<Animator>().SetInteger("state", 5);
                        cliptime = 7.833f;
                        break;
                    default:
                        break;
                }
                animtimer += Time.deltaTime;
                if (animtimer > cliptime)
                {
                    transform.GetComponentInChildren<Animator>().SetInteger("state", 0);
                    AddBehavioursCount();
                }
                
            });
        });
        return "OK";
    }
    //返回对应行为编号的行为
    private Func<object, string> ReturnModelBehaviour(int index)
    {
        switch (index)
        {
            case 1:
                return Move;
            case 2:
                return LookAtPos;
            case 3:
                return RotationBySelf;
            case 4:
                return DisplayText;
            case 5:
                return ModelScale;
            case 6:
                return ModelChange;
            case 7:
                return ModelDestroy;
            case 8:
                return Calculate;
            case 9:
                return PutOffTime;
            case 10:
                return DoTheAction;
            default:
                return null;
        }
    }

    public void AddModelBehaviour(int GetBehaviourID)
    {
        ModelBehavioursList.Add(ReturnModelBehaviour(GetBehaviourID));
    }
    #endregion

    //记录调试前的状态，如移动、旋转等等
    //public void RecordState()
    //{

   // }
    //将记录的状态还原，并将触发调试状态归为默认
    public void ResetState()
    {
        IsDebugging = false;
        IsNotTrigger = true;
        behaviourCount = 0;
        debugtime = 0f;
        texttime = 0f;
        putofftimer = 0f;
        animtimer = 0f;
        cliptime = 0f;
        //token = new CancellationTokenSource();
    }
    
    void AddBehavioursCount()
    {
        if (behaviourCount < ModelBehavioursList.Count-1)
            behaviourCount++;
        else
            ResetState();
    }
    void Update()
    {
        //判断调试状态、条件触发状态
        //将调试状态改为false
        //执行记录函数
        //开始建立线程任务
        if (IsNotTrigger && IsDebugging && ConditionFunc())
        {
            //RecordState();
            Debug.Log("222");
            ModelInformation modelInfo = gameObject.GetComponent<ModelInformation>();
            Debug.Log(modelInfo.gameObject);
            Task<string> task = null;
            task = Task.Factory.StartNew(ModelBehavioursList[behaviourCount], "", token.Token); 
        }
    }
}
