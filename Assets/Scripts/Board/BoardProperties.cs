using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public enum BoardPlayState
{
    Disabled,
    Starting,
    Playing,
    Ending
}
[Serializable]
public enum Direction
{
    Up,
    Left,
    Right,
    Down
}

[Serializable]
public struct InitialActorInfo
{
    public BoardActor actor;
    public Vector2Int position;
    public Direction direction;

    public InitialActorInfo(BoardActor actor, Vector2Int position, Direction direction)
    {
        this.actor = actor;
        this.position = position;
        this.direction = direction;
    }
}

public class BoardProperties : MonoBehaviour
{

    [InspectorName("Initial Actors")]
    [SerializeField] public List<InitialActorInfo> actors;

    private BoardPlayState state;
    int stateCount;

    public Board Board { get => GetComponent<Board>(); }
    public BoardPlayState State
    {
        get => state;
        set
        {
            OnPlayStateChanged?.Invoke(value);
            state = value;
        }
    }

    //States
    public event Action OnSaveState;
    public event Action OnReverseState;
    public event Action<BoardPlayState> OnPlayStateChanged;

    //Events
    public event Action OnBoardEnable;
    public event Action OnBoardDisable;
    public event Action OnStartupActorsChanged;

    private void Start()
    {
        State = BoardPlayState.Disabled;
        // BoardController.instance.StartBoard(this);
    }

    [ContextMenu("StartBoard")]
    public void StartBoard()
    {
        BoardController.instance.StartBoard(this);
    }

    //Startup Actors

    public void AddActor(BoardActor actor, Vector2Int position, Direction direction)
    {
        if (actors.Find(initialActor => initialActor.actor == actor).actor != null)
        {
            Debug.Log("Actor Already Added");
            return;
        }
        actors.Add(new InitialActorInfo(actor, position, direction));
        OnStartupActorsChanged?.Invoke();
    }

    public BoardActor[] GetActors()
    {
        List<BoardActor> boardActors = new List<BoardActor>();
        foreach (var initActor in actors)
        {
            boardActors.Add(initActor.actor);
        }
        return boardActors.ToArray();
    }

    public void RemoveActor(BoardActor actor)
    {
        actors.Remove(actors.Find(initalActor => initalActor.actor == actor));
        OnStartupActorsChanged?.Invoke();
    }

    public void RemoveNullActors()
    {
        actors.RemoveAll(initalActor => initalActor.actor == null);
        OnStartupActorsChanged?.Invoke();
    }

    public void ChangeActor(BoardActor actor, Vector2Int position, Direction direction)
    {
        InitialActorInfo actorInfo = actors.Find(initalActor => initalActor.actor == actor);
        actorInfo.position = position;
        actorInfo.direction = direction;
        actors[actors.FindIndex(initalActor => initalActor.actor == actor)] = actorInfo;
        OnStartupActorsChanged?.Invoke();

    }

    public Vector2Int GetActorPosition(BoardActor actor)
    {
        return actors.Find(initalActor => initalActor.actor == actor).position;
    }
    public Direction GetActorDirection(BoardActor actor)
    {
        return actors.Find(initalActor => initalActor.actor == actor).direction;
    }

    public bool HasActor(BoardActor actor)
    {
        return actors.Find(initalActor => initalActor.actor == actor).actor != null;
    }

    //Live States


    public void BoardStartup()
    {
        Board.GenerateBoard();
        OnBoardEnable?.Invoke();
        foreach (var a in actors)
        {
            a.actor.ActorStartup(this);
            a.actor.FastMove(a.position);
        }
        stateCount = 0;
        State = BoardPlayState.Starting;
    }

    public void BoardShutdown()
    {
        OnBoardDisable?.Invoke();
    }

    public void BoardReset()
    {
        BoardShutdown();
        BoardStartup();
    }


    //States
    public void SaveState()
    {
        OnSaveState?.Invoke();
        stateCount++;

    }
    public void ReverseState()
    {
        if (stateCount > 0)
        {
            stateCount--;
            OnReverseState?.Invoke();
        }
    }

    public bool CheckWinState()
    {
        return false;
    }



}
