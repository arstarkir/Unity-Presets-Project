using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DialogueEditorWindow : EditorWindow
{
    static Texture2D nodeTexture;
    List<Node> nodes = new List<Node>();
    Node selectedNode = null;

    [MenuItem("Window/Dialogue Editor %#d")]
    static void OpenWindow()
    {
        DialogueEditorWindow window = GetWindow<DialogueEditorWindow>("Dialogue Editor");
        nodeTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Node.png");
        window.Show();
    }
    void OnGUI()
    {
        DrawGrid();

        foreach (Node node in nodes)
            node.Draw();

        Events(Event.current);

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
        if (e.button == 0)
            switch (e.type)
            {
                case EventType.MouseDown:
                    SelectNode(e.mousePosition);
                    break;
                case EventType.MouseDrag:
                    if (selectedNode != null)
                    {
                        DragNode(e.delta);
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    selectedNode = null;
                    break;
            }
        else
            if (e.button == 1 && e.type == EventType.MouseDown)
            OptionsMenu(e.mousePosition);
    }
    void OptionsMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }
    void OnClickAddNode(Vector2 mousePosition)
    {
        nodes.Add(new Node(mousePosition, 100, 100, "Node", nodeTexture));
    }
    void SelectNode(Vector2 mousePosition)
    {
        foreach (var node in nodes)
        {
            if (node.HasPoint(mousePosition))
            {
                selectedNode = node;
                break;
            }
        }
    }
    void DragNode(Vector2 delta)
    {
        selectedNode.rect.position += delta;
    }
}
