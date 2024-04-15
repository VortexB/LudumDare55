using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public abstract class BoardActor:MonoBehaviour
{
    [Header("Board Actor")]
    public Board board;
    protected BoardProperties boardProperties;
    private Vector2Int boardPosition;
    protected Stack<Vector2Int> positionSnapshots = new Stack<Vector2Int>();

    [HideInInspector] public ActorRenderer ActorRenderer {get=>GetComponent<ActorRenderer>();}
    [HideInInspector] public bool visualMoving;
    
    public bool Active { get; set; }
    public virtual Vector2Int BoardPosition { get => boardPosition; protected set => boardPosition = value; }

    protected virtual void Start()
    {

    }
    private void OnDisable()
    {
        if (Active) ActorShutdown();
    }

    public virtual void ActorStartup(BoardProperties prop)
    {
        boardProperties = prop;
        boardProperties.OnSaveState += CreateSnapshot;
        boardProperties.OnReverseState += PopSnapshot;
        boardProperties.OnBoardDisable += ActorShutdown;
        boardProperties.OnPlayStateChanged += BoardStateChange;
        board = boardProperties.Board;
        board.AddActor(this, BoardPosition);
        positionSnapshots = new Stack<Vector2Int>();
        Active = true;
        visualMoving = false;
    }

    public virtual void BoardStateChange(BoardPlayState state){
        switch(state){
            case BoardPlayState.Starting:
            break;
            case BoardPlayState.Ending:
            break;
        }
    }

    public virtual void ActorShutdown()
    {
        boardProperties.OnSaveState -= CreateSnapshot;
        boardProperties.OnReverseState -= PopSnapshot;
        boardProperties.OnBoardDisable -= ActorShutdown;
        boardProperties = null;
        board.RemoveActor(this,BoardPosition);
        board = null;
        Active = false;

    }

    public virtual bool Move(Vector2Int position)
    {
        BoardActor[] actors = board.GetActors(position);
        bool stopped = false;
        foreach(var a in actors)
        {
            if(!a.MovedApon(this))stopped =true;
        }
        if (stopped) return false;

        if(board.MoveActor(this, position))
        {
            BoardPosition = position;
            UpdateVisualPosition();
            return true;
        }
        return false;
    }
    public bool CanMove(Vector2Int position)
    {
        BoardActor[] actors = board.GetActors(position);
        bool stopped = false;
        foreach (var a in actors)
        {
            if (!a.CanMoveApon(this)) stopped = true;
        }
        if (stopped) return false;

        if (board.isValidTile(position))
        {
            return true;
        }
        return false;
    }
    public virtual bool FastMove(Vector2Int position)
    {
        if (board.MoveActor(this, position))
        {
            BoardPosition = position;
            FastUpdateVisualPosition();
            return true;
        }
        return false;
    }

    private void FastUpdateVisualPosition()
    {
        transform.position = board.WorldPosition(BoardPosition);
    }
    protected virtual void UpdateVisualPosition()
    {
        visualMoving = true;
        StartCoroutine(SlideMoveOnGrid());

        //transform.position = BoardPosition+board.offset;
    }
    protected virtual IEnumerator SlideMoveOnGrid()
    {
        transform.DOMove(board.WorldPosition(BoardPosition), 0.5f).SetEase(Ease.InOutCubic);
        while (transform.position!= board.WorldPosition(BoardPosition))
        {
            yield return new WaitForFixedUpdate();
        }
        visualMoving = false;
    }

    public bool isActorType<T>()
    {
        return GetComponent<T>()!=null;
    }
    //return value indicates if actor can continue movement
    protected abstract bool MovedApon(BoardActor actor);
    protected abstract bool CanMoveApon(BoardActor actor);

    public virtual void CreateSnapshot()
    {
        positionSnapshots.Push(BoardPosition);
    }
    public virtual void PopSnapshot()
    {
        if(positionSnapshots.Count!=0)
            FastMove(positionSnapshots.Pop());
    }

}

