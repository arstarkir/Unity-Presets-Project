using UnityEngine;

public class Node : DialogueWindowObject
{
    public Texture2D texture;
    public string title = "Node";

    public Node(Vector2 position, float width, float height, string title, Texture2D texture = null)
    {
        this.position = position;
        this.rect = new Rect(position.x, position.y, width, height);
        this.title = title;
        this.texture = texture;
    }
    public override void Draw(Color? color = null)
    {
        base.Draw(color);
        //if (texture != null)
        //    GUI.DrawTexture(rect, texture);
        //else
            GUI.Box(rect, title);
        //GUI.Label(new Rect(rect.x + rect.width*1/4, rect.y, 40, 20), title);
    }
}
