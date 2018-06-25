using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UtilityAI
{
    
    public sealed class AIBuildingBlocks
    {
        //
        // Static Fields
        //
        private static readonly object _instanceLock;

        private static AIBuildingBlocks _instance;

        //
        // Fields
        //
        private List<NamedType> _selectors;

        private List<NamedType> _qualifiers;

        private List<NamedType> _actions;

        private Dictionary<Type, List<NamedType>> _customItems;

        //
        // Static Properties
        //
        public static AIBuildingBlocks Instance
        {
            get;
        }

        //
        // Properties
        //
        public IEnumerable<NamedType> actions
        {
            get;
        }

        public IEnumerable<NamedType> qualifiers
        {
            get;
        }

        public IEnumerable<NamedType> selectors
        {
            get;
        }

        //
        // Constructors
        //
        private AIBuildingBlocks()
        {

        }

        //
        // Static Methods
        //
        private static IEnumerable<Type> GetConstructableTypes()
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<Type> ReflectDerivatives(Type forType, IEnumerable<Type> relevantTypes)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<Type> ReflectDerivatives(Type forType)
        {
            throw new NotImplementedException();
        }

        private static List<NamedType> Wrap(IEnumerable<Type> types)
        {
            throw new NotImplementedException();
        }

        //
        // Methods
        //
        public IEnumerable<NamedType> GetForType(Type t)
        {
            throw new NotImplementedException();
        }

        private void Init()
        {
            
        }


        //
        // Nested Types
        //
        public class NamedType
        {
            public string friendlyName
            {
                get;
                internal set;
            }

            public string description
            {
                get;
                internal set;
            }

            public Type type
            {
                get;
                internal set;
            }

            internal NamedType(Type t){
                
            }
        }
    }
}
