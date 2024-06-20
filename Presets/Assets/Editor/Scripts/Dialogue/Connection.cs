using UnityEditor;
using UnityEngine;

public class Connection : DialogueWindowObject
{
    public Vector2 end;
    public Connection(Vector2 start, Vector2 end)
    {
        this.position = start;
        this.end = end;
        this.rect = new Rect(start.x, start.y, start.x - end.x, start.y - end.x);
    }
    public override void Draw(Color? color = null)
    {
        base.Draw(color);
        Handles.BeginGUI();
        if (color == null)
            Handles.color = Color.white;
        else
            Handles.color = (Color)color;
        Handles.DrawLine(position, end, 5);
        Handles.EndGUI();
    }
}
