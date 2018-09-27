using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace CM_TaskSystem
{
    public class CM_WorkerTaskAI : MonoBehaviour
    {
        private enum State{
            WaitingForNextTask,
            ExecutingTask,
        }

        private CM_IWorker worker;
        private CM_TaskSystem taskSystem;
        private State state;
        private float waitingTimer;


        public void Setup(CM_IWorker worker, CM_TaskSystem taskSystem){
            this.worker = worker;
            this.taskSystem = taskSystem;
            state = State.WaitingForNextTask;
        }


		private void Update()
		{
            switch(state)
            {
                case State.WaitingForNextTask:
                    //  Waiting to request a new task
                    waitingTimer -= Time.deltaTime;
                    if(waitingTimer <= 0)
                    {
                        float waitingTimerMax = 0.2f; // 200ms
                        waitingTimer = waitingTimerMax;
                        RequestNextTask();
                    }
                    break;
                case State.ExecutingTask:
                    break;
            }
		}


        private void RequestNextTask()
        {
            CM_TaskSystem.Task task = taskSystem.RequestNextTask();
            if(task == null){
                state = State.WaitingForNextTask;
            } else {
                ExecuteTask(task);
            }
        }


        private void ExecuteTask(CM_TaskSystem.Task task){
            //  Execute Task;

        }
	}
}