using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerSkeletonSummoner : MonoBehaviour
{
    [SerializeField] GameObject summonObject;
    [SerializeField] GameObject summonSelectionObject;
    [SerializeField] GameObject summonCamera;
    PlayerActor player;

    List<GameObject> instansiatedSummonSelections;
    SkeletonActor tempSkeletonActor;
    private void Start()
    {
        player = GetComponent<PlayerActor>();
    }
    public void ToggleSummonViewOn(Vector2Int playerPosition, int summonCount,Action<bool> SummonedCompleted)
    {
        //move camera
        summonCamera.SetActive(true);
        //popup overlay
        Vector2Int[] dirs = { Vector2Int.right, Vector2Int.left, Vector2Int.down, Vector2Int.up };
        instansiatedSummonSelections = new List<GameObject>();
        tempSkeletonActor = gameObject.AddComponent<SkeletonActor>();
        tempSkeletonActor.board = player.board;
        foreach (var dir in dirs)
        {
            // if (player.board.GetActors<SigilActor>(playerPosition + dir).Length > 0) continue;
            if (player.board.GetActors<SkeletonActor>(playerPosition + dir).Length > 0)
            {
                var skeleton = player.board.GetActors<SkeletonActor>(playerPosition + dir)[0];
                if (skeleton.State == SkeletonActor.SkeletonState.Bones)
                {
                    var selection = Instantiate(summonSelectionObject);
                    instansiatedSummonSelections.Add(selection);
                    selection.GetComponent<SummonSelection>().ClickEvent = () =>
                    {

                        foreach (var subdir in dirs){
                            if(subdir+skeleton.BoardPosition == player.BoardPosition)continue;
                            if (skeleton.CanMove(skeleton.BoardPosition + subdir))
                            {
                                var subSelection = Instantiate(summonSelectionObject);
                                instansiatedSummonSelections.Add(subSelection);
                                subSelection.GetComponent<SummonSelection>().ClickEvent = () =>
                                {
                                    player.board.BoardProperties.SaveState();
                                    ReanimateSummon(skeleton, subdir);
                                    ToggleSummonViewOff();
                                    SummonedCompleted(false);
                                };
                                subSelection.transform.position = player.board.WorldPosition(playerPosition + dir+ subdir);
                                subSelection.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
                            }
                        }

                    };
                    selection.transform.position = player.board.WorldPosition(playerPosition + dir);
                    selection.GetComponentInChildren<SpriteRenderer>().color = Color.cyan;
                }
            }
            else if (tempSkeletonActor.CanMove(playerPosition + dir))
            {
                if(summonCount<=0)continue;
                var selection = Instantiate(summonSelectionObject);
                instansiatedSummonSelections.Add(selection);
                selection.GetComponent<SummonSelection>().ClickEvent = () =>
                {
                    player.board.BoardProperties.SaveState();
                    InstantiateSummon(playerPosition + dir, dir);
                    ToggleSummonViewOff();
                    SummonedCompleted(true);
                };
                selection.transform.position = player.board.WorldPosition(playerPosition + dir);
            }
        }
        //any anims
        
    }

    private void ReanimateSummon(SkeletonActor skeleton, Vector2Int direction)
    {
        player.anim.SetTrigger("Summon");
        skeleton.State = SkeletonActor.SkeletonState.Summoned;
        skeleton.Direction = direction;
        skeleton.anim.SetTrigger("Revive");
    }

    public void ToggleSummonViewOff()
    {
        summonCamera.SetActive(false);
        foreach (var selection in instansiatedSummonSelections)
        {
            Destroy(selection);
        }
        instansiatedSummonSelections = null;
        Destroy(tempSkeletonActor);
    }

    void InstantiateSummon(Vector2Int position, Vector2Int direction)
    {
        player.anim.SetTrigger("Summon");
        GameObject newSummon = Instantiate(summonObject);
        SkeletonActor actor = newSummon.GetComponent<SkeletonActor>();
        actor.ActorStartup(player.board.BoardProperties);
        actor.FastMove(position);
        actor.Direction = direction;
        actor.State = SkeletonActor.SkeletonState.Summoned;
        actor.anim.SetTrigger("Revive");
        TurnController.instance.AddActor(actor);

    }
}
