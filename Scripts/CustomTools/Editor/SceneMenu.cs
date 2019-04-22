using System.Collections;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SceneMenu : Editor {
	[MenuItem("Open Scene/RayCastTestScene")]
	public static void OpenRayCastTestScene(){
		OpenScene("RayCastTestScene");
	}

	[MenuItem("Open Scene/ManipulatorTestScene")]
	public static void OpenManipulatorTestScene(){
		OpenScene("ManipulatorTestScene");
	}

	static void OpenScene(string name){
		if (EditorApplication.SaveCurrentSceneIfUserWantsTo() )
		{
			EditorApplication.OpenScene ("Assets/Scenes/" + name + ".unity");
		}
	}

}



