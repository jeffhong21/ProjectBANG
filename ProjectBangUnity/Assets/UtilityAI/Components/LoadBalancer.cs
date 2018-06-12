namespace UtilityAI
{
    using UnityEngine;
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;


    public class LoadBalancer : SingleInstanceComponent<LoadBalancer>
    {




        protected override void OnAwake()
        {
            
        }

        private void Update()
        {
            
        }



    }










    public abstract class SingleInstanceComponent<T> : MonoBehaviour where T : MonoBehaviour
    {
        //
        // Static Fields
        //
        //private static int _instanceMark;

        //
        // Constructors
        //
        //protected SingleInstanceComponent();

        //
        // Methods
        //
        private void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            
        }

        protected virtual void OnDestroy()
        {
            
        }
    }






}

