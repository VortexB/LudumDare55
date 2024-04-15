using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Door : BoardActor
{
    [SerializeField] bool inverseDoor;
    bool open;

        protected Stack<bool> openSnapshots = new Stack<bool>();


    public bool Open
    {
        get => open;
        set
        {
            if(inverseDoor)
                open =!value;
            else open = value;
            Invoke(nameof(DelayDoorVisual), 0.5f);

        }
    }
    Animator anim;

    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        if(inverseDoor)open =true;
    }

    void DelayDoorVisual()
    {
        anim.SetBool("open", open);
    }

    protected override bool MovedApon(BoardActor actor)
    {
        if (open)
            return true;
        return false;
    }

    protected override bool CanMoveApon(BoardActor actor)
    {
        if (open)
            return true;
        return false;
    }
     public override void CreateSnapshot()
   {
        base.CreateSnapshot();
        openSnapshots.Push(open);


   }
   public override void PopSnapshot()
   {
       base.PopSnapshot();
       Open = openSnapshots.Pop();
   }

}
