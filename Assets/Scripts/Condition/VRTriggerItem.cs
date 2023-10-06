using UnityEngine;
using System.Collections;
using System;
using VRStandardAssets.Utils;
using UnityEngine.UI;

public class VRTriggerItem : MonoBehaviour
{
    private float speed = 2f;
    private bool isTrigger;
    private bool isSlider;
    private VRInteractiveItem vr;

    public event Action onOver;
    public event Action onOut;

    private GameObject rayGo;
    Ray ray;
    RaycastHit hit;

    public Text ceshi;
    void Awake()
    {
        ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        vr = GetComponent<VRInteractiveItem>() ?? gameObject.AddComponent<VRInteractiveItem>();
        ceshi = GameObject.Find("Ceshi").GetComponent<Text>();
    }
    void OnEnable()
    {
        vr.OnOver += OnOver;
        vr.OnOut += OnOut;
    }
    void OnDisable()
    {
        vr.OnOver -= OnOver;
        vr.OnOut -= OnOut;
    }
    void OnOver()
    {
        isSlider = false;
        onOver?.Invoke();
    }
    void OnOut()
    {
        isSlider = false;
        Ring.instance.ring.fillAmount = 0f;
        onOut?.Invoke();
    }
    void OnTrigger()
    {
        isSlider = true;
        Ring.instance.ring.fillAmount = 0f;
        Ring.instance.SetZXModel(rayGo);
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        isTrigger = vr.IsOver;
        ceshi.text = isTrigger.ToString();
        if (isTrigger && !isSlider)
        {
            if (Physics.Raycast(ray, out hit))
            {
                rayGo = hit.collider.gameObject;
                string ZXTimeStr = rayGo.transform.parent.GetComponent<ModelInformation>().ReturnId().ToString() + "2" + ConditionKey.WatchingModelTime;
                float timer = PlayerPrefs.GetFloat(ZXTimeStr);
                speed = timer;
                ceshi.text = rayGo.name;
            }
            Ring.instance.ring.fillAmount += Time.deltaTime / speed;

        }
        if (isTrigger && Ring.instance.ring.fillAmount == 1f)
            OnTrigger();
    }
}
