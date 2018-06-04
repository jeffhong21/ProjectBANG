using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
///  This script will copy all components and add it to all the target objects.
///
///
///  - AllComponents:  Will add selected components, even if target objects has component.
///  - OnlyNew:  Will add selected components, but only if the target object does not have that component.
///  - OnlyValues:  Will copy values from selected components onto target objects components only if target has component.
///
///
///
///  v1.0   basic copy component to multiple targets.
///  v2.0   able to reorder the components in the source object.
///
///
/// </summary>

namespace CopyComponents
{
    
    public class CopyComponentsWindow : EditorWindow
    {

        private enum CopyOptions { AllComponents, OnlyNew, OnlyValues };
        

        private GameObject m_SrcObject;
        private ComponentToCopy m_SrcTransform;
        //  List structure of components on source object.  (bool, component)
        private List<ComponentToCopy> m_SrcComponents = new List<ComponentToCopy>();
        //  List of componets to add and values to copy.
        private List<ComponentToAdd> m_ComponentsToAdd  = new List<ComponentToAdd>();
        //  List of all the target objects
        private List<GameObject> m_TargetObjects = new List<GameObject>();
        //  Each targets list of components to add.
        private Dictionary<GameObject, List<ComponentToAdd>> m_TargetComponentsToAdd = new Dictionary<GameObject, List<ComponentToAdd>>();
        //  Reorderable list.
        private ReorderableList m_List;



        private CopyOptions m_CopyOptions = CopyOptions.OnlyNew;
        private bool m_AllComponents = true;

        private bool debug = true;

        #region Window Functions
        [MenuItem("JH Tools/Copy Components")]
        private static void OpenWindow()
        {
            CopyComponentsWindow window = (CopyComponentsWindow)GetWindow(typeof(CopyComponentsWindow));

            window.titleContent = new GUIContent("Copy Components Tool");
            window.minSize = new Vector2(450, 400);
            window.maxSize = new Vector2(450, 400);
            window.Show();

            //window.ShowUtility();
            GUI.BringWindowToFront(0);

        }


        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayoutOption GUIHeight = GUILayout.Height(Screen.height - 74);

            DrawSourceObjectGUI(GUIHeight);
            DrawTargetObjectGUI(GUIHeight);
            EditorGUILayout.EndHorizontal();
            DrawCopyComponentOptionGUI();
        }


        /// ---------------------------------------------------------------------------------------------------------------
        /// ---------------------------------------------------------------------------------------------------------------
        /// ---------------------------------------------------------------------------------------------------------------

		private void OnEnable()
		{
            if (m_SrcObject == null){
                return;
            }
            InitializeList();
		}


        //  ** CURRENTLY LIST DOESN'T GIVE CORRECT COMPONENT ORDER. **
        //  Updates Source Component List and the Transform.
        private void UpdateComponentList()
        {
            if (m_SrcObject != null){
                m_SrcComponents.Clear();
                //  -- Go through the source object list of components.
                for (int index = 0; index < m_SrcObject.GetComponents<Component>().Length; index++){
                    //  -- Check to see if list contains the component.  Otherwise it will keep on adding.
                    Component srcComponent = m_SrcObject.GetComponents<Component>()[index];
                    ComponentToCopy srcComponentToCopy = new ComponentToCopy(true, srcComponent);
                    int arrayIndex = Array.IndexOf(m_SrcObject.GetComponents<Component>(), srcComponent);

                    
                    m_SrcComponents.Add(srcComponentToCopy);

                    // if (m_SrcComponents.Contains(srcComponentToCopy) == false){
                    //     //m_SrcComponents.Add(srcComponentToCopy);
                    //     m_SrcComponents.Insert(arrayIndex, srcComponentToCopy);
                    // }
                }
                PrintUpdatedList();
            }
        }


        private void PrintUpdatedList(){
            string info = "Source Component Order\n";
            for (int i = 0; i < m_SrcComponents.Count; i ++){
                info += m_SrcComponents[i].ComponentName +"\n";
            }

            Debug.Log(info);
        }


