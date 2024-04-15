using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeftBoardBehavior : ActorBehavior
{
    public override void StartTurn(Action<ActorBehavior> endTurnFunc){
        base.StartTurn(endTurnFunc);
        StartCoroutine(MoveAnim());
    }
    public override void EndTurn(){

        base.EndTurn();
    }

    IEnumerator MoveAnim(){
        yield return new WaitForSeconds(1);
        
        if(Actor.CanMove(Actor.BoardPosition+Vector2Int.left))
            Actor.Move(Actor.BoardPosition+Vector2Int.left);
        EndTurn();
    }
}
