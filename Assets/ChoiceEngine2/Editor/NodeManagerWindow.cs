using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

/* This script displays the Visualizer window */
public class NodeManagerWindow : EditorWindow {
    
    //Showing the window
    [MenuItem("Window/Visualizer")]
    public static void ShowWindow() {
        GetWindow<NodeManagerWindow>("Visualizer");
    }

    //Used to get current node.
    int currentNode = 0;
    string currentNodeTemp = "";

    string currentNodeById = "";

    //Used to delete node
    int toRemove = 0;
    string toRemoveTemp = "Node index";
    bool confirmDelete = false;

    //Used to export/import nodes
    bool confirmExport = false;
    bool confirmImport = false;
    
    string exportFilename;
    string importFilename;

    //Used to add nodes
    int toAdd = 0;
    string toAddTemp = "Insertion index";

    //Size of nodes list
    int ListSize;
    LevelManager r;

    //Visualizer Views
    Vector2 scrollPos;
    Vector2 scrollPos2;
    Vector2 scrollPos3;
    Vector2 scrollPos4;

    //Used by the view toggle button
    string[] switches = new string[]{"Visualizer", "Inspector"};
    int currentSwitch = 0;

    List<string> visitedNodes = new List<string>();
    List<Vector2> occupiedCoordinates = new List<Vector2>();

    float inspectorHeight;
    float inspectorWidth;

    GUIStyle s = new GUIStyle();
    GUIStyle s2 = new GUIStyle();

    Texture2D GetTexture (Color c) {
        Texture2D t = new Texture2D(2, 2);
        
        var p = t.GetPixels();

        for(var i = 0; i < p.Length; ++i) {
            p[i] = c;
        }
        
        t.SetPixels( p );
        t.Apply();

        return t;
    }

    int nodeMenu = 0;

    void OnGUI() {

        //Style 1
        var t = GetTexture(new Color(0.1f,0.1f,0.1f));
        
        s.normal.textColor = Color.white;
        s.normal.background = t;
        s.alignment = TextAnchor.MiddleCenter;
        //Style 2
        var t2 = GetTexture(new Color(0.7f,0.7f,0.7f));
        
        s2.normal.textColor = Color.black;
        s2.normal.background = t2;

        r = FindObjectOfType<LevelManager>();

        if (!r) {
            EditorGUILayout.HelpBox("No level manager object found on the scene. If this is your starting scene, ignore this message. To use the node manager, please access a scene with the level manager component on it.", MessageType.Info);
        } else {
                
            r = FindObjectOfType<LevelManager>();
            var so = new SerializedObject(r);

            EditorGUILayout.BeginHorizontal(); // ---------------------- [1][Main Horizontal]

                EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.Width(300)); // --------------------- [2][First Vertical]
                    
                    GUI.Box (new Rect(0, 0, 300, Screen.height), "", s2);
                    GUI.Label (new Rect(0, 0, 300,30), "ChoiceEngine 2", s);
                    GUILayout.Space(40);
                    EditorGUILayout.HelpBox("Use indexes to insert or remove elements.", MessageType.Info);

                    EditorGUILayout.BeginHorizontal(GUILayout.Height(100)); // --------------------- [3][Inner Horizontal]

                    scrollPos3 = EditorGUILayout.BeginScrollView(scrollPos3, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

                    //Adding new node
                    if (GUI.Button(new Rect(5, 10, 150, 20), "Insert New Node")) {
                        Node n = new Node();
                        if (r.nodes.Count > 0) {
                            r.nodes.Insert(toAdd, new Node()); 
                        } else {
                            r.nodes.Add(new Node());
                        }
                        
                    }
                    
                    toAddTemp = EditorGUI.TextField(new Rect (160, 10, 100, 20), toAdd.ToString());
                    try { toAdd = int.Parse(Regex.Replace(toAddTemp, @"[^a-zA-Z0-9 ]", "")); } catch {}

                    //Deleting node
                    if (GUI.Button(new Rect(5, 40, 150, 20), "Remove Node")) {
                        confirmDelete = true;
                    }

                    EditorGUILayout.Space(90);

                    //Confirm node deletion
                    if (confirmDelete) {

                        if (GUI.Button(new Rect(5, 70, 100, 20), "Confirm")) {
                            r.nodes.RemoveAt(toRemove);
                            confirmDelete = false;
                        }

                        if (GUI.Button(new Rect(115, 70, 100, 20), "Cancel")) {
                            confirmDelete = false;
                        }

                        EditorGUILayout.Space(50);
                        
                    }

                    toRemoveTemp = EditorGUI.TextField(new Rect (160, 40, 100, 20), toRemove.ToString());
                    try { toRemove = int.Parse(Regex.Replace(toRemoveTemp, @"[^a-zA-Z0-9 ]", "")); } catch {}

                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndHorizontal(); // --------------------- [3][End]

                    EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(true)); // --------------------- [3][Inner Horizontal]

                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                    
                    for (int i = 0; i < r.nodes.Count; i++) {

                        var n = r.nodes[i];
                        var nodeName = n.id != "" ? i.ToString() + " - " + n.id : i.ToString() + " - ";

                        GUIStyle style = new GUIStyle(GUI.skin.button);
                        style.alignment = TextAnchor.MiddleLeft;

                        if (GUI.Button(new Rect(5, i * 20, 240, 20), nodeName, style)) {
                            currentNode = i;
                            currentSwitch = 0;
                            currentNodeById = "";
                            GUI.FocusControl(null);
                        }

                        if (GUI.Button(new Rect(250, i * 20, 40, 20), "X")) {
                            toRemove = i;
                            confirmDelete = true;
                        }

                        EditorGUILayout.Space(20);

                    }

                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndHorizontal(); // --------------------- [3][End]

                EditorGUILayout.EndVertical(); // --------------------- [2][End]

                // ----------- [Second Vertical]

                //Creating scroll view to make the window more dynamic.
                EditorGUILayout.BeginVertical(); // --------------------- [2][Second Vertical]

                    EditorGUILayout.BeginHorizontal(); // --------------- [3][Secondary Horizontal]

                    EditorGUI.indentLevel = 1;

                    scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2);

