using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ParticleToolC))]
public class ManipulatorEditor : Editor 
{

	private ParticleToolC m_ParticleTool;
	private ManipulatorObjectC m_ManipulatorObject;
	private ParticleDebuggerC m_ParticleDebugger;

	private ParticleSystem m_ParticleSystem;				
	private ParticleSystem.Particle[] m_Particles;
	private List<ManipulatorObjectC> m_Manipulators;

	private Transform m_Transform;
	private Vector3 m_Position;
	private Quaternion m_Rotation;
	private Vector3 m_Scale;

	private MANIPULATORTYPE m_Type;				
	private MANIPULATORSHAPE m_Shape;									
	private float m_Size;												
	private Bounds m_Bounds;											
	private float m_Strength;												
	private float m_SmoothStrength;			
	private float m_SmoothDistance;


	// -------- Internal Editor variables----------
	private string manipulatorInformation = string.Format("Manipulator UI Active:  " );


	private void GetManipulatorProperty(ManipulatorObjectC manipulator){

		m_ManipulatorObject = manipulator;
		//  Handles work in world space, so we need to convert position and rotation to world space
		m_Transform = m_ManipulatorObject.transform;
		m_Position = m_ManipulatorObject.transform.position;
		m_Rotation = m_ManipulatorObject.transform.rotation = Tools.pivotRotation == PivotRotation.Local ? m_ManipulatorObject.transform.rotation : Quaternion.identity;
		m_Scale = m_ManipulatorObject.transform.lossyScale;
		m_Type = m_ManipulatorObject.type;
		m_Shape = m_ManipulatorObject.shape;
		m_Size = m_ManipulatorObject.size;
		m_Bounds = m_ManipulatorObject.bounds;
		m_Strength = m_ManipulatorObject.strength;
		m_SmoothStrength = m_ManipulatorObject.smoothStrength;
		m_SmoothDistance = m_ManipulatorObject.smoothDistance;

		
	}


	public override void OnInspectorGUI () 
	{
		m_ParticleTool = target as ParticleToolC;
		m_ParticleSystem = m_ParticleTool.particleSystem;
		m_Particles = m_ParticleTool.particles;


		EditorGUILayout.HelpBox (manipulatorInformation, MessageType.Info);
		DrawCreateManipulatorBtn();
		
		if (m_ParticleTool.ActiveManipulator()){
			for (int index = 0; index < m_ParticleTool.manipulators.Count; index++){
				DrawManipulatorGUI(index);
				GUILayout.Space (8);
			}
		}


		GUILayout.Space (16);
		DebugPanel();
	}

	private void DrawCreateManipulatorBtn(){
		if(GUILayout.Button("Create Manipulator") ){
			m_ParticleTool.CreateManipulator();
			//DebuggerInfo(m_ParticleTool.manipulatorInstance);
			//manipulatorInformation = m_ParticleTool.DebugManipulatorObject(m_ParticleTool.manipulatorInstance);
		}
	}


	private void DrawManipulatorGUI(int index)
	{

		m_ManipulatorObject = m_ParticleTool.manipulators[index];

		GUILayout.BeginVertical (new GUIStyle("HelpBox") ); 
		EditorGUILayout.ObjectField(new GUIContent("Manipulator"), m_ManipulatorObject.transform, typeof(Transform), false);
		EditorGUI.indentLevel++;
		m_ManipulatorObject.transform.localPosition = EditorGUILayout.Vector3Field(new GUIContent("Position"), m_ManipulatorObject.transform.localPosition);
		m_ManipulatorObject.transform.localRotation = Quaternion.Euler(EditorGUILayout.Vector3Field(new GUIContent("Rotation"), m_ManipulatorObject.transform.localRotation.eulerAngles ) );
		m_ManipulatorObject.transform.localScale = EditorGUILayout.Vector3Field(new GUIContent("Scale"), m_ManipulatorObject.transform.localScale);
		EditorGUI.indentLevel--;
		m_ManipulatorObject.type = (MANIPULATORTYPE) EditorGUILayout.EnumPopup(new GUIContent("Type"), m_ManipulatorObject.type);
		m_ManipulatorObject.shape = (MANIPULATORSHAPE) EditorGUILayout.EnumPopup(new GUIContent("Shape"), m_ManipulatorObject.shape);
		if (m_ManipulatorObject.shape == MANIPULATORSHAPE.Box){
			EditorGUI.indentLevel++;
			m_ManipulatorObject.bounds = EditorGUILayout.BoundsField(new GUIContent("Box Scale"), m_ManipulatorObject.bounds );
			EditorGUI.indentLevel--;
		}
		m_ManipulatorObject.size = EditorGUILayout.Slider(new GUIContent("Size"), m_ManipulatorObject.size, 0, m_ManipulatorObject.size * 2);
		m_ManipulatorObject.strength = EditorGUILayout.Slider(new GUIContent("Strength"), m_ManipulatorObject.strength, 0, m_ManipulatorObject.size);
		m_ManipulatorObject.smoothStrength = EditorGUILayout.Slider(new GUIContent("Smooth Strength"), m_ManipulatorObject.smoothStrength, 0, 1);
		m_ManipulatorObject.smoothDistance = EditorGUILayout.Slider(new GUIContent("Smooth Distance"), m_ManipulatorObject.smoothDistance, 0, 1);
		GUILayout.Space (5);
		if(GUILayout.Button("Delete Manipulator") ){
			DeleteManipulator(m_ManipulatorObject);
		}
		GUILayout.Space (5);
		GUILayout.EndVertical ();
	}



