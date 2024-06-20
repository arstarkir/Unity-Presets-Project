using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static Codice.Client.BaseCommands.Import.Commit;

public class DialogueEditorWindow : EditorWindow
{
    static Texture2D nodeTexture;
    List<Node> nodes = new List<Node>(); 
    List<Connection> connections = new List<Connection>();
    DialogueWindowObject selected = null;

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

        //Will need to fix
        //Shortcuts
        if (e.keyCode == KeyCode.N)
        {
            OnClickAddNode(e.mousePosition);
            Repaint();
        }
        if (e.keyCode == KeyCode.C)
        {
            OnClickAddConnection(e.mousePosition);
            Repaint();
        }

        if (GUI.changed) Repaint();
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
                SelectNode(e.mousePosition);
                if (e.button == 1 && e.type == EventType.MouseDown)
                    OptionsMenu(e);
                break;
            case EventType.MouseDrag:
                if (selected != null)
                {
                    if (e.button == 0)
                    {
                        DragNode(e.delta);
                        e.Use();
                    }
                }
                break;
            case EventType.MouseMove: //FIX HERE
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
    }
    void OptionsMenu(Event e)
    {
        Vector2 mousePosition = e.mousePosition;
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.AddItem(new GUIContent("Add Connection"), false, () => OnClickAddConnection(mousePosition));
        genericMenu.ShowAsContext();
    }
    void OnClickAddNode(Vector2 mousePosition)
    {
        nodes.Add(new Node(mousePosition, 100, 100, "Node", nodeTexture));
    }
    void OnClickAddConnection(Vector2 mousePosition)
    {
        connections.Add(new Connection(mousePosition, mousePosition + new Vector2(20,20)));
        selected = connections[connections.Count - 1];
    }
    void SelectNode(Vector2 mousePosition)
    {
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
    void DragNode(Vector2 delta)
    {
        selected.rect.position += delta;
    }
    void DragConnection(Vector2 delta)
    {
        ((Connection)selected).end = delta;
    }
}
