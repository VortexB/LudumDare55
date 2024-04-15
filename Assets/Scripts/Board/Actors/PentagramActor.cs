using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PentagramActor : BoardActor
{


    Animator anim;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }



    protected override bool MovedApon(BoardActor actor)
    {
        // if(actor is PlayerActor){
        //     BoardController.instance.CheckExtraordinaryCondition();
        // }
        return true;
    }

    protected override bool CanMoveApon(BoardActor actor)
    {
        return true;
    }
}
