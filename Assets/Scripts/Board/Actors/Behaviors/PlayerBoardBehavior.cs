using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoardBehavior : ActorBehavior
{
    bool playerInputEnabled;
    PlayerActor playerActor;
    [SerializeField] PlayerSkeletonSummoner playerSkeletonSummoner;
    [SerializeField] Camera mainCam;
    bool summonViewOn;

    private void Start()
    {
        playerActor = GetComponent<PlayerActor>();
        playerSkeletonSummoner = GetComponent<PlayerSkeletonSummoner>();
        if (mainCam == null) mainCam = Camera.main;
    }

    private void Update()
    {
        if (playerInputEnabled)
        {
            GetInputs();
        }
    }
    private void GetInputs()
    {
        if (playerActor.visualMoving) return;
        if(!summonViewOn){
            if (Input.GetKeyDown(KeyCode.E))
            {
                MovePlayer(Vector2Int.zero);//change this
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                MovePlayer(TranslateCamDirection(Vector2Int.right));
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                MovePlayer(TranslateCamDirection(Vector2Int.left));
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                MovePlayer(TranslateCamDirection(Vector2Int.up));
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                MovePlayer(TranslateCamDirection(Vector2Int.down));
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                PlayerWantsUndo();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space)){
            ToggleSummonView();
        }
        if (Input.GetKeyDown(KeyCode.R))
            {
                GameController.instance.RestartLevel();
            }

    }
    void ToggleSummonView(){
        if(summonViewOn){
            playerSkeletonSummoner.ToggleSummonViewOff();
            summonViewOn = false;
        }else{
            playerSkeletonSummoner.ToggleSummonViewOn(playerActor.BoardPosition,playerActor.summonCount,(bool newSummon)=>{
                EndTurn();
                summonViewOn = false;
                if(newSummon)
                    playerActor.summonCount--;
                //play anim
            });
            summonViewOn = true;
        }
    }
    private Vector2Int TranslateCamDirection(Vector2Int inputDir)
    {

        float forwardMag = Vector3.Magnitude(mainCam.transform.forward - Vector3.forward);
        float backMag = Vector3.Magnitude(mainCam.transform.forward - Vector3.back);
        float leftMag = Vector3.Magnitude(mainCam.transform.forward - Vector3.left);
        float rightMag = Vector3.Magnitude(mainCam.transform.forward - Vector3.right);
        float min = Math.Min(forwardMag,
                    Math.Min(backMag,
                    Math.Min(leftMag,
                    rightMag)));
        switch (min)
        {
            case float m when (m == forwardMag):
                return new Vector2Int(inputDir.x,inputDir.y);

            case float m when (m == backMag):
                return new Vector2Int(-inputDir.x,-inputDir.y);

            case float m when (m == leftMag):
                return new Vector2Int(-inputDir.y,inputDir.x);

            case float m when (m == rightMag):
                return new Vector2Int(inputDir.y,-inputDir.x);

            default:
                return inputDir;
        }
    }

    private void PlayerWantsUndo()
    {
        Actor.board.BoardProperties.ReverseState();
    }

    void MovePlayer(Vector2Int direction)
    {

        Actor.board.BoardProperties.SaveState();
        bool successfulMovement = playerActor.PlayerMove(direction);
        if (!successfulMovement)
        {
            Actor.board.BoardProperties.ReverseState();
            return;
        }

        EndTurn();
    }
    public override void StartTurn(Action<ActorBehavior> endTurnFunc)
    {
        base.StartTurn(endTurnFunc);
        playerInputEnabled = true;
    }
    public override void EndTurn()
    {
        playerInputEnabled = false;
        base.EndTurn();
    }

}
