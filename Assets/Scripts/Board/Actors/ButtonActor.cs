using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ButtonActor : BoardActor
{

    bool pressed;
    public UnityEvent<bool> OnPressed;
    protected Stack<bool> pressedSnapshots = new Stack<bool>();


    Animator anim;
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
    }

    public bool Pressed
    {
        get => pressed;
        set
        {
            if (value != pressed)
            {
                OnPressed?.Invoke(value);
            }
            pressed = value;
            Invoke(nameof(DelayButtonVisual), 0.5f);
        }
    }

    public override void ActorStartup(BoardProperties prop)
    {
        base.ActorStartup(prop);
        Pressed = false;
        TurnController.instance.OnTurnEnded += CheckButton;
    }

    public override void ActorShutdown()
    {
        TurnController.instance.OnTurnEnded -= CheckButton;
        base.ActorShutdown();
    }

    void CheckButton(ActorBehavior _)
    {
        var positionActors = board.GetActors(BoardPosition);
        foreach (var actor in positionActors)
        {
            if (actor is PlayerActor || actor is SkeletonActor || actor is PushableActor)
            {
                Pressed = true;
                return;
            }
        }
        Pressed = false;
    }

    void DelayButtonVisual()
    {
        anim.SetBool("pressed", pressed);
    }

    protected override bool MovedApon(BoardActor actor)
    {
        return true;
    }
    protected override bool CanMoveApon(BoardActor actor)
    {
        return true;
    }

    public override void CreateSnapshot()
    {
        base.CreateSnapshot();
        pressedSnapshots.Push(pressed);
    }

    public override void PopSnapshot()
    {
        base.PopSnapshot();
        Pressed = pressedSnapshots.Pop();
    }

}
