using UnityEngine;
using System.Collections;

namespace Bang
{
    public class CoroutineHelper : SingletonMonoBehaviour<CoroutineHelper>
    {

        public static Coroutine Start(IEnumerator routine)
        {
            return instance.StartCoroutine(routine);
        }

        public static void Stop(IEnumerator routine)
        {
            if(routine != null)
            {
                instance.StopCoroutine(routine);
            }

        }


    }

}