	public void DeleteManipulator(ManipulatorObjectC m_ManipulatorObject){
		m_ParticleTool.DeleteManipulator(m_ManipulatorObject);
	}






	private void DebugPanel()
	{
		int maxManipulators = m_ParticleTool.manipulators.Count == 0 ? 0 : m_ParticleTool.manipulators.Count - 1;
		m_ParticleDebugger = new ParticleDebuggerC();
		

		GUILayout.BeginVertical (new GUIStyle("GroupBox") ); 
		GUILayout.Space (8);

		m_ParticleTool.manipulatorInstance = EditorGUILayout.IntSlider("Which Manipulator: ", m_ParticleTool.manipulatorInstance, 0, maxManipulators);

		if(GUILayout.Button("Manipulator Information") ){
			manipulatorInformation = m_ParticleDebugger.DebugManipulatorObject(m_ParticleTool.manipulators[m_ParticleTool.manipulatorInstance]);
			//Debug.Log(m_ParticleDebugger.DebugManipulatorObject(m_ParticleTool.manipulators[m_ParticleTool.manipulatorInstance]));

		}

		if(GUILayout.Button("Particle Information") ){
			//m_ParticleTool.ParticleSystem();
			Debug.Log(m_Particles[0].remainingLifetime);
			//m_ParticleTool.GetParticleProperties();
		}
		GUILayout.Space (8);
		GUILayout.EndVertical ();
		
	}






	private void OnSceneGUI(){
		m_ParticleTool = target as ParticleToolC;

		if(m_ParticleTool.manipulators.Count > 0){
			GetManipulatorProperty(m_ParticleTool.manipulators[m_ParticleTool.manipulatorInstance]);
			ShowManipulatorHandle(m_Position, m_Rotation);
			ShowManipulatorShape(m_Position, m_Rotation, m_Size, m_Shape);
			ShowManipulatorStrength(m_Position, m_Rotation, m_Strength, m_Size, m_Shape);
		}
	}
	

