using System;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityAI.Utilities
{
    public static class ComponentHelper
    {
        //
        // Static Methods
        //
        public static IEnumerable<T> FindAllComponentsInScene<T>() where T : Component
        {
            return UnityEngine.Object.FindObjectsOfType<T>();
        }

        public static T FindFirstComponentInScene<T>() where T : Component
        {
            return UnityEngine.Object.FindObjectOfType<T>();
        }
    }
}
