using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



namespace CM_TaskSystem
{
    public interface CM_IWorker
    {
        void MoveTo(Vector3 position, Action onArrivedAtPosition = null);


    }
}