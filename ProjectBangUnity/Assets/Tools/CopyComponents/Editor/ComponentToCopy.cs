using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

namespace CopyComponents
{
    [System.Serializable]
    public class ComponentToCopy : IEquatable<ComponentToCopy>
    {
        bool isCopyComponent;
        Component component;
        string componentName;

        public bool IsCopyComponent
        {
            get { return isCopyComponent; }
            set { isCopyComponent = value; }
        }

        public Component Component
        {
            get { return component; }
        }

        public string ComponentName
        {
            get { return componentName; }
        }

        public ComponentToCopy(Component _component)
        {
            component = _component;
            componentName = component.GetType().ToString();
        }

        public ComponentToCopy(bool _isCopyComponent, Component _component)
        {
            isCopyComponent = _isCopyComponent;
            component = _component;
            componentName = component.GetType().ToString();
        }

        public bool Equals(ComponentToCopy other)
        {
            return this.component == other.component;
        }

    }



}





