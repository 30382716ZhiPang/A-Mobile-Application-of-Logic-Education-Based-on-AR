using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARManager : MonoBehaviour
{
    static private ARManager _this;
    static public ARManager This
    {
        get {
            return _this;
        }
    }
    void Start()
    {
        _this = this;
        Debug.Log(_this.gameObject.name);
    }
}
