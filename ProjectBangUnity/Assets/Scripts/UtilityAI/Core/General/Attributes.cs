namespace UtilityAI
{
    using UnityEngine;
    using System;
    using System.Collections;


    /// <summary>
    /// Used to make a readonly property.  (Field cannot be edited)
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
        
    }





    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class AISerializationAttribute : Attribute
    {
        public string name { get; set; }

        public AISerializationAttribute(string name)
        {
            this.name = name;
        }
    }


    public class MemberEditorAttribute : Attribute
    {

    }

    ///<summary>
    /// An attribute to decorate AI entity fields and properties to assign them to a category when displayed in the AI editor inspector.
    ///</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class MemberCategoryAttribute : Attribute
    {
        public string name { get; private set; }
        public int sortOrder { get; private set; }

        public MemberCategoryAttribute(string name){
            this.name = name;
        }

        public MemberCategoryAttribute(string name, int sortOrder){
            this.name = name;
            this.sortOrder = sortOrder;
        }

    }




    public class DefaultAttribute : Attribute
    {
        
    }

}