                    //Middle button panning.

                    if (Event.current.button == 2 && Event.current.type == EventType.MouseDrag) {
                        
                        scrollPos2 += -Event.current.delta;
                        Event.current.Use();

                    }
                    
                    EditorGUILayout.BeginHorizontal(GUILayout.Height(0), GUILayout.Width(0));

                    EditorGUIUtility.labelWidth = 200;
                    EditorGUIUtility.fieldWidth = 200;

                    if (currentSwitch == 0) {

                        EditorGUILayout.BeginVertical();
                        
                        //Placing button to allow switching into a different UI
                        if (GUI.Button(new Rect(5, 10, 100, 20), switches[currentSwitch])) {
                                currentSwitch = currentSwitch == 0 ? 1 : 0;
                        }

                        //Placing button to allow switching into a different UI
                        if (GUI.Button(new Rect(125, 10, 200, 20), "Export/Import Nodes")) {
                                currentSwitch = 2;
                        }

                        EditorGUILayout.Space(40);

                        //Node Inspector
                        //Main label
                        GUILayout.Label("Nodes List - " + r.nodes.Count.ToString() + " Nodes Available");
                            
                        if (currentNodeById != "") {

                            var t3 = 0;

                            for (int i = 0; i < r.nodes.Count; i++) {
                                if (r.nodes[i].id == currentNodeById) {
                                    t3 = i;
                                    break;
                                }
                            }

                            currentNode = t3;
                        }

                        //Choosing node
                        EditorGUILayout.Space(5);
                        currentNodeTemp = EditorGUILayout.TextField("Current node: ", currentNode.ToString(), GUILayout.ExpandWidth (false));
                        try { currentNode = int.Parse(Regex.Replace(currentNodeTemp, @"[^a-zA-Z0-9 ]", "")); } catch {}
                        EditorGUILayout.Space(5);
                        
                        //Displaying node if available
                        if (currentNode < r.nodes.Count) {
                            
                            SerializedProperty sp = so.FindProperty(string.Format ("nodes.Array.data[{0}]", currentNode));
                            SerializedProperty nodeId = so.FindProperty(string.Format ("nodes.Array.data[{0}].id", currentNode));
                            SerializedProperty nodeType = so.FindProperty(string.Format ("nodes.Array.data[{0}].nodeType", currentNode));
                            SerializedProperty nextNodeId = so.FindProperty(string.Format ("nodes.Array.data[{0}].nextNodeId", currentNode));
                            SerializedProperty nextNodeRandom = so.FindProperty(string.Format ("nodes.Array.data[{0}].nextNodeRandom", currentNode));
                            SerializedProperty randomNodes = so.FindProperty(string.Format ("nodes.Array.data[{0}].randomNodes", currentNode));
                            SerializedProperty textAreaInteractive = so.FindProperty(string.Format ("nodes.Array.data[{0}].textAreaInteractive", currentNode));
                            SerializedProperty checkPoint = so.FindProperty(string.Format ("nodes.Array.data[{0}].checkPoint", currentNode));
                            SerializedProperty nodeText = so.FindProperty(string.Format ("nodes.Array.data[{0}].text", currentNode));
                            SerializedProperty nodeChoices = so.FindProperty(string.Format ("nodes.Array.data[{0}].choices", currentNode));
                            SerializedProperty entryConidtions = so.FindProperty(string.Format ("nodes.Array.data[{0}].entryConidtions", currentNode));
                            SerializedProperty onNodeEnter = so.FindProperty(string.Format ("nodes.Array.data[{0}].onNodeEnter", currentNode));

                            SerializedProperty menu = so.FindProperty(string.Format ("nodes.Array.data[{0}].onNodeEnter.menu", currentNode));
                            SerializedProperty backgroundConfig = so.FindProperty(string.Format ("nodes.Array.data[{0}].onNodeEnter.backgroundConfig", currentNode));
                            SerializedProperty audioConfig = so.FindProperty(string.Format ("nodes.Array.data[{0}].onNodeEnter.audioConfig", currentNode));
                            SerializedProperty avatarConfig = so.FindProperty(string.Format ("nodes.Array.data[{0}].onNodeEnter.avatarConfig", currentNode));
                            SerializedProperty nameConfig = so.FindProperty(string.Format ("nodes.Array.data[{0}].onNodeEnter.nameConfig", currentNode));
                            SerializedProperty dialogueWindowConfig = so.FindProperty(string.Format ("nodes.Array.data[{0}].onNodeEnter.dialogueWindowConfig", currentNode));
                            SerializedProperty loadLevelConfig = so.FindProperty(string.Format ("nodes.Array.data[{0}].onNodeEnter.loadLevelConfig", currentNode));
                            SerializedProperty spawnConfig = so.FindProperty(string.Format ("nodes.Array.data[{0}].onNodeEnter.spawnConfig", currentNode));
                            SerializedProperty modificationsConfig = so.FindProperty(string.Format ("nodes.Array.data[{0}].onNodeEnter.modificationsConfig", currentNode));
                            SerializedProperty functionsConfig = so.FindProperty(string.Format ("nodes.Array.data[{0}].onNodeEnter.functionsConfig", currentNode));
                            SerializedProperty notificationsConfig = so.FindProperty(string.Format ("nodes.Array.data[{0}].onNodeEnter.notificationsConfig", currentNode));

                            SerializedProperty menu2 = so.FindProperty(string.Format ("nodes.Array.data[{0}].entryConidtions.menu", currentNode));
                            SerializedProperty itemChecks = so.FindProperty(string.Format ("nodes.Array.data[{0}].entryConidtions.itemChecks", currentNode));
                            SerializedProperty affectionChecks = so.FindProperty(string.Format ("nodes.Array.data[{0}].entryConidtions.affectionChecks", currentNode));
                            SerializedProperty resourceChecks = so.FindProperty(string.Format ("nodes.Array.data[{0}].entryConidtions.resourceChecks", currentNode));
                            SerializedProperty statChecks = so.FindProperty(string.Format ("nodes.Array.data[{0}].entryConidtions.statChecks", currentNode));
                            SerializedProperty questChecks = so.FindProperty(string.Format ("nodes.Array.data[{0}].entryConidtions.questChecks", currentNode));
                            SerializedProperty questStateChecks = so.FindProperty(string.Format ("nodes.Array.data[{0}].entryConidtions.questStateChecks", currentNode));
                            SerializedProperty failNodeID = so.FindProperty(string.Format ("nodes.Array.data[{0}].entryConidtions.failNodeID", currentNode));

                            try {

                                EditorGUILayout.BeginVertical();

                                    GUILayout.Space(10);
                                    
                                    EditorGUILayout.BeginHorizontal();

                                    GUILayout.Space(18);

                                    if (GUILayout.Button(NodeButton, EditorStyles.miniButtonLeft, width)) {
                                        nodeMenu = 0;
                                    }

                                    if (GUILayout.Button(EnterConditionsButton, EditorStyles.miniButtonMid, width)) {
                                        nodeMenu = 1;
                                    }

                                    if (GUILayout.Button(EnterEventsButton, EditorStyles.miniButtonMid, width)) {
                                        nodeMenu = 2;
                                    }

                                    EditorGUILayout.EndHorizontal();

                                    GUILayout.Space(10);

                                    if (nodeMenu == 0) {

                                        EditorGUILayout.PropertyField(nodeId);
                                        EditorGUILayout.PropertyField(nodeType, true);
                                        EditorGUILayout.PropertyField(nextNodeRandom);

                                        if (r.nodes[currentNode].nextNodeRandom) {
                                            EditorGUILayout.PropertyField(randomNodes, true);
                                        } else {
                                            EditorGUILayout.PropertyField(nextNodeId);
                                        }
                                        
                                        EditorGUILayout.PropertyField(textAreaInteractive);
                                        EditorGUILayout.PropertyField(nodeText);
                                        EditorGUILayout.PropertyField(nodeChoices, true);
                                        EditorGUILayout.PropertyField(checkPoint);
                                        so.ApplyModifiedProperties();

                                    } else if (nodeMenu == 1) {

                                        EditorGUILayout.PropertyField(menu2, true);
                                        GUILayout.Space(10);
                                        so.ApplyModifiedProperties();

                                        switch(r.nodes[currentNode].entryConidtions.menu) {

                                            case EntryConidtions.menuEnum.All:
                                                EditorGUILayout.PropertyField(entryConidtions, true);
                                                onNodeEnter.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                            case EntryConidtions.menuEnum.Items:
                                                EditorGUILayout.PropertyField(itemChecks, true);
                                                itemChecks.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                            case EntryConidtions.menuEnum.Quests:
                                                EditorGUILayout.PropertyField(questChecks, true);
                                                EditorGUILayout.PropertyField(questStateChecks, true);
                                                so.ApplyModifiedProperties();
                                                break;

                                            case EntryConidtions.menuEnum.Resources:
                                                EditorGUILayout.PropertyField(resourceChecks, true);
                                                resourceChecks.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                            case EntryConidtions.menuEnum.Affections:
                                                EditorGUILayout.PropertyField(affectionChecks, true);
                                                affectionChecks.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                            case EntryConidtions.menuEnum.Stats:
                                                EditorGUILayout.PropertyField(statChecks, true);
                                                statChecks.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                        }

                                        GUILayout.Space(10);
                                        EditorGUILayout.PropertyField(failNodeID);
                                        so.ApplyModifiedProperties();

                                    } else {

                                        EditorGUILayout.PropertyField(menu, true);
                                        GUILayout.Space(10);
                                        so.ApplyModifiedProperties();

                                        switch(r.nodes[currentNode].onNodeEnter.menu) {

                                            case NodeEnter.menuEnum.All:
                                                EditorGUILayout.PropertyField(onNodeEnter, true);
                                                onNodeEnter.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                            case NodeEnter.menuEnum.Audio:
                                                EditorGUILayout.PropertyField(audioConfig, true);
                                                audioConfig.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                            case NodeEnter.menuEnum.Avatar:
                                                EditorGUILayout.PropertyField(avatarConfig, true);
                                                avatarConfig.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                            case NodeEnter.menuEnum.Background:
                                                EditorGUILayout.PropertyField(backgroundConfig, true);
                                                backgroundConfig.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                            case NodeEnter.menuEnum.DialogueWindow:
                                                EditorGUILayout.PropertyField(dialogueWindowConfig, true);
                                                dialogueWindowConfig.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                            case NodeEnter.menuEnum.Functions:
                                                EditorGUILayout.PropertyField(functionsConfig, true);
                                                functionsConfig.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                            case NodeEnter.menuEnum.LevelLoading:
                                                EditorGUILayout.PropertyField(loadLevelConfig, true);
                                                loadLevelConfig.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                            case NodeEnter.menuEnum.Modifications:
                                                EditorGUILayout.PropertyField(modificationsConfig, true);
                                                modificationsConfig.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                            case NodeEnter.menuEnum.Name:
                                                EditorGUILayout.PropertyField(nameConfig, true);
                                                nameConfig.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                            case NodeEnter.menuEnum.Notifications:
                                                EditorGUILayout.PropertyField(notificationsConfig, true);
                                                notificationsConfig.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;

                                            case NodeEnter.menuEnum.Spawn:
                                                EditorGUILayout.PropertyField(spawnConfig, true);
                                                spawnConfig.isExpanded = true;
                                                so.ApplyModifiedProperties();
                                                break;
                                        
                                        }

                                    }

                                EditorGUILayout.EndVertical();

                            } catch {}
                            
                        }

                        EditorGUILayout.EndVertical();

                    } else if (currentSwitch == 1) {

                        EditorGUILayout.BeginHorizontal(GUILayout.Height(inspectorHeight), GUILayout.Width(inspectorWidth));
                        EditorGUILayout.BeginVertical();
                        

                        //Placing button to allow switching into a different UI
                        if (GUI.Button(new Rect(5, 10, 100, 20), switches[currentSwitch])) {
                                currentSwitch = currentSwitch == 0 ? 1 : 0;
                        }

                        //Placing button to allow switching into a different UI
                        if (GUI.Button(new Rect(125, 10, 200, 20), "Export/Import Nodes")) {
                                currentSwitch = 2;
                        }

                        //Visualizer
                        visitedNodes.Clear();
                        occupiedCoordinates.Clear();

                        try {
                            drawNodes(r.nodes[0], 5, -50, true, r.nodes[0].id , true); 
                        } catch {}
                        
                        
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        
                    } else {

                        EditorGUILayout.BeginVertical();
                        
                        //UI Switches
                        if (GUI.Button(new Rect(5, 10, 100, 20), switches[0])) {
                                currentSwitch = 1;
                        }

                        if (GUI.Button(new Rect(125, 10, 200, 20), switches[1])) {
                                currentSwitch = 0;
                        }
                        
                        EditorGUILayout.BeginVertical( GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                        
                        //Import & Export filenames
                        EditorGUIUtility.labelWidth = 120;
                        exportFilename = EditorGUI.TextField(new Rect (220, 40, 350, 20), "Export file path", exportFilename);
                        importFilename = EditorGUI.TextField(new Rect (220, 80, 350, 20), "Import file path", importFilename);
                        EditorGUIUtility.labelWidth = 200;

                        //Export & import buttons
                        if (GUI.Button(new Rect(5, 40, 200, 20), "Export")) {
                            confirmExport = true;
                        }

                        if (GUI.Button(new Rect(5, 80, 200, 20), "Import")) {
                            confirmImport = true;
                        }

                        var s = FindObjectOfType<LevelManager>();

                        //Confrimation buttons
                        if (confirmExport) {

                            GUI.Label (new Rect(5, 110, 300, 30), "Export nodes as " + exportFilename + ".json ?");

                            if (GUI.Button(new Rect(5, 150, 200, 20), "Confrim")) {
                                confirmExport = false;
                                s.exportNodes(exportFilename);
                            }

                            if (GUI.Button(new Rect(235, 150, 200, 20), "Cancel")) {
                                confirmExport = false;
                            }
                        }

                        if (confirmImport) {

                            GUI.Label (new Rect(5, 110, 300, 30), "Import nodes from " + importFilename + ".json ?");

                            if (GUI.Button(new Rect(5, 150, 200, 20), "Confrim")) {
                                s.importNodes(importFilename);
                                confirmImport = false;
                            }

                            if (GUI.Button(new Rect(235, 150, 200, 20), "Cancel")) {
                                confirmImport = false;
                            }

                        }


                        
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(30);
                    EditorGUILayout.EndScrollView();

                    //Using leftover space
                    EditorGUILayout.EndHorizontal(); // --------------- [3][End]
                EditorGUILayout.EndVertical(); // --------------------- [2][End]
            EditorGUILayout.EndHorizontal(); // ---------------------- [1][End]

        }


    }

