using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


public class RdPlanePanel : MonoBehaviour
{
    private Button RdPlane;//按钮
    private Text RdPlaneBtntext;//扫描平面/停止扫描
    private bool isRdPlane;//是否启用扫描

    private void Awake()
    {
        RdPlane = transform.Find("switchRdPlane").GetComponent<Button>();
        RdPlane.onClick.AddListener(ReadPlaneOnClick);

        RdPlaneBtntext = transform.Find("switchRdPlane/Text").GetComponent<Text>();

        isRdPlane = false;//是否开启平面识别 
    }

    private void ReadPlaneOnClick()
    {
        if (!isRdPlane)
        {
            //扫描时按钮值
            RdPlaneBtntext.text = "停止扫描";
            GameManager._instance.DisPlane = true;
        }
        else
        {
            //未扫描时按钮值
            RdPlaneBtntext.text = "扫描平面";
        }

        isRdPlane = !isRdPlane;
        RdEnable(isRdPlane);
    }

    //控制ARPointCloudManager/ARPlaneManager两个脚本的启用状态
    private void RdEnable(bool isenable)
    {
        ARManager.This.gameObject.GetComponent<ARPointCloudManager>().enabled = isenable;
        ARManager.This.gameObject.GetComponent<ARPlaneManager>().enabled = isenable;
    }
}
