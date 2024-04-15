using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonActor : BoardActor
{
    [System.Serializable]
    public enum SkeletonState{
        Summoned,
        Moving,
        Bones,
        Sigiled
    }
    [SerializeField] private Vector2Int direction;
    [SerializeField] private SkeletonState state;
    public Vector2Int Direction { get => direction; set => direction = value; }
    public SkeletonState State { get => state; set => state = value; }
    public SigilActor OnSigil;

    [SerializeField] public Animator anim;

    protected Stack<SkeletonState> stateSnapshots = new Stack<SkeletonState>();

    protected Stack<Vector2Int> directionSnapshots = new Stack<Vector2Int>();
    protected Stack<SigilActor> sigiledSnapshots = new Stack<SigilActor>();

    protected override void Start() {
        base.Start();
        anim = GetComponentInChildren<Animator>();
    }
    public override void ActorStartup(BoardProperties prop)
    {
        base.ActorStartup(prop);
        anim = GetComponentInChildren<Animator>();

    }
    protected override bool MovedApon(BoardActor actor)
    {
        if (actor is ObstacleActor||actor is SkeletonActor||actor is PlayerActor || actor is PushableActor)
            return false;
        return true;
    }
    protected override bool CanMoveApon(BoardActor actor)
    {
        return MovedApon(actor);
    }
    public override void CreateSnapshot()
   {
        base.CreateSnapshot();
        stateSnapshots.Push(State);
        directionSnapshots.Push(direction);
        sigiledSnapshots.Push(OnSigil);
   }
   public override void PopSnapshot()
   {
       if(positionSnapshots.Count==0){
        ActorShutdown();
        TurnController.instance.RemoveActor(this);
        Destroy(gameObject);
        return;
       }
       base.PopSnapshot();
       direction = directionSnapshots.Pop();
       if(state!=stateSnapshots.Peek()){
        anim.SetTrigger("Undoed");
       }
       state = stateSnapshots.Pop();
       OnSigil = sigiledSnapshots.Pop();
   }
}
