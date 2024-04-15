using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BoardController : MonoBehaviour
{
    public static BoardController instance;
    public BoardProperties currentBoardProperties;
    public event Action<BoardProperties> OnBoardLinked;//board will be in its prestartup state
    public event Action<BoardProperties> OnBoardUnlinked;

    private void Awake()
    {
        if (instance) Destroy(this);
        else instance = this; 
    }

    private void Update()
    {
        if (currentBoardProperties != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {

                currentBoardProperties.BoardReset();

            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (currentBoardProperties.State == BoardPlayState.Playing)
                {
                    currentBoardProperties.ReverseState();
                }
            }
        }
    }
    public void StartBoard(BoardProperties board)
    {
        currentBoardProperties = board;
        OnBoardLinked?.Invoke(board);
        board.OnPlayStateChanged += OnBoardStateChange;
        board.BoardStartup();
    }

    private void OnBoardStateChange(BoardPlayState state)
    {
        switch (state)
        {
            case BoardPlayState.Starting:
                PlayBoardStartAnimation();
                break;
            case BoardPlayState.Ending:
                currentBoardProperties.OnPlayStateChanged -= OnBoardStateChange;
                OnBoardUnlinked?.Invoke(currentBoardProperties);
                currentBoardProperties = null;
                break;

            default:
                return;
        }
    }

    void PlayBoardStartAnimation()
    {
        //play some cool animation idk man
        currentBoardProperties.State = BoardPlayState.Playing;
    }

    public bool CheckExtraordinaryCondition(){
        //if win state true
        return CheckSigilCompletion();
        // return false;
    }

    private bool CheckSigilCompletion()
    {
        bool allSigilsActive=true;
        bool playerOnPentagram =true;

        foreach(var actor in currentBoardProperties.Board.GetActors()){
            if(actor is SigilActor){
                if(!((SigilActor)actor).activated)
                    allSigilsActive = false;
            }
            if(actor is PentagramActor){
                if(currentBoardProperties.Board.GetActors<PlayerActor>()[0].BoardPosition!=actor.BoardPosition){
                    playerOnPentagram = false;
                }
            }
        }
        if(allSigilsActive && playerOnPentagram){
            currentBoardProperties.Board.GetActors<PlayerActor>()[0].anim.SetTrigger("Ascend");
            Invoke(nameof(LevelControllerComplete),2);
            return true;
        }
        return false;
    }
    void LevelControllerComplete(){
        LevelController.instance.CompleteLevel();
    }
}
