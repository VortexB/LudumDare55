using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardStarter : MonoBehaviour
{
    [SerializeField] BoardProperties board;

    [ContextMenu("Start Board")]
    void StartBoard()
    {
        BoardController.instance.StartBoard(board);
    }
}
