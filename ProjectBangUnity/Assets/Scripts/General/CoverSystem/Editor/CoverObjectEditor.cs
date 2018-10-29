namespace Bang
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;


    [CustomEditor(typeof(CoverObject))]
    public class CoverObjectEditor : Editor
    {
        CoverObject t;

        private void OnEnable()
        {
            t = target as CoverObject;

            //SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        }

        private void OnDisable()
        {
            //SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(12);
            if (GUILayout.Button("Setup Cover Spots"))
            {
                t.SetupCoverLocations();
            }

            EditorGUILayout.HelpBox(HelpBoxInfo(), MessageType.Info);

        }




  //      private void OnSceneGUI()
		//{
  //          t = target as CoverObject;

  //          //  Handle Cover Spots
  //          Handles.color = t.DebugOptions.EmptyColor;
  //          for (int i = 0; i < t.CoverSpots.Count; i++)
  //          {
  //              if (t.Sides[i] != Vector3.zero){
  //                  //Handles.color = t.Occupants[i] == null ? t.DebugOptions.EmptyColor : t.DebugOptions.OccupiedColor;
  //                  Handles.DrawSolidDisc(t.CoverSpots[i], Vector3.up, t.EntitySize);
  //                  //Handles.DrawWireCube(t.Sides[i], t.DebugOptions.BoxSize);
  //              }
  //          }

  //          //  Handle Sides
  //          Handles.color = Color.red;
  //          for (int i = 0; i < t.Sides.Length; i++){
  //              if(t.Sides[i] != Vector3.zero){
  //                  //Handles.DrawSolidDisc(t.Sides[i], Vector3.up, t.DebugOptions.Size);
  //                  Handles.DrawWireCube(t.Sides[i], t.DebugOptions.BoxSize);
  //              }
  //          }

  //          //  Handle Corners
  //          Handles.color = Color.red;
  //          for (int i = 0; i < t.Corners.Length; i++)
  //          {
  //              if (t.Sides[i] != Vector3.zero){
  //                  //Handles.DrawSolidDisc(t.Corners[i], Vector3.up, t.DebugOptions.Size);
  //                  Handles.DrawWireCube(t.Corners[i], t.DebugOptions.BoxSize);
  //              }
  //          }
		//}




        private string HelpBoxInfo()
        {
            string helpBoxInfo = "";
            helpBoxInfo += string.Format("Target: {0}\n", t.name);


            for (int index = 0; index < t.Sides.Length; index++)
            {
                helpBoxInfo += string.Format("Side({0}): {1}\n", index, t.Sides[index]);
            }

            helpBoxInfo += string.Format("Debug Box Size: {0}", t.DebugOptions.BoxSize);

            return helpBoxInfo;
        }


        //private void OnDestroy()
        //{
        //    // When the window is destroyed, remove the delegate
        //    // so that it will no longer do any drawing.
        //    SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        //}

	}
}