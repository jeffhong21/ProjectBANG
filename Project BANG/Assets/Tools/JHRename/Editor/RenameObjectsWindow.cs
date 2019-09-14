using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class RenameObjectsWindow : EditorWindow {

    string m_NewName;
	int m_NumSequentialDigits;
	string m_Prefix;
	string m_Suffix;
	string m_Find;
	string m_Replace;
	List<Object> m_ObjectsToRename; // = new List<Object>();

	bool m_DisplaySectedObjects;
	bool m_IsSelectionMode;
	bool m_StartSelectionMode;
	bool m_StopSelectionMode;

	Rect m_InputSection;
	Rect m_ObjectSection;


	[MenuItem("Tools/Toolbox/Rename Objects")]
	private static void OpenWindow(){
		
		RenameObjectsWindow window = (RenameObjectsWindow)GetWindow(typeof(RenameObjectsWindow));

		window.titleContent = new GUIContent("Rename Objects Tool");
		window.minSize = new Vector2(350, 150);
		window.maxSize = new Vector2(350, 600);
		window.Show();

		


	}

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	private void Awake()
	{
	
	}


	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	private void Start()
	{


	}


	/// <summary>
	/// This function is called when the object becomes enabled and active.
	/// </summary>
	private void OnEnable()
	{
		m_IsSelectionMode = false;
		m_StartSelectionMode = true;
		m_StopSelectionMode = false;

		m_NumSequentialDigits = 1;

	}



	/// <summary>
	/// OnGUI is called for rendering and handling GUI events.
	/// This function can be called multiple times per frame (one call per event).
	/// </summary>
	private void OnGUI()
	{
		//DrawLayouts();
		DrawInputSection();
		DrawObjectSection();


	}


	private void DrawLayouts()
	{
		m_ObjectSection.x = 0;
		m_ObjectSection.y = 0;
		m_ObjectSection.width = Screen.width;
		m_ObjectSection.height = 50;

		//GUI.backgroundColor = Color.cyan;

	}


	private void DrawInputSection()
	{
		GUILayoutOption[] GUILayoutOption = new UnityEngine.GUILayoutOption[]{ GUILayout.Width (Screen.width/2) }; 
		GUILayout.Space(8);

		EditorGUILayout.BeginHorizontal();
        GUILayout.Label("New Name:");
        m_NewName = EditorGUILayout.TextField(m_NewName);
        GUILayout.Label("Seperator:");
        m_NewName = EditorGUILayout.TextField(m_NewName);
        GUILayout.Label("Digits:");
        m_NumSequentialDigits = EditorGUILayout.IntField(m_NumSequentialDigits);

		if (m_NumSequentialDigits < 1){
			m_NumSequentialDigits = 1;
		}
		else if (m_NumSequentialDigits > 5){
			m_NumSequentialDigits = 5;
		}
		
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		
		GUILayout.Label("Find:  ");
		m_Find = EditorGUILayout.TextField(m_Find);
		GUILayout.Label("Replace: ");
		m_Replace = EditorGUILayout.TextField(m_Replace);

		EditorGUILayout.EndHorizontal();

		GUILayout.Space(8);
		if (GUILayout.Button("Rename Object(s)") ){
			RenameSelectedObjects(Selection.gameObjects, m_NewName, m_Find, m_Replace);
			//Debug.Log("Renaming <" + Selection.activeObject + "> to " + m_NewName);
		}

		
	}


	private void DrawObjectSection()
	{
		//	-- START OF OBJECT SELECTION PANEL
		EditorGUILayout.BeginVertical(new GUIStyle("HelpBox") );
		//	-- LABEL
		GUILayout.Label("Objects To Rename: ");

		DrawSelectedObjects(Selection.gameObjects);

		//  -- BEGIN HORIZONTAL LAYOUT OF BUTTONS
		EditorGUILayout.BeginHorizontal();

		
		// GUI.enabled = m_StartSelectionMode;
		// if (GUILayout.Button("Start Adding Objects"))
		// {
		// 	m_DisplaySectedObjects = true;
		// 	m_IsSelectionMode = true;
		// 	m_StartSelectionMode = false;
		// 	m_StopSelectionMode = true;
			
		// 	//Debug.Log("Start Selection Mode");
		// }

		// GUI.enabled = m_StopSelectionMode;
		// if (GUILayout.Button("Stop Adding Objects") )
		// {
		// 	m_IsSelectionMode = false;
		// 	m_StartSelectionMode = true;
		// 	m_StopSelectionMode = false;
		// 	Debug.Log("Stop Selection Mode");
		// }
		
		//  -- END HORIZONTAL LAYOUT OF BUTTONS
		EditorGUILayout.EndHorizontal();
		

		//	-- END OBJECT SELECTION PANEL
		EditorGUILayout.EndVertical();
	}

	private void DrawSelectedObjects(GameObject[] selectedObjects)
	{
		EditorGUILayout.BeginVertical(new GUIStyle("HelpBox") ); 
		foreach(GameObject obj in selectedObjects){
			EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
		}
		
		EditorGUILayout.EndVertical();
		Repaint();

	}

	private void RenameSelectedObjects(GameObject[] selectedObjects, string newName, string find, string replace)
	{

		for(int index = 0; index < selectedObjects.Length; index ++){
			selectedObjects[index].name.Replace(find, replace);
			selectedObjects[index].name = newName + index.ToString();
		}

		Repaint();

	}


	private void Update()
	{
		if (m_IsSelectionMode){
			if (Selection.activeGameObject != null){
				DrawSelectedObjects(Selection.gameObjects);
			}
		}
	}

}
