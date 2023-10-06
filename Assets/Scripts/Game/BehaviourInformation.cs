using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourInformation : MonoBehaviour
{
    private ManagerVars vars;
    private Button BehaviourButotn;
    private Text txt_name;
    private int ModelItem;  //记录生成的模型编号
    private int id;         //记录生成的行为编号
    private int Item;       //记录生成的行为类型

    private void Awake() {
        EventCenter.AddListener<bool>(EventDefine.SaveBehaviourPanel,SaveBehaviourOnClick);
        EventCenter.AddListener(EventDefine.DelectBehaviourPanel,DelectBehaviourOnClick);
        UpdateAllValue();
    }
    private void OnDestroy() {
        EventCenter.RemoveListener<bool>(EventDefine.SaveBehaviourPanel,SaveBehaviourOnClick);
        EventCenter.RemoveListener(EventDefine.DelectBehaviourPanel,DelectBehaviourOnClick);
    }

    private void UpdateAllValue(){
        vars=ManagerVars.GetManagerVars();
        txt_name=transform.Find("txt_name").GetComponent<Text>();
        BehaviourButotn=gameObject.GetComponent<Button>();
        BehaviourButotn.onClick.AddListener(BehaviourButtonOnClick);
    }
    
    //查询Behaviour编号
    public int ReturnId(){
        return id;
    }
    //查询Behaviour的Model编号
    public int ReturnModelItem(){
        return ModelItem;
    }

    public void SaveData(int item,int behaviourId){
        ModelItem=item;
        id=behaviourId;
        Item=GameManager._instance.ReturnModelBehaviourData(ModelItem,id)-1;
        
        //实例化后如果行为已赋值则修改样式
        if(GameManager._instance.ReturnModelBehaviourData(ModelItem,id)!=0){
            UpdateAllValue();
        
            txt_name.gameObject.SetActive(false);
            transform.Find("Image").gameObject.SetActive(true);
            transform.Find("Image").GetChild(0).gameObject.GetComponent<Image>().sprite
                =vars.behaviourSprite[Item];
        }
    }

    //将模型的编号提取至InformationPanel中
    private void BehaviourButtonOnClick(){
        //记录当前选中的行为
        GameManager._instance.OnClickBehaviourItem=id;

        //Debug 当按钮按下时，显示当前Item的值(0)，id(点击的为第几个行为)，SelectBehaviour(当前选中的行为索引值)，
        // Debug.Log("Model Item(模型编号) = "+ModelItem+"\n"+"Behaviour Id(当前选中的行为编号) = "+id +"\n"
        //     +"存储在Data下的Id = "+GameManager._instance.ReturnModelBehaviourData(ModelItem,id)+"\n"+"SelectModel = "+GameManager._instance.SelectBehaviour);

        //DOTO 显示行为参数
        if(GameManager._instance.ReturnModelBehaviourData(ModelItem,id)!=0){
            GameManager._instance.NowSelectBehaviour=id;
            BehaviourParam._instance.ShowBehaviourPanel(
                GameManager._instance.ReturnModelBehaviourData(ModelItem,id)-1);
        }

        //DOTO 进行判断模型行为是否为空，如果空则添加行为，否则显示行为参数
        if(GameManager._instance.SelectBehaviour!=0&&GameManager._instance.SelectCondition==0){
            if(GameManager._instance.ReturnBehaviourHaveDataCount(ModelItem)<GameManager._instance.ReturnBehaviourCount(ModelItem)){//当行为还有空位时
                //将行为 按顺序 存进模型的结构体中
                GameManager._instance.AddBehaviourData(ModelItem,GameManager._instance.SelectBehaviour);

                ModelInformation modelInfo = GameManager._instance.ReturnModelInformation(ModelItem);
                modelInfo.AddModelBehaviour(GameManager._instance.SelectBehaviour);

                txt_name.gameObject.SetActive(false);
                transform.Find("Image").gameObject.SetActive(true);
                transform.Find("Image").GetChild(0).gameObject.GetComponent<Image>().sprite
                =vars.behaviourSprite[GameManager._instance.ReturnModelBehaviourData(ModelItem,id)-1];
            
                //隐藏条件参数面板
                BehaviourParam._instance.HideBehaviourPanel();

                //添加一个Behaviour空格
                EventCenter.Broadcast(EventDefine.AddBehaviourButtonDown);
            }

            //清空功能面板的索引值
            GameManager._instance.SelectBehaviour=0;
            GameManager._instance.SelectCondition=0;
        }
    }


    //保存行为面板上的行为
    private void SaveBehaviourOnClick(bool UpdateData){
        //id表示该模型的第几个行为
        //GameManager._instance.ReturnModelBehaviourData(ModelItem,id)-1 表示类型 0代表移动，1代表转向目标... 
        if(GameManager._instance.NowSelectBehaviour==id){
            switch(Item){//如何获取数据  Move._instacne.ReturnSpeed;      0 0 0 0  模型编号 行为编号 行为类型 行为参数
                case 0: //移动
                        string MoveSpeedStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.MoveSpeed;
                        string MovePosXStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.MovePosX; 
                        string MovePosYStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.MovePosY;
                        string MovePosZStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.MovePosZ;
                        if(UpdateData==true){
                            PlayerPrefs.SetFloat(MoveSpeedStr,Move._instance.ReturnSpeed());
                        }else{
                            if(PlayerPrefs.HasKey(MoveSpeedStr)){
                                PlayerPrefs.DeleteKey(MoveSpeedStr);
                            }else{
                                PlayerPrefs.SetFloat(MoveSpeedStr,1);
                            }
                            if(PlayerPrefs.HasKey(MovePosXStr)&&PlayerPrefs.HasKey(MovePosYStr)&&PlayerPrefs.HasKey(MovePosZStr)){
                                PlayerPrefs.DeleteKey(MovePosXStr);
                                PlayerPrefs.DeleteKey(MovePosYStr);
                                PlayerPrefs.DeleteKey(MovePosZStr);
                            }else{
                                PlayerPrefs.SetFloat(MovePosXStr,0);
                                PlayerPrefs.SetFloat(MovePosYStr,0);
                                PlayerPrefs.SetFloat(MovePosZStr,0);
                            }
                        }

                        Debug.Log("Set = "+PlayerPrefs.GetFloat(ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.MoveSpeed));
                break;
                case 1: //转向目标
                        string LootAtPosSpeedStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.LootAtSpeed;
                        string LootAtPosXStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.LootAtPosX; 
                        string LootAtPosYStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.LootAtPosY;
                        string LootAtPosZStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.LootAtPosZ;
                        if(UpdateData==true){
                            PlayerPrefs.SetFloat(LootAtPosSpeedStr,LootAtPos._instance.ReturnSpeed());
                        }else{
                            if(PlayerPrefs.HasKey(LootAtPosSpeedStr)){
                                PlayerPrefs.DeleteKey(LootAtPosSpeedStr);
                            }else{
                                PlayerPrefs.SetFloat(LootAtPosSpeedStr,1);
                            }
                            if(PlayerPrefs.HasKey(LootAtPosXStr)&&PlayerPrefs.HasKey(LootAtPosYStr)&&PlayerPrefs.HasKey(LootAtPosZStr)){
                                PlayerPrefs.DeleteKey(LootAtPosXStr);
                                PlayerPrefs.DeleteKey(LootAtPosYStr);
                                PlayerPrefs.DeleteKey(LootAtPosZStr);
                            }else{
                                PlayerPrefs.SetFloat(LootAtPosXStr,0);
                                PlayerPrefs.SetFloat(LootAtPosYStr,0);
                                PlayerPrefs.SetFloat(LootAtPosZStr,0);
                            }
                        }
                break;
                case 2: //自转
                        string RotationBySelfCircleStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.RotationBySelfCircle;
                        string RotationBySelfSpeedStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.RotationBySelfSpeed;
                        if(UpdateData==true){
                            PlayerPrefs.SetFloat(RotationBySelfCircleStr,RotationBySelf._instance.ReturnCircle());
                            PlayerPrefs.SetFloat(RotationBySelfSpeedStr,RotationBySelf._instance.ReturnSpeed());
                        }else{
                            if(PlayerPrefs.HasKey(RotationBySelfCircleStr)){
                                PlayerPrefs.DeleteKey(RotationBySelfCircleStr);
                            }else{
                                PlayerPrefs.SetFloat(RotationBySelfCircleStr,1);  
                            }
                            if(PlayerPrefs.HasKey(RotationBySelfSpeedStr)){
                                PlayerPrefs.DeleteKey(RotationBySelfSpeedStr);
                            }else{
                                PlayerPrefs.SetFloat(RotationBySelfSpeedStr,1);  
                            }
                        }
                break;
                case 3: //眼前文字
                        string DisplayTextStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.DisplayTextText;
                        string DisplayTextShowTimeStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.DisplayTextShowTime;
                        if(UpdateData==true){
                            PlayerPrefs.SetString(DisplayTextStr,DisplayText._instance.ReturnText());
                            PlayerPrefs.SetFloat(DisplayTextShowTimeStr,DisplayText._instance.ReturnShowTime());
                        }else{
                            if(PlayerPrefs.HasKey(DisplayTextStr)){
                                PlayerPrefs.DeleteKey(DisplayTextStr);
                            }else{
                                PlayerPrefs.SetString(DisplayTextStr,"");  
                            }
                            if(PlayerPrefs.HasKey(DisplayTextShowTimeStr)){
                                PlayerPrefs.DeleteKey(DisplayTextShowTimeStr);
                            }else{
                                PlayerPrefs.SetFloat(DisplayTextShowTimeStr,1);  
                            }
                        }
                break;
                case 4: //模型变化
                        string ModelScaleScaleStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.ModelScaleScale.ToString();
                        string ModelScaleSpeedStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.ModelScaleSpeed.ToString();
                        if(UpdateData==true){
                            PlayerPrefs.SetFloat(ModelScaleScaleStr,ModelScale._instance.ReturnScale());
                            PlayerPrefs.SetFloat(ModelScaleSpeedStr,ModelScale._instance.ReturnSpeed());
                        }else{
                            if(PlayerPrefs.HasKey(ModelScaleScaleStr)){
                                PlayerPrefs.DeleteKey(ModelScaleScaleStr);
                            }else{
                                PlayerPrefs.SetFloat(ModelScaleScaleStr,0.5f);  
                            }
                            if(PlayerPrefs.HasKey(ModelScaleSpeedStr)){
                                PlayerPrefs.DeleteKey(ModelScaleSpeedStr);
                            }else{
                                PlayerPrefs.SetFloat(ModelScaleSpeedStr,1);  
                            }
                        }
                break;
                case 5: //更换模型
                        string ModelChangeStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.ModelChangeItem;
                        if(UpdateData==true){
                            PlayerPrefs.SetFloat(ModelChangeStr,ModelChange._instance.ReturnIndex());
                        }else{
                            if(PlayerPrefs.HasKey(ModelChangeStr)){
                                PlayerPrefs.DeleteKey(ModelChangeStr);
                            }else{
                                PlayerPrefs.SetFloat(ModelChangeStr,0);
                            }
                        }
                break;
                case 6: //删除模型
                        string ModelDestroyStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.ModelDestroy;
                        if(UpdateData==true){
                            PlayerPrefs.SetFloat(ModelDestroyStr,1);
                        }else{
                            if(PlayerPrefs.HasKey(ModelDestroyStr)){
                                PlayerPrefs.DeleteKey(ModelDestroyStr);
                            }else{
                                PlayerPrefs.SetFloat(ModelDestroyStr,1);
                            }
                        }
                break;
                case 7: //数值运算
                        string CalculateKeyStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.CalculateKey;
                        string CalculateSymbolStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.CalculateSymbol;
                        string CalculateValueStr=ModelItem.ToString()+id.ToString()+Item.ToString()+BehaviourKeyWord.CalculateValue;
                        if(UpdateData==true){
                            PlayerPrefs.SetInt(CalculateKeyStr,Calculate._instance.ReturnKey());
                            PlayerPrefs.SetString(CalculateSymbolStr,Calculate._instance.ReturnSymbol());
                            PlayerPrefs.SetString(CalculateValueStr,Calculate._instance.ReturnValue());
                        }else{
                            if(PlayerPrefs.HasKey(CalculateKeyStr)){
                                PlayerPrefs.DeleteKey(CalculateKeyStr);
                            }else{
                                PlayerPrefs.SetInt(CalculateKeyStr,Calculate._instance.ReturnKey());
                            }
                            if(PlayerPrefs.HasKey(CalculateSymbolStr)){
                                PlayerPrefs.DeleteKey(CalculateSymbolStr);
                            }else{
                                PlayerPrefs.SetString(CalculateSymbolStr,Calculate._instance.ReturnSymbol());
                            }
                            if(PlayerPrefs.HasKey(CalculateValueStr)){
                                PlayerPrefs.DeleteKey(CalculateValueStr);
                            }else{
                                PlayerPrefs.SetString(CalculateValueStr,Calculate._instance.ReturnValue());
                            }
                        }
                break;
                case 8://时间推迟
                    string PutOffTimeStr = ModelItem.ToString() + id.ToString() + Item.ToString() + BehaviourKeyWord.PutOffTimeTime;
                    if (UpdateData == true)
                    {
                        PlayerPrefs.SetFloat(PutOffTimeStr, PutOffTime._instance.ReturnTime());
                    }
                    else
                    {
                        if (PlayerPrefs.HasKey(PutOffTimeStr))
                        {
                            PlayerPrefs.DeleteKey(PutOffTimeStr);
                        }
                        else
                        {
                            PlayerPrefs.SetFloat(PutOffTimeStr, PutOffTime._instance.ReturnTime());
                        }
                    }
                    break;
                case 9://做动作
                    string DoToActionItemStr = ModelItem.ToString() + id.ToString() + Item.ToString() + BehaviourKeyWord.DoToActionItem;
                    if (UpdateData == true)
                    {
                        PlayerPrefs.SetString(DoToActionItemStr, DoToAction._instance.ReturnAction());
                    }
                    else
                    {
                        if (PlayerPrefs.HasKey(DoToActionItemStr))
                        {
                            PlayerPrefs.DeleteKey(DoToActionItemStr);
                        }
                        else
                        {
                            PlayerPrefs.SetString(DoToActionItemStr, DoToAction._instance.ReturnAction());
                        }
                    }
                    break;
                default:
                break;
            }
        }
    }
    //清除行为面板上的行为
    private void DelectBehaviourOnClick(){
        if(GameManager._instance.NowSelectBehaviour==id){
            GameManager._instance.ReduceBehaviourData(ModelItem,id);
            //清空多余的条件格子
            GameManager._instance.DelectBehaviourCount(ModelItem);

            //刷新信息面板
            EventCenter.Broadcast(EventDefine.ShowInformationPanel);

            //隐藏行为参数面板
            BehaviourParam._instance.HideBehaviourPanel();
        }
    }
}
