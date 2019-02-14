using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace ActorSkins
{
    [CustomEditor(typeof(ActorSkinsManager))]
    public class ActorSkinsManagerEditor : Editor
    {
        ActorSkinsManager _target;


        private void OnEnable()
        {
            _target = target as ActorSkinsManager;
        }


        public override void OnInspectorGUI()
        {

            for (int i = 0; i < _target.skins.Length; i++)
            {
                SerializedProperty property = serializedObject.FindProperty("skins").GetArrayElementAtIndex(i);
                SerializedProperty id = property.FindPropertyRelative("id");
                SerializedProperty mesh = property.FindPropertyRelative("mesh");

                using (new EditorGUILayout.HorizontalScope())
                {
                    //EditorGUILayout.LabelField("Name: ");
                    GUILayout.Space(8f);
                    EditorGUILayout.PropertyField(id, GUIContent.none); //, GUILayout.Width(Screen.width * 0.25f));

                    EditorGUILayout.PropertyField(mesh, GUIContent.none); //, GUILayout.Width(Screen.width * 0.25f));
                    //GUILayout.Space(8f);
                    if (GUILayout.Button("Create Skin Objects", EditorStyles.miniButton, GUILayout.Width(100)))//  GUILayout.Width(Screen.width * 0.15f)
                    {
                        _target.CreateActorSkinObjects(_target.skins[i].id, _target.skins[i].mesh);
                    }   
                }
            }



            base.OnInspectorGUI();

        }


        public void Delete(int idx)
        {
            _target.skins = ShrinkArray(_target.skins, idx);
            EditorUtility.SetDirty(target);
            Repaint();
        }


        private T[] GrowArray<T>(T[] array, int increase)
        {
            T[] newArray = array;
            Array.Resize(ref newArray, array.Length + increase);
            return newArray;
        }

        private T[] ShrinkArray<T>(T[] array, int idx)
        {
            T[] newArray = new T[array.Length - 1];
            if (idx > 0)
                Array.Copy(array, 0, newArray, 0, idx);

            if (idx < array.Length - 1)
                Array.Copy(array, idx + 1, newArray, idx, array.Length - idx - 1);

            return newArray;
        }
    }
}

