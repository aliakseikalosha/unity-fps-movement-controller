﻿using UnityEditor;
using UnityEngine;

// By B0N3head

// This script, although readable was not created out to be super readable for beginners.
// Its just for setup and is not used during gameplay
namespace FPSMovementController
{
    [CustomEditor(typeof(PlayerMovement))]
    public class PlayerControllerEditor : Editor
    {
        private readonly string dialogTitle = "Player Movement Setup v1.1", dialogYes = "Yeah", dialogNo = "No thanks";

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            PlayerMovement pmvmt = (PlayerMovement)target;
            GUILayout.Space(10);
            if (GUILayout.Button("Setup Player & World"))
            {
                if (EditorUtility.DisplayDialog(
                        dialogTitle,
                        "Would you like me to set custom world physics for you?\n\n" +
                        "This changes the gravity\n" +
                        "If this is a pre-existing project please use caution, it will change how anything using physics moves",
                        dialogYes,
                        dialogNo
                ))
                {
                    Physics.gravity = new Vector3(0, -19F, 0);
                    Debug.Log("Physics set");
                }

                if (EditorUtility.DisplayDialog(
                        dialogTitle,
                        $"Would you like me to create a \"{Constants.GroundTag}\" tag for you?",
                        dialogYes,
                        dialogNo
                ))
                {
                    PlayerSetup.AddGroundTag();
                }

                pmvmt.SetupCharacter();

                EditorUtility.DisplayDialog(
                        dialogTitle,
                        "Character all setup\n\n" +
                        "Please don't forget to set the ground tag to the ground of your level/scenes",
                        "Awesome, will do"
                );
            }
        }


    }

    public static class PlayerSetup
    {
        [MenuItem("FPSMovementController/Add Ground Tag")]
        public static void AddGroundTag()
        {
            CreateTag(Constants.GroundTag);
        }


        // Mashup of code from ctwheels & Leslie-Young by B0N3head 
        // Dw about understanding this, it's just a tool for creating tags in editor for unity
        /// <summary>
        /// Add tag, if missing, to the unity editor
        /// </summary>
        /// <param name="tagName">Name of the tag</param>
        public static void CreateTag(string tagName)
        {
            SerializedObject tagM = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsP = tagM.FindProperty("tags");
            bool found = false;

            if (tagsP.arraySize < 10000)
            {
                for (int i = 0; i < tagsP.arraySize; i++)
                {
                    SerializedProperty t = tagsP.GetArrayElementAtIndex(i);
                    if (t.stringValue.Equals(tagName))
                    {
                        found = true;
                        Debug.Log($"The \"{tagName}\" tag already exists");
                        break;
                    }
                }

                if (!found)
                {
                    tagsP.InsertArrayElementAtIndex(0);
                    SerializedProperty n = tagsP.GetArrayElementAtIndex(0);
                    n.stringValue = tagName;
                    Debug.Log($"The \"{tagName}\" tag has been added");
                    tagM.ApplyModifiedPropertiesWithoutUndo();
                }
            }
        }
    }
}