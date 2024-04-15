
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BoardActorManager : EditorWindow
{
    [MenuItem("Window/Board/ActorManager")]
    public static void ShowWindow()
    {
        GetWindow<BoardActorManager>("Board Actor Manager");
    }
    private Board board;

    public bool positionSync = true;
    bool unaddedActorsDropdown = true;

    bool selectedActorsDropdown = true;
    bool allActorsDropdown = false;
    Vector2 scrollPos;

    //bool highlightActors = true;

    public Board SelectedBoard
    {
        get => board;
        set
        {
            if (board != null)
            {
                // board.BoardProperties.OnStartupActorsChanged -= UpdateState;
            }
            board = value;
            if (value != null)
            {
                // board.BoardProperties.OnStartupActorsChanged += UpdateState;
            }
        }
    }

    List<BoardActor> selectedActors; //SelectedActors();
    List<InitialActorInfo> initialActors; //SelectedBoard.BoardProperties.GetActors();

    GUILayoutOption[] largeButtons = {GUILayout.MinHeight(40)};

    void UpdateState()
    {
        if(board!=null){
            board.BoardProperties.RemoveNullActors();
            initialActors = board.BoardProperties.actors;
        }
    }
    void OnSelectionChange()
    {
        //finds selected actors
        UpdateState();
        selectedActors = new List<BoardActor>();
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj.GetComponent<BoardActor>() != null)
            {
                selectedActors.Add(obj.GetComponent<BoardActor>());
            }
        }
    }

    void OnDestroy()
    {
        // board.BoardProperties.OnStartupActorsChanged -= UpdateState;

    }
    void OnFocus()
    {
        UpdateState();
    }


    void CreateGUI()
    {
        selectedActors = new List<BoardActor>();
        initialActors = new List<InitialActorInfo>();

        //Getting Board
    }

    SerializedObject serializedObject;

    void OnGUI()
    {
        EditorGUILayout.ObjectField("Selected Board", SelectedBoard, typeof(Board), true);
        Board hoveredBoard = null;
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (obj.GetComponent<Board>() != null)
            {
                hoveredBoard = obj.GetComponent<Board>();
            }
        }

        if (hoveredBoard != null)
        {
            if (GUILayout.Button("Select Board"))
            {
                SelectedBoard = hoveredBoard;
                // serializedObject = new SerializedObject(SelectedBoard.BoardProperties);

            }
        }


        GUILayout.Space(10);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        if (SelectedBoard != null)
        {
            // serializedObject.Update();


            GUILayout.Label("Actors", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // // highlights
            // GUILayout.Toggle(highlightActors, "Actors Outline");
            // if (highlightActors)
            // {
            //     foreach (var initialActor in initialActors)
            //     {
            //         if (selectedActors.Contains(initialActor.actor)) continue;
            //         initialActor.actor.gameObject.GetComponent<ActorRenderer>().RendererObject.GetComponent<Renderer>().material.color = initialActor.actor.gameObject.GetComponent<Renderer>().material.color + new Color(0.3f,0,0);
            //     }
            // }

            GUILayout.Space(10);

            //dropdown for each selected actor
            unaddedActorsDropdown = EditorGUILayout.BeginFoldoutHeaderGroup(unaddedActorsDropdown, "Unadded Actors", EditorStyles.toolbarDropDown);
            if (unaddedActorsDropdown)
            {
                foreach (var actor in selectedActors)
                {
                    var rect = EditorGUILayout.BeginVertical();
                    GUI.Box(rect, "");

                    if (initialActors.Find(initalActor => initalActor.actor == actor).actor == null)
                    {
                        GUILayout.Label(actor.gameObject.name, EditorStyles.miniBoldLabel);
                        EditorGUILayout.ObjectField("Actor", actor, typeof(BoardActor), true);
                        if (GUILayout.Button("Add to Board",largeButtons))
                        {
                            SelectedBoard.BoardProperties.AddActor(actor, SelectedBoard.ToBoardPosition(actor.transform.position), Direction.Down);
                        }
                    }
                    GUILayout.Space(5);
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(5);

                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            GUILayout.Space(10);

            selectedActorsDropdown = EditorGUILayout.BeginFoldoutHeaderGroup(selectedActorsDropdown, "Selected Actors", EditorStyles.toolbarDropDown);
            if (selectedActorsDropdown && selectedActors.Count>0)
            {
                if (GUILayout.Button(new GUIContent("Sync Visuals","Sync Selected Visuals to Positions"),largeButtons))
                {
                    foreach (var actor in selectedActors)
                    {
                        if (initialActors.Find(initialActor => initialActor.actor == actor).actor != null)
                        {
                            SyncVisualToPosition(actor);
                        }
                    }
                }
                if (GUILayout.Button(new GUIContent("Sync Positions","Sync Selected Positions to Visuals"),largeButtons))
                {
                    foreach (var actor in selectedActors)
                    {
                        if (initialActors.Find(initialActor => initialActor.actor == actor).actor != null)
                        {
                            SyncPositionToVisual(actor);
                        }
                    }
                }

                if (GUILayout.Button("Remove Actors",GUILayout.Width(100)))
                {

                    foreach (var actor in selectedActors)
                    {
                        if (initialActors.Find(initialActor => initialActor.actor == actor).actor != null)
                        {
                            SelectedBoard.BoardProperties.RemoveActor(actor);
                        }
                    }

                }
                // if (GUILayout.Button("Delete Actors"))
                // {
                //     foreach (var actor in selectedActors)
                //     {
                //         if (initialActors.Find(initialActor => initialActor.actor == actor).actor != null)
                //         {
                //             SelectedBoard.BoardProperties.RemoveActor(actor);
                //             DestroyImmediate(actor.gameObject);
                //         }
                //     }
                // }

                foreach (var actor in selectedActors)
                {
                    if (initialActors.Find(initialActor => initialActor.actor == actor).actor == null) continue;

                    var rect = EditorGUILayout.BeginVertical();
                    GUI.Box(rect, "");
                    GUILayout.Label(actor.gameObject.name, EditorStyles.miniBoldLabel);
                    EditorGUILayout.ObjectField("Actor", actor, typeof(BoardActor), true);

                    Vector2Int position = initialActors.Find(initalActor => initalActor.actor == actor).position;
                    EditorGUI.BeginChangeCheck();
                    position = EditorGUILayout.Vector2IntField("Position", position);
                    if (EditorGUI.EndChangeCheck())
                        if (position != board.BoardProperties.actors.Find(initalActor => initalActor.actor == actor).position)
                            if (SelectedBoard.isValidTile(position))
                                SelectedBoard.BoardProperties.ChangeActor(actor, position, initialActors.Find(initalActor => initalActor.actor == actor).direction);

                    Direction direction = initialActors.Find(initalActor => initalActor.actor == actor).direction;
                    EditorGUI.BeginChangeCheck();
                    direction = (Direction)EditorGUILayout.EnumFlagsField(direction);
                    if (EditorGUI.EndChangeCheck())
                        if (direction != board.BoardProperties.actors.Find(initalActor => initalActor.actor == actor).direction)
                            SelectedBoard.BoardProperties.ChangeActor(actor, initialActors.Find(initalActor => initalActor.actor == actor).position, direction);

                    if (GUILayout.Button("Remove Actor"))
                    {

                        if (initialActors.Find(initialActor => initialActor.actor == actor).actor != null)
                        {
                            SelectedBoard.BoardProperties.RemoveActor(actor);

                        }


                    }
                    // if (GUILayout.Button("Delete Actor"))
                    // {
                    //     if (initialActors.Find(initialActor => initialActor.actor == actor).actor != null)
                    //     {
                    //         SelectedBoard.BoardProperties.RemoveActor(actor);
                    //         DestroyImmediate(actor.gameObject);
                    //     }
                    // }
                    GUILayout.Space(5);
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(5);


                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            GUILayout.Space(10);


            //dropdown for All board actor
            allActorsDropdown = EditorGUILayout.BeginFoldoutHeaderGroup(allActorsDropdown, "All Board Actors", EditorStyles.toolbarDropDown);
            if (allActorsDropdown && initialActors.Count>0)
            {
                if (GUILayout.Button(new GUIContent("Sync Visuals","Sync All Visuals to Positions"),largeButtons))
                {
                    foreach (var initialActor in initialActors)
                    {

                        SyncVisualToPosition(initialActor.actor);

                    }
                }
                if (GUILayout.Button(new GUIContent("Sync Positions","Sync All Positions to Visuals"),largeButtons))
                {
                    foreach (var initialActor in initialActors)
                    {

                        SyncPositionToVisual(initialActor.actor);

                    }
                }
                foreach (var initialActor in initialActors)
                {
                    var rect = EditorGUILayout.BeginVertical();
                    GUI.Box(rect, "");
                    GUILayout.Label(initialActor.actor.gameObject.name, EditorStyles.miniBoldLabel);
                    EditorGUILayout.ObjectField("Actor", initialActor.actor, typeof(BoardActor), true);

                    Vector2Int position = initialActors.Find(initalActor => initalActor.actor == initialActor.actor).position;
                    position = EditorGUILayout.Vector2IntField("Position", position);
                    if (position != board.BoardProperties.actors.Find(initalActor => initalActor.actor == initialActor.actor).position)
                        if (SelectedBoard.isValidTile(position))
                            SelectedBoard.BoardProperties.ChangeActor(initialActor.actor, position, initialActors.Find(initalActor => initalActor.actor == initialActor.actor).direction);


                    Direction direction = initialActors.Find(initalActor => initalActor.actor == initialActor.actor).direction;
                    direction = (Direction)EditorGUILayout.EnumFlagsField(direction);
                    if (direction != board.BoardProperties.actors.Find(initalActor => initalActor.actor == initialActor.actor).direction)
                        SelectedBoard.BoardProperties.ChangeActor(initialActor.actor, initialActors.Find(initalActor => initalActor.actor == initialActor.actor).position, direction);

                    if (GUILayout.Button("Remove Actor"))
                    {
                        SelectedBoard.BoardProperties.RemoveActor(initialActor.actor);
                    }
                    // if (GUILayout.Button("Delete Actor"))
                    // {
                    //     SelectedBoard.BoardProperties.RemoveActor(initialActor.actor);
                    //     DestroyImmediate(initialActor.actor.gameObject);
                    // }

                    GUILayout.Space(5);
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(5);

                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

        }
        EditorGUILayout.EndScrollView();
        GUILayout.Label(initialActors.Count.ToString(), EditorStyles.miniBoldLabel);
            // serializedObject.ApplyModifiedProperties();


            EditorUtility.SetDirty(SelectedBoard);
            EditorUtility.SetDirty(SelectedBoard.BoardProperties);
            PrefabUtility.RecordPrefabInstancePropertyModifications(SelectedBoard);
            PrefabUtility.RecordPrefabInstancePropertyModifications(SelectedBoard.BoardProperties);
            AssetDatabase.SaveAssets();


    }

    void SyncVisualToPosition(BoardActor actor)
    {
        actor.gameObject.transform.position = SelectedBoard.WorldPosition(SelectedBoard.BoardProperties.GetActorPosition(actor));
        actor.ActorRenderer.Rotate(SelectedBoard.BoardProperties.GetActorDirection(actor));
    }

    void SyncPositionToVisual(BoardActor actor)
    {
        SelectedBoard.BoardProperties.ChangeActor(actor, SelectedBoard.ToBoardPosition(actor.transform.position), SelectedBoard.ActorDirection(actor.transform));
    }

}
