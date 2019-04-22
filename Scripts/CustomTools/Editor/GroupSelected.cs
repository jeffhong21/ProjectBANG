// using UnityEngine;
// using UnityEditor;
// using System.Collections;
 
// public class GroupSelected : EditorWindow
// {
//     /// &lt;summary&gt;
//     ///  Creates an empty node at the center of all selected nodes and parents all selected underneath it. 
//     ///  Basically a nice re-creation of Maya grouping!
//     /// &lt;/summary&gt;
 
//     [MenuItem(&quot;GameObject/Group Selected %g&quot;, priority = 80)]
//     static void Init()
//     {
//         Transform[] selected = Selection.GetTransforms(SelectionMode.ExcludePrefab | SelectionMode.TopLevel);
 
//         GameObject emptyNode = new GameObject();
//         Vector3 averagePosition = Vector3.zero;
//         foreach(Transform node in selected)
//         {
//             averagePosition += node.position;
//         }
//         if (selected.Length &gt; 0)
//         {
//             averagePosition /= selected.Length;
//         }
//         emptyNode.transform.position = averagePosition;
//         emptyNode.name = &quot;group&quot;;
//         foreach (Transform node in selected)
//         {
//             node.parent = emptyNode.transform;
//         }
//    }
// }