        #region ReorderableList
        //  -- Setup the lists of components.
        private void InitializeList()
        {
            UpdateComponentList();
            m_SrcComponents.RemoveAt(0);
            //  -- Create the ReorderableList
            m_List = new ReorderableList(m_SrcComponents, typeof(ComponentToCopy), false, true, false, false);

            //  Draw ReorderableList Header.
            m_List.drawHeaderCallback = (Rect rect) => {  
                m_SrcTransform = new ComponentToCopy(true, m_SrcObject.GetComponents<Component>()[0]);
                DrawElements(rect, m_SrcTransform );
            };

            //  Draw ReorderableList Element
            m_List.drawElementCallback =  (Rect rect, int index, bool isActive, bool isFocused) => {
                DrawElements(rect, m_SrcComponents[index]);
            };

             //  What happens when the lists changes.
            m_List.onReorderCallback = (ReorderableList internalList) => {
                //Move Down
                for (int i = 0; i < m_SrcComponents.Count; i++){
                    int listIndex = internalList.list.IndexOf(m_SrcComponents[i].Component);
                    int difference = listIndex - i;
                    if (difference > 0)
                        for (int j = 0; j < Mathf.Abs(difference); j++)
                            ComponentUtility.MoveComponentDown(m_SrcComponents[i].Component);
                }
                //Move Up
                UpdateComponentList();
                m_SrcComponents.RemoveAt(0);
                for (int i = m_SrcComponents.Count-1; i >= 0; i--){
                    int listIndex = internalList.list.IndexOf(m_SrcComponents[i].Component);
                    int difference = listIndex - i;
                    if (difference < 0)
                        for (int j = 0; j < Mathf.Abs(difference); j++)
                            ComponentUtility.MoveComponentUp(m_SrcComponents[i].Component);
                }
            };

        }


        private void DrawElements(Rect originalRect, ComponentToCopy component)
        {
            Rect rect = new Rect(originalRect);

            bool isRectTransform = component.Component is RectTransform;
            bool isTransform = component.Component is Transform || isRectTransform;

			if (isTransform){
				rect.x+=14;
				rect.y-=2;
			}

            //Enable/Disable Toggle Handler
            rect.x += 5;
            rect.y += 2;
            rect.width = rect.height = 20;
            component.IsCopyComponent = EditorGUI.Toggle(rect, component.IsCopyComponent);

			//  Component Icon
            rect.x += 20;
            
            Texture componentIcon = EditorGUIUtility.ObjectContent(null,component.Component.GetType() ).image;
            EditorGUI.LabelField(rect, new GUIContent(componentIcon) );


            //  Component name.
            rect.x += 20;
            //rect.y = originalRect.y;
            rect.width = originalRect.width - 30;
            EditorGUI.LabelField(rect,component.ComponentName);

        }

        #endregion


        /// ---------------------------------------------------------------------------------------------------------------
        /// ---------------------------------------------------------------------------------------------------------------
        /// ---------------------------------------------------------------------------------------------------------------






