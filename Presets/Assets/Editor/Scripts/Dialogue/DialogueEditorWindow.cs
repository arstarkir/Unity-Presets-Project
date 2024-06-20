using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DialogueEditorWindow : EditorWindow
{
    static Texture2D nodeTexture;
    List<Node> nodes = new List<Node>(); 
    List<Connection> connections = new List<Connection>();
    DialogueWindowObject selected = null;

    //Ctrl + Shift + Z
    Vector2 prevPos;
    DialogueWindowObject lastaChanged;

    [MenuItem("Window/Dialogue Editor %#d")]
    static void OpenWindow()
    {
        DialogueEditorWindow window = GetWindow<DialogueEditorWindow>("Dialogue Editor");
        nodeTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Node.png");
        window.Show();
    }
    void OnGUI()
    {
        Event e = Event.current;

        DrawGrid();
        foreach (Node node in nodes)
            node.Draw();
        foreach (Connection connection in connections)
            connection.Draw((connection == selected) ? Color.red : null);

        Events(e);

        /*if (GUI.changed)*/ Repaint();
    }
    void DrawGrid()
    {
        int gridSpacing = 20;
        int gridSize = 2000;

        Handles.BeginGUI();
        Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);

        for (int x = 0; x <= gridSize; x += gridSpacing)
            Handles.DrawLine(new Vector2(x, -gridSize), new Vector2(x, gridSize));
        for (int y = 0; y <= gridSize; y += gridSpacing)
            Handles.DrawLine(new Vector2(-gridSize, y), new Vector2(gridSize, y));

        Handles.color = Color.white;
        Handles.EndGUI();
    }
    void Events(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (selected is Connection)
                {
                    Node temp = Connect(e.mousePosition);
                    if(temp != null)
                    {
                        ((Connection)selected).nextNode = temp;
                        ((Connection)selected).end = temp.rect.position;
                    }
                }
                Select(e.mousePosition);
                if(selected != null)
                {
                    prevPos = new Vector2(selected.position.x, selected.position.y);
                    lastaChanged = selected;
                }
                if (e.button == 1 && e.type == EventType.MouseDown)
                    OptionsMenu(e);
                break;
            case EventType.MouseDrag:
                if (selected != null)
                {
                    if (e.button == 0)
                    {
                        DragNode(e.mousePosition);
                        e.Use();
                    }
                }
                break;
            case EventType.MouseMove:
                if (selected != null && selected is Connection)
                {
                    DragConnection(e.mousePosition);
                    e.Use();
                }
                break;
            case EventType.MouseUp:
                if (selected is Node)
                    selected = null;
                break;

        }
        Shortcuts(e);
    }
    void Shortcuts(Event e)
    {
        //Ctrl + Shift + Z
        if (e.control && e.shift && e.keyCode == KeyCode.Z && lastaChanged != null)
        {
            if (lastaChanged is Node)
            {
                if (lastaChanged.rect.position != prevPos)
                {
                    DragNode(prevPos,(Node)lastaChanged);
                    lastaChanged = null;
                }
                else
                {
                    List<Connection> temps = IsConnected((Node)lastaChanged);
                    foreach (var temp in temps)
                    {
                        if (temp.nextNode == lastaChanged)
                            temp.nextNode = null;
                        else
                            temp.originalNode = null;
                    }
                    nodes.Remove((Node)lastaChanged);
                    lastaChanged = null;
                }
                Repaint();
            }
            if (lastaChanged is Connection)
            {
                connections.Remove((Connection)lastaChanged);
                lastaChanged = null;
                Repaint();
            }
            
        }
        if(e.type == EventType.KeyDown)
        {
            //Ctrl + Shift + F
            if (e.control && e.shift && e.keyCode == KeyCode.F)
            {
                OnClickAddNode(e.mousePosition);
                Repaint();
            }
            //Ctrl + Shift + G
            if (e.control && e.shift && e.keyCode == KeyCode.G)
            {
                OnClickAddConnection(e.mousePosition);
                Repaint();
            }
            //Ctrl + V
            //if (e.control && e.keyCode == KeyCode.C)
            //{
            //    OnClickAddNode(e.mousePosition);
            //    Repaint();
            //}
        }
    }
    void OptionsMenu(Event e)
    {
        Vector2 mousePosition = e.mousePosition;
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node %#f"), false, () => OnClickAddNode(mousePosition));
        genericMenu.AddItem(new GUIContent("Add Connection %#g"), false, () => OnClickAddConnection(mousePosition));
        genericMenu.ShowAsContext();
    }
    void OnClickAddNode(Vector2 mousePosition, Node node = null)
    {
        nodes.Add((node != null) ?node : new Node(mousePosition, 100, 100, "Node", nodeTexture));
        lastaChanged = nodes[nodes.Count - 1];
        prevPos = lastaChanged.position;
    }
    void OnClickAddConnection(Vector2 mousePosition)
    {
        if (selected != null && selected is Node)
            connections.Add(new Connection(ref ((Node)selected).position, mousePosition + new Vector2(20, 20), (Node)selected));
        else
            connections.Add(new Connection(mousePosition, mousePosition + new Vector2(20, 20)));
        selected = connections[connections.Count - 1];
        wantsMouseMove = true;
    }
    void Select(Vector2 mousePosition)
    {
        wantsMouseMove = false;
        foreach (var node in nodes)
        {
            if (node.HasPoint(mousePosition))
            {
                selected = node;
                break;
            }
        }
        foreach (var connection in connections)
        {
            if (connection.HasPoint(mousePosition))
            {
                selected = connection;
                break;
            }
        }
    }
    Node Connect(Vector2 mousePosition)
    {
        foreach (var node in nodes)
        {
            if (node.HasPoint(mousePosition))
            {
                return node;
            }
        }
        return null;
    }
    List<Connection> IsConnected(Node node)
    {
        List<Connection> tempConnections = new List<Connection>();
        foreach (var connection in connections)
        {
            if (connection.nextNode == node|| connection.originalNode == node)
            {
                tempConnections.Add(connection);
            }
        }
        return tempConnections;
    }
    void DragNode(Vector2 mousePosition, Node node = null)
    {
        node = (node == null)? (Node)selected : node;
        List<Connection> temps = IsConnected(node);
        foreach (var temp in temps)
        {
            if (temp.nextNode == node)
                temp.end = mousePosition;
            else
                temp.position = mousePosition;
        }
        node.rect.position = mousePosition;
        node.position = mousePosition;
    }
    void DragConnection(Vector2 mousePosition)
    {
        ((Connection)selected).end = mousePosition;
    }
}
