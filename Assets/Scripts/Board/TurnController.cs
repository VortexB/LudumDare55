using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    public static TurnController instance;

    bool turnOrderActive;
    int rounds;
    private int currentActorIndex;
    public int CurrentActorIndex
    {
        get => currentActorIndex;
        set
        {
            if (initiativeOrder != null)
            {
                if (value < 0)
                    currentActorIndex = 0;
                else if (value < initiativeOrder.Count)
                    currentActorIndex = value;
                else
                {
                    currentActorIndex = 0;
                    rounds++;
                }
            }
        }
    }
    public event Action<ActorBehavior> OnTurnEnded;

    List<ActorBehavior> initiativeOrder;

    Stack<int> roundsSnapshots = new Stack<int>();
    Stack<int> currentIndexSnapshots = new Stack<int>();

    private void Awake()
    {
        if (instance == null) instance = this;

    }

    void Start()
    {
        BoardController.instance.OnBoardLinked += NewBoard;
        BoardController.instance.OnBoardUnlinked += RemoveBoard;

    }

    void NewBoard(BoardProperties boardProperties)
    {
        List<ActorBehavior> actors = new List<ActorBehavior>();
        foreach (BoardActor actor in boardProperties.GetActors())
        {
            ActorBehavior behav = actor.GetComponent<ActorBehavior>();
            if (behav != null)
            {
                if (behav.initiative > 0)
                {
                    actors.Add(behav);
                }
            }
        }
        actors.Sort((actor1, actor2) => actor1.initiative.CompareTo(actor2.initiative));
        initiativeOrder = actors;
        CurrentActorIndex = 0;
        rounds = 0;
        boardProperties.OnPlayStateChanged += HandleBoardState;
        boardProperties.OnSaveState += CreateTurnSnapshot;
        boardProperties.OnReverseState += PopTurnSnapshot;
    }

    public void AddActor(BoardActor actor)
    {
        ActorBehavior behav = actor.GetComponent<ActorBehavior>();
        if (behav != null)
        {
            if (behav.initiative > 0)
            {
                initiativeOrder.Add(behav);
                initiativeOrder.Sort((actor1, actor2) => actor1.initiative.CompareTo(actor2.initiative));
                if (initiativeOrder.FindIndex(val => val == behav) < currentActorIndex)
                {
                    currentActorIndex++;
                }
            }
        }
    }

    public void RemoveActor(BoardActor actor)
    {
        ActorBehavior behav = actor.GetComponent<ActorBehavior>();
        if (behav != null)
        {
            if (behav.initiative <= 0) return;
            int index =initiativeOrder.FindIndex(val => val == behav);
            initiativeOrder.Remove(behav);
            if (index < currentActorIndex)
            {
                currentActorIndex--;
            }
        }
    }

    private void PopTurnSnapshot()
    {
        if (roundsSnapshots.Count != 0)
            rounds = roundsSnapshots.Pop();
        if (currentIndexSnapshots.Count != 0)
            currentActorIndex = currentIndexSnapshots.Pop();
    }

    private void CreateTurnSnapshot()
    {
        roundsSnapshots.Push(rounds);
        currentIndexSnapshots.Push(currentActorIndex);
    }

    void RemoveBoard(BoardProperties boardProperties)
    {
        initiativeOrder = null;
        boardProperties.OnPlayStateChanged -= HandleBoardState;
        boardProperties.OnSaveState -= CreateTurnSnapshot;
        boardProperties.OnReverseState -= PopTurnSnapshot;
    }

    void HandleBoardState(BoardPlayState state)
    {
        switch (state)
        {
            case BoardPlayState.Playing:
                if (!turnOrderActive)
                    ProcessTurn(initiativeOrder[0]);
                break;
            default:
                break;
        }
    }

    void ProcessTurn(ActorBehavior actor)
    {
        turnOrderActive = true;
        Action<ActorBehavior> startNextTurn = (ActorBehavior actor) =>
        {
            OnTurnEnded?.Invoke(actor);
            if (initiativeOrder[currentActorIndex] != actor)
            {
                turnOrderActive = false;
                return;
            }
            if(actor is PlayerBoardBehavior){
                if(BoardController.instance.CheckExtraordinaryCondition()){
                    turnOrderActive = false;
                    return;
                }
            }
            CurrentActorIndex++;
            TakeTurn();
        };
        actor.StartTurn(startNextTurn);
    }

    public void TakeTurn() => ProcessTurn(initiativeOrder[CurrentActorIndex]);

}