        //  -- THE LEFT SIDE
        private void DrawSourceObjectGUI(GUILayoutOption GUIHeight)
        {
			GUIStyle style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 12;

            //  -- START OF OBJECT SELECTION PANEL  
            EditorGUILayout.BeginVertical(new GUIStyle("HelpBox"), GUIHeight);
            
            //  -- SOURCE GAME OBJECT TO COPY FROM
            GUILayout.Label("Source Object (Copy From) : ", style);
            m_SrcObject = (GameObject)EditorGUILayout.ObjectField(m_SrcObject, typeof(GameObject), true);
            //  -- ALL COMPONENTS FROM GAME OBJECT
            GUILayout.Space(4);


            GUILayout.Label(" Source Components To Copy ", style);

            //  -- DRAW COMPONENTS
            //EditorGUILayout.BeginVertical();    //  -- LIST OF TOGGLES BEGINVERTICAL
            if(m_SrcObject != null){
                if (m_List == null || m_List.list == null){
                    try{
                        InitializeList();
                    }
                    catch{
                    }
                }
                else{
                    // --  Begin Change Check.
                    EditorGUI.BeginChangeCheck();  
                    m_AllComponents = GUILayout.Toggle(m_AllComponents, "All");

                    //  Draw Reorderable list
                    if (m_List != null && m_List.list != null){
                        m_List.DoLayoutList();
                    }

                    // --  End Change Check.
                    if(EditorGUI.EndChangeCheck()){
                        // if All toggle is selected, set all components to true.
                        if(m_AllComponents == true){
                            for (int index = 0; index < m_SrcComponents.Count; index++){
                                m_SrcComponents[index].IsCopyComponent = true;
                            }
                        }
                    }
                    // --  End Change Check.
                }
            }
            //  -- LIST OF TOGGLES ENDVERTICAL
            //EditorGUILayout.EndVertical();                                  


            // if (GUILayout.Button(" Add Selected ", new GUILayoutOption[] { GUILayout.Height(24) })){
            //     GameObject selection = Selection.activeGameObject;
            //     if (selection != null){
            //         m_SrcObject = selection;
            //         Repaint();
            //     }
            //     else{
            //         m_SrcObject = null;
            //     }
            // }

            //  -- END OBJECT SELECTION PANEL
            EditorGUILayout.EndVertical();
        }











        //  -- THE RIGHT SIDE
        private void DrawTargetObjectGUI(GUILayoutOption GUIHeight)
        {
            //  -- START OF OBJECT SELECTION PANEL  
            EditorGUILayout.BeginVertical(new GUIStyle("HelpBox"), GUIHeight);
            //  -- TARGETS TO 
            GUILayout.Label("Targets (Paste To): ");

            // Displays the all the target objects ObjectField UI
            for (int i = 0; i < m_TargetObjects.Count; i++){
                m_TargetObjects[i] = (GameObject)EditorGUILayout.ObjectField(m_TargetObjects[i], typeof(GameObject), true);
            }

            //  -- ALL THE BUTTONS FOR ADDING OR REMOVING OBJECTS
            int buttonGUIHeight = 24;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(" + ", new GUILayoutOption[] { GUILayout.Height(buttonGUIHeight) })){
                //Debug.Log("Add " + Selection.activeGameObject + " target objects");
                AddTargetObjects();
            }

            if (GUILayout.Button(" - ", new GUILayoutOption[] { GUILayout.Height(buttonGUIHeight) })){
                //Debug.Log("Remove " + Selection.activeGameObject + " from target objects list");
                RemoveTargetObjects();
            }

            if (GUILayout.Button("Clear Objects", new GUILayoutOption[] { GUILayout.Height(buttonGUIHeight) })){
                //Debug.Log("Clear All Objects");
                ClearTargetObjects();
            }

            EditorGUILayout.EndHorizontal();

