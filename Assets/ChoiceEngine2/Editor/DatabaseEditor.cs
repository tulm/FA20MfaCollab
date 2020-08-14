using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Database))]
[CanEditMultipleObjects]
public class DatabaseEditor : Editor  {

    SerializedProperty Menu, Characters, Images, Audio, Prefabs, Languages, Gallery, Quests, Resources, Items, PlayerName, PlaceHolder, Locked;
    Database db;

    List<Vector2> scrolls = new List<Vector2>();

    void OnEnable() {

        db = FindObjectOfType<Database>();

        Menu = serializedObject.FindProperty("databaseMenu");
        Characters = serializedObject.FindProperty("characters");
        Images = serializedObject.FindProperty("images");

        Audio = serializedObject.FindProperty("audios");
        Prefabs = serializedObject.FindProperty("prefabs");
        Languages = serializedObject.FindProperty("languages");
        Gallery = serializedObject.FindProperty("gallery");
        Quests = serializedObject.FindProperty("quests");

        Resources = serializedObject.FindProperty("resources");
        Items = serializedObject.FindProperty("items");
        PlayerName = serializedObject.FindProperty("playerName");

        Locked = serializedObject.FindProperty("locked");
        PlaceHolder = serializedObject.FindProperty("placeholder");

        for (int x = 0; x < 12; x++) {
            scrolls.Add(new Vector2());
        }
    }

    void ShowList(SerializedProperty list, int b) {
        
        EditorGUILayout.PropertyField(list, false);

        if (list.isExpanded) {

            scrolls[b] = EditorGUILayout.BeginScrollView (scrolls[b], GUILayout.ExpandHeight(true), GUILayout.MaxHeight(Screen.height));

            EditorGUILayout.PropertyField (list.FindPropertyRelative ("Array.size"));
            GUILayout.Space (5);

            for (int j = 0; j < list.arraySize; j++) {
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUI.indentLevel = 1;

                    EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField (list.GetArrayElementAtIndex (j), true);
                    EditorGUILayout.EndHorizontal ();

                    GUILayout.Space(10);

                    EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        ShowButtons(list, j);
                    EditorGUILayout.EndHorizontal ();

                    GUILayout.Space(10);

                EditorGUILayout.EndVertical ();

            }

            EditorGUILayout.EndScrollView();

        }

    }
    public override void OnInspectorGUI() {

        serializedObject.Update();
        EditorGUILayout.PropertyField(Menu);

        switch (db.databaseMenu) {
            
            case Database.databaseMenuEnum.Characters:
                ShowList(Characters, 0);
                break;
            case Database.databaseMenuEnum.Images:
                ShowList(Images, 1);
                break;
            case Database.databaseMenuEnum.Audio:
                ShowList(Audio, 2);
                break;
            case Database.databaseMenuEnum.Prefabs:
                ShowList(Prefabs, 3);
                break;
            case Database.databaseMenuEnum.Languages:
                ShowList(Languages, 4);
                break;
            case Database.databaseMenuEnum.Gallery:
                ShowList(Gallery, 5);
                break;
            case Database.databaseMenuEnum.Quests:
                ShowList(Quests, 6);
                break;
            case Database.databaseMenuEnum.Resources:
                ShowList(Resources, 7);
                break;
            case Database.databaseMenuEnum.Items:
                ShowList(Items, 8);
                break;
            case Database.databaseMenuEnum.Misc:

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(10);
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.PropertyField(PlayerName);
                    EditorGUILayout.PropertyField(PlaceHolder);
                    EditorGUILayout.PropertyField(Locked);
                    GUILayout.Space(10);
                EditorGUILayout.EndVertical();
                break;
            
        }

        serializedObject.ApplyModifiedProperties();

    }

    static GUILayoutOption width = GUILayout.Width(35f);

    static GUIContent moveUp = new GUIContent("\u2191", "Move Up"),
        moveDown = new GUIContent("\u2193", "Move Down"),
        duplicate = new GUIContent("+", "Duplicate"),
        delete = new GUIContent("-", "Delete");

    static void ShowButtons (SerializedProperty list, int index) {

        var t = index;

		if (GUILayout.Button(moveUp, EditorStyles.miniButtonLeft, width)) {
			list.MoveArrayElement(index, t - 1);
		}

        if (GUILayout.Button(moveDown, EditorStyles.miniButtonMid, width)) {
            list.MoveArrayElement(index, t + 1);
		}

		if (GUILayout.Button(duplicate, EditorStyles.miniButtonMid, width)) {
			list.InsertArrayElementAtIndex(index);
		}

		if (GUILayout.Button(delete, EditorStyles.miniButtonRight, width)) {
			
			var oldSize = list.arraySize;

			list.DeleteArrayElementAtIndex(index);

			if (list.arraySize == oldSize) {
				list.DeleteArrayElementAtIndex(index);
			}

		}
			
		list.serializedObject.ApplyModifiedProperties ();

	}
    

}