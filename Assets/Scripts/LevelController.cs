using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelController : MonoBehaviour
{
    public static LevelController instance;
    [SerializeField] GameObject LevelCompleteUI;
    [SerializeField] bool autoStart = false;
    [SerializeField] BoardProperties board;

    private void Awake()
    {
        if (instance) Destroy(this);
        else instance = this; 
    }

    private void Start() {
        Invoke(nameof(DelayStart),0.5f);
    }
    void DelayStart(){
        if(autoStart){
            BoardController.instance.StartBoard(board);

        }
    }
    public void CompleteLevel(){
        print("you win :)");
        LevelCompleteUI.SetActive(true);
    }

    
}
