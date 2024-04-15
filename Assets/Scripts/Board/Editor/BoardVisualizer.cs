using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Board))]
public class BoardVisualizer : Editor
{
   private void OnSceneGUI()
    {
        Board board = (Board)target;

        if(board.GetComponent<BoardRenderer2D>()!=null){
            for (int i = 0; i <= board.boardSize.x; i++)
            {
                Handles.DrawLine(
                    new Vector2((i + board.transform.position.x)*board.BoardRenderer.cellSize.x, (board.transform.position.y)*board.BoardRenderer.cellSize.y),
                    new Vector2((i  + board.transform.position.x)*board.BoardRenderer.cellSize.x, (board.boardSize.y+ board.transform.position.y)*board.BoardRenderer.cellSize.y));
            }
            for (int i = 0; i <= board.boardSize.x; i++)
            {
                Handles.DrawLine(
                    new Vector2((board.transform.position.x)*board.BoardRenderer.cellSize.x, (i+ board.transform.position.y)*board.BoardRenderer.cellSize.y),
                    new Vector2((board.boardSize.x+ board.transform.position.x)*board.BoardRenderer.cellSize.x, (i + board.transform.position.y)*board.BoardRenderer.cellSize.y));
            }
        }

        if(board.GetComponent<BoardRenderer3D>()!=null){
            for (int i = 0; i <= board.boardSize.x; i++)
            {
                Handles.DrawLine(
                    new Vector3((i + board.transform.position.x)*board.BoardRenderer.cellSize.x,0, (board.transform.position.y)*board.BoardRenderer.cellSize.y),
                    new Vector3((i  + board.transform.position.x)*board.BoardRenderer.cellSize.x,0, (board.boardSize.y+ board.transform.position.y)*board.BoardRenderer.cellSize.y));
            }
            for (int i = 0; i <= board.boardSize.x; i++)
            {
                Handles.DrawLine(
                    new Vector3((board.transform.position.x)*board.BoardRenderer.cellSize.x,0, (i+ board.transform.position.y)*board.BoardRenderer.cellSize.y),
                    new Vector3((board.boardSize.x+ board.transform.position.x)*board.BoardRenderer.cellSize.x,0, (i + board.transform.position.y)*board.BoardRenderer.cellSize.y));
            }
        }
    }
}
