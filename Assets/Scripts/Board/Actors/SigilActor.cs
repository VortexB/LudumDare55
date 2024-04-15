using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SigilActor : BoardActor
{
    public bool activated;

    protected Stack<bool> activatedSnapshots = new Stack<bool>();


    public bool Activated
    {
        get => activated;
        set
        {
            activated = value;
            Invoke(nameof(DelayActivatedVisual), 0.5f);

        }
    }
    Animator anim;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }

    void DelayActivatedVisual()
    {
        anim.SetBool("active", activated);
    }

    protected override bool MovedApon(BoardActor actor)
    {
        if(actor is SkeletonActor){
            ((SkeletonActor)actor).OnSigil = this;
        }
        return true;
    }

    protected override bool CanMoveApon(BoardActor actor)
    {
        return true;
    }

     public override void CreateSnapshot()
   {
        base.CreateSnapshot();
        activatedSnapshots.Push(activated);


   }
   public override void PopSnapshot()
   {
       base.PopSnapshot();
       Activated = activatedSnapshots.Pop();
   }

}