	private void ShowManipulatorHandle(Vector3 position, Quaternion rotation){
		Vector3 manipulatorShapeHandle;

		EditorGUI.BeginChangeCheck();
		manipulatorShapeHandle = Handles.PositionHandle( m_ManipulatorObject.transform.position,  m_ManipulatorObject.transform.rotation);

		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(m_ParticleTool, "Move Manipulator");
			EditorUtility.SetDirty(m_ParticleTool);
			m_ManipulatorObject.transform.position = manipulatorShapeHandle;
		}
		m_ManipulatorObject.transform.position = manipulatorShapeHandle;
		//return m_ManipulatorObject.transform.position;
	}


	private void ShowManipulatorShape(Vector3 position, Quaternion rotation, float size, MANIPULATORSHAPE shape){
		Color handleColor = Color.white;
		Handles.color = handleColor;
		switch(shape)
		{
			case MANIPULATORSHAPE.Sphere:
				DrawSphere(position, rotation, size);
				break;
			case MANIPULATORSHAPE.Box:
				DrawCubeHandle(position, size);
				break;
		}
	}


	private void DrawSphere(Vector3 position, Quaternion rotation, float size){
		m_ManipulatorObject.size = Handles.RadiusHandle(m_ManipulatorObject.transform.rotation, m_ManipulatorObject.transform.position, m_ManipulatorObject.size);
	}


	private void DrawCubeHandle(Vector3 position, float size){

		//Handles.matrix = m_Transform.localToWorldMatrix;
 		//Handles.matrix = Matrix4x4.TRS(position, rotation, scale);
		Color handleColor = Color.white;
		Handles.color = handleColor;
		Handles.DrawWireCube(position, new Vector3(size, size, size));
		//Handles.DrawWireCube(position, Vector3.one);

		float capSize = 0.05f;
		Vector3[] handlePoints = 
			{
				position + Vector3.up * (size/2 + capSize/2),
				position + Vector3.down * (size/2 + capSize/2),
				position + Vector3.forward * (size/2 + capSize/2),
				position + Vector3.back * (size/2 + capSize/2),
				position + Vector3.left * (size/2 + capSize/2),
				position + Vector3.right * (size/2 + capSize/2)
			};
		
		//bool upHandle = Handles.Button(handlePoints[0], Quaternion.identity, capSize, capSize * 2, Handles.CubeCap);
		Vector3 upHandle = Handles.Slider(handlePoints[0], Vector3.up, capSize * HandleUtility.GetHandleSize(handlePoints[0]), Handles.CubeCap, 0.1f) - handlePoints[0];
		Vector3 downHandle = Handles.Slider(handlePoints[1], Vector3.down, capSize * HandleUtility.GetHandleSize(handlePoints[1]), Handles.CubeCap, 0.1f) - handlePoints[1];
		Vector3 forwardHandle = Handles.Slider(handlePoints[2], Vector3.forward, capSize * HandleUtility.GetHandleSize(handlePoints[2]), Handles.CubeCap, 0.1f) - handlePoints[2];
		Vector3 backHandle = Handles.Slider(handlePoints[3], Vector3.back, capSize * HandleUtility.GetHandleSize(handlePoints[3]), Handles.CubeCap, 0.1f) - handlePoints[3];
		Vector3 leftHandle = Handles.Slider(handlePoints[4], Vector3.left, capSize * HandleUtility.GetHandleSize(handlePoints[4]), Handles.CubeCap, 0.1f) - handlePoints[4];
		Vector3 rightHandle = Handles.Slider(handlePoints[5], Vector3.right, capSize * HandleUtility.GetHandleSize(handlePoints[5]), Handles.CubeCap, 0.1f) - handlePoints[5];

	}


	private void ShowManipulatorStrength(Vector3 position, Quaternion rotation, float strength, float size, MANIPULATORSHAPE shape){

		Handles.color = new Color(0, 0.75f, 1, 0.05f);;
		switch(m_ManipulatorObject.type)
		{
			case MANIPULATORTYPE.Attraction:
				if (shape == MANIPULATORSHAPE.Sphere){
					Handles.SphereCap( 0, position, rotation, strength * size );
				}
				if (shape == MANIPULATORSHAPE.Box){
					Handles.CubeCap( 0, position, rotation, (strength * size) * 0.5f );
				}
				break;
			case MANIPULATORTYPE.Repellent:
				if (shape == MANIPULATORSHAPE.Sphere){
					Handles.SphereCap( 0, position, rotation, (size * 2) + (strength * size) * 0.5f );
				}
				if (shape == MANIPULATORSHAPE.Box){
					Handles.CubeCap( 0, position, rotation, size + (strength * size) * 0.5f );
				}
				break;
			case MANIPULATORTYPE.VortexAttraction:

				if (shape == MANIPULATORSHAPE.Sphere){
					Handles.SphereCap( 0, position, rotation, strength * size );
				}
				if (shape == MANIPULATORSHAPE.Box){
					Handles.CubeCap( 0, position, rotation, (strength * size) * 0.5f );
				}

				Handles.color = new Color(0, 0.75f, 1, 1.0f);
				if (shape == MANIPULATORSHAPE.Box){
					Handles.DrawLine(position, (position + (m_ManipulatorObject.transform.forward * size) * 0.75f) );
				}
				else{
					Handles.DrawLine(position, (position + (m_ManipulatorObject.transform.forward * size) * 1.25f) ); // Quaternion.Euler(Vector3.up)
				}
				
				break;
		}
	}


}




