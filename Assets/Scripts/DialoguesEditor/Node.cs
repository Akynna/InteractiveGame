using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class Node
{
    public Rect rect;



    // Fields to fill the dialogues
    public string title;
    public Rect titleRect;
    public string sceneID = "SceneID";

    // Character
    public string characterNameTitle = "Character";
    public Rect characterTitleRect;
    public string characterNameField = "Character name here";
    public Rect characterFieldRect;

    // Dialogue
    public string dialogueTitle = "Dialogue";
    public Rect dialogueTitleRect;
    public string dialogueArea = "Dialogue here";
    public Rect dialogueAreaRect;

    // Answers
    public string answersTitle = "Answers";
    public Rect answersTitleRect;
    public int nbAnswers = 1;
    public string answerField;
    private List<string> answersList;
    public Rect answerFieldRect;
    private List<Rect> answerFieldsRects;
    public Rect addAnswerButtonRect;
    public Boolean answerActive = false;
    public Button addAnswerButton;


    public bool isDragged;
    public bool isSelected;
    public GUIContent content;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;

    public Action<Node> OnRemoveNode;

    public Node(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint, Action<Node> OnClickRemoveNode)
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        OnRemoveNode = OnClickRemoveNode;

        // Positions of the fields
        titleRect = new Rect(position.x, position.y, 100, 20);

        // Character
        characterTitleRect = new Rect(position.x + 20, position.y + 30, 70, 20);
        characterFieldRect = new Rect(position.x + 100, position.y + 30, 130, 20);

        // Dialogue
        dialogueTitleRect = new Rect(position.x + 20, position.y + 60, 70, 40);
        dialogueAreaRect = new Rect(position.x + 100, position.y + 60, 130, 40);

        // Answers
        answersList = new List<string>();
        answerFieldsRects = new List<Rect>();

        answersTitleRect = new Rect(position.x + 20, position.y + 110, 70, 40);
        addAnswerButtonRect = new Rect(position.x + 100, position.y + 110, 130, 20);
        answerFieldRect = new Rect(position.x + 100, position.y + 110, 130, 20);
        //addAnswerButton = new Button();
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;

        titleRect.position += delta;

        // Character
        characterTitleRect.position += delta;
        characterFieldRect.position += delta;

        // Dialogue
        dialogueTitleRect.position += delta;
        dialogueAreaRect.position += delta;

        // Answers
        answersTitleRect.position += delta;
        addAnswerButtonRect.position += delta;
        answerFieldRect.position += delta;

        for(int i=0; i < answerFieldsRects.Count; ++i) {
            answerFieldsRects[i] = new Rect(answerFieldsRects[i].position.x + delta.x, 
            answerFieldsRects[i].position.y + delta.y, 130, 20);
        }
    }

    public void Draw()
    {
        //content = new GUIContent("This is a box", "This is a tooltip");
        inPoint.Draw();
        outPoint.Draw();

        // Draw the box itself
        GUI.Box(rect, title, style);

        // Draw the content of the box
        sceneID = GUI.TextField(titleRect, sceneID, 25);

        // Character
        GUI.Label(characterTitleRect, characterNameTitle);
        characterNameField = GUI.TextField(characterFieldRect, characterNameField, 25);

        // Dialogue
        GUI.Label(dialogueTitleRect, dialogueTitle);
        dialogueArea = GUI.TextArea(dialogueAreaRect, dialogueArea, 100);

        // Answers
        GUI.Label(answersTitleRect, answersTitle);

        if(answerActive) {

            // Increase the position of the answerField
            Rect newAnswerFieldRect = new Rect(answerFieldRect.position.x,
            answerFieldRect.position.y + (nbAnswers-1)*30, 130, 20);

            // Add a field of text for an answer
            answersList.Add(answerField.Clone().ToString());

            // Add the corresponding Rect transform for that field
            answerFieldsRects.Add(newAnswerFieldRect);

            // Increase the number of answers
            nbAnswers += 1;
        }

        if(GUI.Button(addAnswerButtonRect, "Add an answer") && nbAnswers <= 3)
        {
            answerField = "answer " + nbAnswers.ToString() + " here";

            // Move the button down
            addAnswerButtonRect.position += new Vector2(0, 30);

            // Set the answer field to true to show it
            answerActive = true;

        } else {
            answerActive = false;
            
            for(int i=0; i < answersList.Count; ++i) {
                answersList[i] = GUI.TextField(answerFieldsRects[i], answersList[i], 25);
            }
        }

    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }

                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }

        return false;
    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }
}