    Vector2 getUnoccupied(float x, float y, bool expandDown) {

        var newX = x;
        var newY = y + 120;

        while (occupiedCoordinates.Contains(new Vector2(newX, newY))) {
            newY = expandDown ? newY + 100 : newY;
            newX = expandDown ? newX : newX + 100;
        }

        //Adding coordinates to visisted
        occupiedCoordinates.Add(new Vector2(newX, newY));

        inspectorHeight = inspectorHeight < newY ? newY + 100: inspectorHeight;
        inspectorWidth = inspectorWidth < newX ? newX + 180: inspectorWidth;

        return new Vector2(newX, newY);
    }
    
    void drawNodes (Node n, float x, float y, bool expandDown, string label, bool isRoot) {

        var unoccupied = getUnoccupied(x, y, expandDown);
        var newX = unoccupied.x;
        var newY = unoccupied.y;

        //Adding coordinates to visisted
        occupiedCoordinates.Add(new Vector2(newX, newY));

        drawNode(n, x, y, newX, newY, label, isRoot);

        //Drawing children
        if (!visitedNodes.Contains(n.id)) {
            //Labeling node as visisted
            visitedNodes.Add(n.id);

            var nextNode = r.getNodeById(n.nextNodeId);
            if (nextNode != null) {
                drawNodes(nextNode, newX, newY, true, nextNode.id, false);
            }
            
            var nodeFail = r.getNodeById(n.entryConidtions.failNodeID);
            if (nodeFail != null) {
                drawNodes(nodeFail, newX, newY, false, nodeFail.id + " [Fail]", false);
            }

            foreach (Choice c in n.choices) {
                var nodeOption = r.getNodeById(c.choiceNodeID);
                if (nodeOption != null) {
                    drawNodes(nodeOption, newX, newY, false, "Option " + nodeOption.id, false); 
                }
                
            }

        }

        
    }

