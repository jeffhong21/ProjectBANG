using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;




namespace CM_TaskSystem
{
    public class CM_TaskSystem
    {

        public class QueuedTask{

            private Func<Task> tryGetTaskFunc;

            public QueuedTask(Func<Task>tryGetTaskFunc){
                this.tryGetTaskFunc = tryGetTaskFunc;
            }

            public Task TryDequeueTask(){
                return tryGetTaskFunc();
            }
        }


        public class Task{
        }


        private List<Task> taskList;
        private List<QueuedTask> queuedTaskList;

        public CM_TaskSystem(){
            taskList = new List<Task>();
        }


        public Task RequestNextTask(){
            // Worker requesting a task
            if(taskList.Count > 0){
                //  Give worker the first task.
                Task task = taskList[0];
                taskList.RemoveAt(0);
                return taskList[0];
            } else {
                //  No tasks are available.
                return null;
            }
        }


        public void AddTask(Task task){
            taskList.Add(task);
        }


        public void EnqueueTask(Func<Task> tryGetTaskFunc){
            
        }


        private void DequeueTasks()
        {
            for (int i = 0; i < queuedTaskList.Count; i++)
            {
                QueuedTask queuedTask = queuedTaskList[i];
                Task task = queuedTask.TryDequeueTask();
                if(task != null){
                    //Task dequeued! AddTask to normal list
                    AddTask(task);
                    queuedTaskList.RemoveAt(i);
                    i--;
                } else {
                    //  Return task is null.
                }


            }
        }

    }
}