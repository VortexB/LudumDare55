using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BasicSkeletonBehavior : ActorBehavior
{
    SkeletonActor skeletonActor;

    private void Start() {
        skeletonActor = Actor.GetComponent<SkeletonActor>();
    }
    

    public override void StartTurn(Action<ActorBehavior> endTurnFunc){
        if(skeletonActor==null)skeletonActor = Actor.GetComponent<SkeletonActor>();
        base.StartTurn(endTurnFunc);

        //check sigil
        if(skeletonActor.OnSigil!=null){
            if(skeletonActor.State==SkeletonActor.SkeletonState.Bones){
                skeletonActor.OnSigil.Activated = true;
                skeletonActor.State = SkeletonActor.SkeletonState.Sigiled;
            }
        }


        switch(skeletonActor.State){
            case SkeletonActor.SkeletonState.Summoned:
                //play anim
                // skeletonActor.anim.SetTrigger("Revive");

                //end turn
                skeletonActor.State = SkeletonActor.SkeletonState.Moving;
                EndTurn();
            break;
            case SkeletonActor.SkeletonState.Moving:
                //move one in direction
                ProcessMove();
                

            break;
            case SkeletonActor.SkeletonState.Bones:
            //do nothing?

            EndTurn();
            break;
            case SkeletonActor.SkeletonState.Sigiled:
            //do nothing?
            skeletonActor.anim.SetTrigger("Ascend");

            EndTurn();
            break;
        }
    }

    private void Update() {
        if(Actor.visualMoving){
            skeletonActor.anim.SetBool("Moving",true);
        }else{
            skeletonActor.anim.SetBool("Moving",false);
        }
    }

    private void ProcessMove()
    {
        if(Actor.CanMove(Actor.BoardPosition+skeletonActor.Direction)){
            StartCoroutine(MoveAnim(skeletonActor.Direction));
            if(skeletonActor.OnSigil!=null){
                skeletonActor.OnSigil=null;
            }
        }
        else{
            //anim this
            skeletonActor.State = SkeletonActor.SkeletonState.Bones;
            skeletonActor.anim.SetTrigger("Crumble");

            EndTurn();
        }

    }

    public override void EndTurn(){

        base.EndTurn();
    }

    IEnumerator MoveAnim(Vector2Int direction){
        yield return new WaitForSeconds(0.5f);
        
        if(Actor.CanMove(Actor.BoardPosition+direction))
            Actor.Move(Actor.BoardPosition+direction);
        EndTurn();
    }
}
