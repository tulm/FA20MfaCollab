using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/*  A property drawer that allows to show/hide variables in the inpsector based on the int values of other variables.

    For example, if you want to display variable A (in the inspector) if B == 1, then you could do the following.

    [Display(false, "", "B", new int[]{1})]
    A = 12;

    In this case, false means that B is not located inside of a list, and the second parameter is empty since there is no list to name.
    For this tag to work you'd also need to have a support class. You can view and example by inspecting "NodeManager.cs".

*/

[CustomPropertyDrawer(typeof(DisplayAttribute))]
public class DisplayDrawer : PropertyDrawer {

    private bool show = false;
    
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        DisplayAttribute display = (DisplayAttribute) this.attribute;

        var isInList = display.isInList;
        var listName = display.listName;
        var propertyName = display.propertyName;
        var propertyValues = display.propertyValues;
        var targetProperty = display.targetProperty;

        var obj = property.serializedObject;

        if (isInList) {

            var n = obj.FindProperty (listName);

            for (int i = 0; i < n.arraySize; i++) {

                if (n.arraySize > i && n.GetArrayElementAtIndex (i).isExpanded) {

                    var p = obj.FindProperty (string.Format (listName + ".Array.data[{0}]." + propertyName, i));
                    var z = obj.FindProperty (string.Format (listName + ".Array.data[{0}]." + targetProperty, i));

                    if (property.propertyPath == z.propertyPath) {

                        if (Array.IndexOf(propertyValues, p.intValue) > -1) {
                            EditorGUI.PropertyField(position, property, label, true);
                            obj.ApplyModifiedProperties();
                        }
                        
                    }
                            
                }

            }

        } else {

            var n = obj.FindProperty (propertyName);

            if (Array.IndexOf(propertyValues, n.intValue) > -1) {
                EditorGUI.PropertyField(position, property, label, true);
                obj.ApplyModifiedProperties();
                show = true;
            } else {
                show = false;
            }

        }

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {

        DisplayAttribute display = (DisplayAttribute) this.attribute;

        var isInList = display.isInList;
        var listName = display.listName;
        var propertyName = display.propertyName;
        var propertyValues = display.propertyValues;
        var targetProperty = display.targetProperty;

        var obj = property.serializedObject;

        if (isInList) {

            var n = obj.FindProperty (listName);

            for (int i = 0; i < n.arraySize; i++) {

                if (n.arraySize > i && n.GetArrayElementAtIndex (i).isExpanded) {

                    var p = obj.FindProperty (string.Format (listName + ".Array.data[{0}]." + propertyName, i));
                    var z = obj.FindProperty (string.Format (listName + ".Array.data[{0}]." + targetProperty, i));

                    if (property.propertyPath == z.propertyPath) {

                        if (Array.IndexOf(propertyValues, p.intValue) > -1) {
                            return EditorGUI.GetPropertyHeight(property);
                        } else {
                            return -EditorGUIUtility.standardVerticalSpacing;
                        }
                        
                    }
                            
                }

            }

        } else {

            if (show) {
                return EditorGUI.GetPropertyHeight(property);
            } else {
                return -EditorGUIUtility.standardVerticalSpacing;
            }

        }

        return EditorGUI.GetPropertyHeight(property);
        
    }

}

// Code by Cination / Tsenkilidis Alexandros.