            //  -- END OBJECT SELECTION PANEL
            EditorGUILayout.EndVertical();
        }


        //  -- THE BUTTONS
        private void DrawCopyComponentOptionGUI()
        {
            EditorGUILayout.BeginVertical();

            m_CopyOptions = (CopyOptions)EditorGUILayout.EnumPopup(new GUIContent("Copy Options: "), m_CopyOptions);

            if(debug){
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Copy Components", GUILayout.Height(24))){
                    CopyComponentsToTarget();
                }
                if (GUILayout.Button("PRINT DEBUG", GUILayout.Height(24))){
                    DebugMessage();
                }

                EditorGUILayout.EndHorizontal();
            }
            else{
                if (GUILayout.Button("Copy Components", GUILayout.Height(24))){
                    CopyComponentsToTarget();
                }
            }

            EditorGUILayout.EndVertical();
        }







        // // Displays the all the target objects ObjectField UI
        // private void DrawTargetObjectGUI()
        // {
        //     for (int i = 0; i < m_TargetObjects.Count; i++){
        //         m_TargetObjects[i] = (GameObject)EditorGUILayout.ObjectField(m_TargetObjects[i], typeof(GameObject), true);
        //     }
        // }


        private void AddTargetObjects()
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++){
                if (!m_TargetObjects.Contains(Selection.gameObjects[i])){
                    m_TargetObjects.Add(Selection.gameObjects[i]);
                }
            }
            Repaint();
        }


        private void RemoveTargetObjects()
        {
            //  Remove targets from selection.
            if(Selection.gameObjects.Length > 0){
                foreach (GameObject targetObj in Selection.gameObjects){
                    //  If selection is in targetObjects
                    if (m_TargetObjects.Contains(targetObj)){
                        m_TargetObjects.Remove(targetObj);
                    }
                    //  If not remove the last index.
                    else{
                        m_TargetObjects.RemoveAt(m_TargetObjects.Count - 1);
                    }
                }
            }
            //  Remove the last target.
            else{
                if (m_TargetObjects.Count > 0){
                    m_TargetObjects.RemoveAt(m_TargetObjects.Count - 1);
                }
            }
            Repaint();
        }


        private void ClearTargetObjects()
        {
            m_TargetObjects.Clear();
            Repaint();
        }
        #endregion


        //  -- ADD ALL SELECTED COMPONEENTS ONTO TARGET OBJECTS.
        private void CopyComponentsToTarget()
        {
            //  Add all checked components into a new list and get their values.
            m_ComponentsToAdd = GetAllComponentsToCopy(m_SrcObject, m_SrcComponents);
            //  Get List of targets and the components to add to each target.
            m_TargetComponentsToAdd = GetListOfComponentsToCopyForTarget(m_TargetObjects, m_ComponentsToAdd);

            //  Loop through each target and add components.
            foreach(GameObject target in m_TargetObjects)
            {
                List<ComponentToAdd> targetComponentsToAdd = m_TargetComponentsToAdd[target];

                //  Add components.
                for(int i = 0; i < targetComponentsToAdd.Count; i ++)
                {
                    Component componentToAdd = targetComponentsToAdd[i].Component;
                    FieldInfo[] fieldAttrs = targetComponentsToAdd[i].FieldAttrs;

                    switch(m_CopyOptions)
                    {
                        case CopyOptions.AllComponents:
                            CopyAllComponents(target, componentToAdd, fieldAttrs);
                            break;
                        case CopyOptions.OnlyNew:
                            CopyOnlyNew(target, componentToAdd, fieldAttrs);
                            break;
                        case CopyOptions.OnlyValues:
                            CopyOnlyValues(componentToAdd, fieldAttrs);
                            break;
                    }
                }
            }
        }


        private void CopyAllComponents(GameObject target, Component componentToAdd, FieldInfo[] fieldAttrs)
        {
            //Component newComponent = target.AddComponent(componentToAdd.GetType());

            UnityEditorInternal.ComponentUtility.CopyComponent(componentToAdd);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(target);
            UnityEditorInternal.ComponentUtility.PasteComponentValues(componentToAdd);
        }


        private void CopyOnlyNew(GameObject target, Component componentToAdd, FieldInfo[] fieldAttrs)
        {
            UnityEditorInternal.ComponentUtility.CopyComponent(componentToAdd);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(target);
            UnityEditorInternal.ComponentUtility.PasteComponentValues(componentToAdd);
        }


        private void CopyOnlyValues(Component componentToAdd, FieldInfo[] fieldAttrs)
        {
            UnityEditorInternal.ComponentUtility.CopyComponent(componentToAdd);
            UnityEditorInternal.ComponentUtility.PasteComponentValues(componentToAdd);
        }


        private void SetFieldAttr(Component componentToAdd, Component newComponent, FieldInfo[] fieldAttrs)
        {
            if (fieldAttrs.Length > 0)
            {
                //  Add values to newly added component.
                foreach(FieldInfo field in fieldAttrs)
                {
                    //  If you are using private serialized variables.  Remove if something goes wrong.
                    if(field.IsPublic || field.GetCustomAttributes(typeof(SerializeField),true).Length != 0)
                    {
                        //  Get the value from the component that was added.
                        object value = field.GetValue(componentToAdd);
                        //  Set the values of the newly added component.
                        field.SetValue(newComponent, value);
                    }
                }
            }
        }



        //  Get all checked components and put into list.  ComponentToAdd class will get all values.
        private List<ComponentToAdd> GetAllComponentsToCopy(GameObject sourceObject, List<ComponentToCopy> componentsToCopy)
        {
            List<ComponentToAdd> componentsToAdd = new List<ComponentToAdd>();
            if (sourceObject != null){
                for (int k = 0; k < componentsToCopy.Count; k++)
                {
                    if(componentsToCopy[k].IsCopyComponent)
                    {
                        //  Component from the source object
                        Component srcComponentToAdd = componentsToCopy[k].Component;
                        
                        if(srcComponentToAdd.GetType() == typeof(ParticleSystem)){
                            continue;
                        }

                        ComponentToAdd componentToAdd = new ComponentToAdd(srcComponentToAdd);
                        componentToAdd.SetFieldAttr(srcComponentToAdd);
                        componentsToAdd.Add(componentToAdd);
                    }
                }
            }
            return componentsToAdd;
        }


        //  Returns a dictionary of target gameObjects and its componentsToCopy.
        private Dictionary<GameObject, List<ComponentToAdd>> GetListOfComponentsToCopyForTarget(List<GameObject> targetObjects, List<ComponentToAdd> srcComponentsToAdd)
        {
            Dictionary<GameObject, List<ComponentToAdd>> targetComponentsToAdd = new Dictionary<GameObject, List<ComponentToAdd>>();
            List<ComponentToAdd> componentsToAdd = new List<ComponentToAdd>();

            //  Loop through each target object and check if target contains components from src target.
            foreach(GameObject target in targetObjects)
            {
                //  Loop through each srcComponentsToAdd
                foreach(ComponentToAdd srcComponent in srcComponentsToAdd)
                {
                    if (m_CopyOptions == CopyOptions.OnlyNew || m_CopyOptions == CopyOptions.OnlyValues)
                    {
                        //  If target contains a component from srcComponentsToAdd, add it.
                        if(DoesTargetContainComponent(target, srcComponent.Component) == false)
                        {
                            componentsToAdd.Add(srcComponent);
                        }
                    }
                    else{
                        componentsToAdd.Add(srcComponent);
                    }


                }
                targetComponentsToAdd.Add(target, componentsToAdd);
            }

            //  Prints all the components that need to be added to target.
            // PrintComponentList( "List of Components to add to Target", componentsToAdd);
            
            return targetComponentsToAdd;
        }

        //  -- Checks if target contains the specified component.
        private bool DoesTargetContainComponent(GameObject target, Component componentType)
        {
            Component[] targetComponents = target.GetComponents(typeof(Component));

            foreach(Component targetComponent in targetComponents)
            {
                if (componentType.GetType() == targetComponent.GetType())
                {
                    return true;
                }
            }

            return false;
        }










        private void PrintComponentList(string header, List<ComponentToAdd> list)
        {
            string debugInfo = header + "\n";
            for(int k = 0; k < list.Count; k ++){
                debugInfo += list[k].Component.GetType().ToString() + "\n";
            }
            Debug.Log( debugInfo );
        }

        private void PrintComponentList(string header, List<ComponentToCopy> list)
        {
            string debugInfo = header + "\n";
            for(int k = 0; k < list.Count; k ++){
                debugInfo += list[k].Component.GetType().ToString() + "\n";
            }
            Debug.Log( debugInfo );
        }

        private void PrintComponentList(string header, Type[] list)
        {
            string debugInfo = header + "\n";
            for(int k = 0; k < list.Length; k ++){
                debugInfo += list[k].ToString() + "\n";
            }
            Debug.Log( debugInfo );
        }


        private void DebugMessage()
        {

            // Debug.Log("Source Components: " + m_SrcComponents.Count);
            // Debug.Log("Components To Add: " + m_ComponentsToAdd.Count);

            foreach(ComponentToAdd c in GetAllComponentsToCopy(m_SrcObject, m_SrcComponents) ){
                Debug.Log(c.ComponentName);
            }
        }





    }
}


