using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] bool effectEnabled = true;
    [SerializeField] bool billX = true;
    [SerializeField] Camera billCamera;
    // Update is called once per frame

    private void Start() {
        if(!billCamera)billCamera = Camera.main;
    }
    void Update()
    {
        if(effectEnabled)
            transform.rotation = Quaternion.Euler(billX ? billCamera.transform.rotation.eulerAngles.x:0f,billCamera.transform.rotation.eulerAngles.y,0f);
    }
}
