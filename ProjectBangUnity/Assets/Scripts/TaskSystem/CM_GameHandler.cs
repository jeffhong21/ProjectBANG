using UnityEngine;
using System.Collections;
using System.Collections.Generic;




namespace CM_TaskSystem
{
    public class CM_GameHandler : MonoBehaviour
    {

		private void Start()
		{
            CM_TaskSystem taskSystem = new CM_TaskSystem();

            //Debug.Log(taskSystem.RequestNextTask());  //  Should print null;
            //CM_TaskSystem.Task task = new CM_TaskSystem.Task();
            //taskSystem.AddTask(task);
            //Debug.Log(taskSystem.RequestNextTask());  //  Should print task;
            //Debug.Log(taskSystem.RequestNextTask());  //  Should print null;

            //CM_Worker worker = CM_Worker.Create(new Vector3(500, 500));
            //CM_WorkerTaskAI workerTaskAI = workerTaskAI.gameObject.AddComponent<CM_WorkerTaskAI>();
            //workerTaskAI.Setup(worker);
		}



	}
}