    void drawNode(Node n, float x, float y, float x2, float y2, string label, bool isRoot) {

        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.wordWrap = true;

        if(!isRoot) {
            drawLine(x, y, x2, y2, 40, 40);   
        }
        
        if (GUI.Button(new Rect(x2, y2, 80, 80), label, style)) {
            currentNodeById = n.id;
            currentSwitch = 0;
        }

    }

    void drawLine(float x1, float y1, float x2, float y2, float correctionX, float correctionY) {

        x1 += 40;
        x2 += 40;
        y1 += 80;
        EditorGUI.DrawRect(new Rect(x1, y1, 2, 20), Color.green); 
        EditorGUI.DrawRect(new Rect(x1, y1 + 20, x2 - x1, 2), Color.green); 
        EditorGUI.DrawRect(new Rect(x2, y1 + 20, 2, y2 - y1), Color.green); 

        /*
        var dx = x2 - x1;
        var dy = y2 - y1;

        //Naive line drawing
        //This method can be resource heavy, use at your own risk. (aka it will lag a lot)
        if (dx == 0) {

            for (float y = y1; y < y2; y += 8){
                EditorGUI.DrawRect(new Rect(x1 + correctionX, y + correctionY, 2, 2), Color.green); 
            }

        } else {

            for (float x = x1; x < x2; x += 8){
                float y = y1 + (dy * (x - x1))/dx;
                EditorGUI.DrawRect(new Rect(x + correctionX, y + correctionY, 2, 2), Color.green); 
            }
            
        }
        */

    }

    static GUILayoutOption width = GUILayout.Width(100f);

    static GUIContent NodeButton = new GUIContent("Node"),
        EnterConditionsButton = new GUIContent("Entry Conditions"),
        EnterEventsButton = new GUIContent("On Enter Events");

}

// Code by Cination / Tsenkilidis Alexandros.