using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueWindowObject
{
    public Vector2 position;
    public Rect rect;

    public virtual void Draw(Color? color = null)
    {

    }
    public bool HasPoint(Vector2 point)
    {
        return rect.Contains(point);
    }
}
