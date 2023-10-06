using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Ring : MonoBehaviour
{
    [HideInInspector]
    public static Ring instance;
    [HideInInspector]
    public bool isSlider;
    [HideInInspector]
    public Image ring;

    private GameObject ZXModel;

    public GameObject GetZXModel()
    {
        return ZXModel;
    }

    public void SetZXModel(GameObject go)
    {
        ZXModel = go;
    }
    void Awake()
    {
        ring = GetComponent<Image>();
        instance = this;
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (ring.fillAmount == 1f)
            isSlider = true;
        else
            isSlider = false;
    }
}
