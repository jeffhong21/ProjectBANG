using UnityEngine;
using System.Collections.Generic;

public class CircularListScript : MonoBehaviour
{
    public CircularList<string> list = new CircularList<string>(5);

    public List<string> refList = new List<string>(5);
    public string first;
    public int headIndex;

    private void OnValidate()
    {
        for (int i = 0; i < refList.Count; i++) {
            list[i] = refList[i];
        }

        first = list[list.HeadIndex];
        headIndex = list.HeadIndex;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) Debug.Log(list[0]);

        if (Input.GetKeyDown(KeyCode.Alpha1)) Debug.Log(list[1]);

        else if (Input.GetKeyDown(KeyCode.Alpha2)) Debug.Log(list[2]);

        else if (Input.GetKeyDown(KeyCode.Alpha3)) Debug.Log(list[3]);

        else if (Input.GetKeyDown(KeyCode.Alpha4)) Debug.Log(list[4]);

        else if (Input.GetKeyDown(KeyCode.Alpha5)) Debug.Log(list[5]);

        if (Input.GetKeyDown(KeyCode.Alpha7)) Debug.Log(list.Current);

        if (Input.GetKeyDown(KeyCode.Alpha8)) Debug.Log(list.NextOrFirst());

        if (Input.GetKeyDown(KeyCode.Alpha9)) Debug.Log(list.PreviousOrLast());
    }
}