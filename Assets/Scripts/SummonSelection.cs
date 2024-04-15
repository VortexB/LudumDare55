using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSelection : MonoBehaviour
{
    Action clickEvent;

    public Action ClickEvent { get => clickEvent; set => clickEvent = value; }

    private void OnMouseDown() {
        ClickEvent();
    }
}
