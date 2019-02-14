using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

namespace CopyComponents
{
    [System.Serializable]
    public class ComponentToAdd : IEquatable<ComponentToAdd>
    {
        Component component;
        FieldInfo[] fieldsToCopy;
        string componentName;

        public FieldInfo[] FieldAttrs{
            get { return fieldsToCopy; }
            set { fieldsToCopy = value; }
        }

        public Component Component{
            get { return component; }
        }

        public string ComponentName{
            get { return componentName; }
        }

        public ComponentToAdd(Component _component){
            component = _component;
            componentName = component.GetType().ToString();

            SetFieldAttr(component);
        }

        public void SetFieldAttr(Component componentType){
            fieldsToCopy = componentType.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        }

        public bool Equals(ComponentToAdd other){
            return this.component == other.component;
        }

    }




}





