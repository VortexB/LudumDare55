using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShapeMaker
{

    public static Vector2Int[] Plus(Vector2Int pos, int range,bool removeOrigin)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        for (int i = -range; i <= range; i++)
        {
            if (removeOrigin && i == 0) continue;
            list.Add(new Vector2Int(0, i)+pos);
            list.Add(new Vector2Int(i, 0)+pos);
        }
        return list.ToArray();
    }
    public static Vector2Int[] VerticalLine(Vector2Int pos, int range, bool removeOrigin)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        for (int i = -range; i <= range; i++)
        {
            if (removeOrigin && i == 0) continue;
            list.Add(new Vector2Int(0, i)+pos);
        }
        return list.ToArray();
    }
    public static Vector2Int[] HorizontalLine(Vector2Int pos, int range, bool removeOrigin)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        for (int i = -range; i <= range; i++)
        {
            if (removeOrigin && i == 0) continue;
            list.Add(new Vector2Int(i, 0)+pos);
        }
        return list.ToArray();
    }
    public static Vector2Int[] Square(Vector2Int pos, int range, bool removeOrigin)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        for (int i = -range; i <= range; i++)
        {
            for (int j = -range; j <= range; j++)
            {
                if (removeOrigin && i == 0 && j==0) continue;
                list.Add(new Vector2Int(i, j) + pos);
            }
        }
        return list.ToArray();
    }

    public static Vector2Int[] Line(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        bool flipped=false;
        if (start.x > end.x)
        {
            Vector2Int temp = start;
            start = end;
            end = temp;
            flipped = true;
        }
        float dx = end.x - start.x;
        float dy = end.y - start.y;

        float steps;
        if (Mathf.Abs(dx) > Mathf.Abs(dy))
            steps = Mathf.Abs(dx);
        else
            steps = Mathf.Abs(dy);
        float xi = dx / steps;
        float yi = dy / steps;

        float x = start.x;
        float y = start.y;
        for (int i = 0; i < steps; i++)
        {
            x += xi;
            y += yi;
            Vector2Int d = new Vector2Int((int)Mathf.Round(x), (int)Mathf.Round(y));
            if (!list.Contains(d))
                list.Add(d);
            if (x + xi != x && y + yi != y)
            {
                Vector2Int xtraY = new Vector2Int((int)Mathf.Round(x), (int)Mathf.Round(y - yi));
                Vector2Int xtraX = new Vector2Int((int)Mathf.Round(x - xi), (int)Mathf.Round(y));
                if(!list.Contains(xtraY))
                    list.Add(xtraY);
                if (!list.Contains(xtraX))
                    list.Add(xtraX);
            }
        }
        if (!flipped)
        {
            while (list.Contains(start))
                list.Remove(start);
            if (!list.Contains(end))
                list.Add(end);
        }
        else
        {
            while (list.Contains(end))
                list.Remove(end);
            if (!list.Contains(start))
                list.Add(start);
        }
        return list.ToArray();

    }

}
