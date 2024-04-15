using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ActorBehavior : MonoBehaviour
{
    [SerializeField] public int initiative;
    Action<ActorBehavior> EndTurnFunc;
    protected BoardActor Actor { get =>gameObject.GetComponent<BoardActor>();}
    public virtual void StartTurn(Action<ActorBehavior> endTurnFunc){
        this.EndTurnFunc = endTurnFunc;
    }
    public virtual void EndTurn(){
        EndTurnFunc(this);
    }
}
