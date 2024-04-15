using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerActor : BoardActor
{

    [Header("Player Actor")]
    [SerializeField] public Animator anim;
    [SerializeField] public int summonCount= 1;

    protected Stack<int> summonCountSnapshots = new Stack<int>();


    protected override void Start()
    {
        base.Start();
        //    anim = GetComponent<Animator>();
        anim = GetComponentInChildren<Animator>();


    }

    public override void ActorStartup(BoardProperties prop)
    {
        base.ActorStartup(prop);
        anim = GetComponentInChildren<Animator>();
    }



    public bool PlayerMove(Vector2Int deltaPosition)
    {
        if(!CanMove(BoardPosition + deltaPosition))return false;
        bool moveResult = base.Move(BoardPosition + deltaPosition);
        if (moveResult)
        {
            if (deltaPosition.x > 0)//the same for other directions
            {
                //play anim right
            }
        }
        return moveResult;
    }


    public override void CreateSnapshot()
    {
        base.CreateSnapshot();
        summonCountSnapshots.Push(summonCount);

    }
    public override void PopSnapshot()
    {
        summonCount = summonCountSnapshots.Pop();
        base.PopSnapshot();
    }

    protected override bool MovedApon(BoardActor actor)
    {

        return false;
    }

    protected override bool CanMoveApon(BoardActor actor)
    {
        if (actor is ObstacleActor || actor is SkeletonActor)
        {
            return false;
        }
            return true;
    }
}

