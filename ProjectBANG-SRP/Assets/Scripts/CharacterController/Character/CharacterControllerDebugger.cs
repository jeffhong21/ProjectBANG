using UnityEngine;
using System;

[Serializable]
public class CharacterControllerDebugger
{

    public Color moveDirectionColor = Color.blue;

    public Color velocityColor = Color.green;




    [SerializeField]
    protected Vector3 debugHeightOffset = new Vector3(0, 0.25f, 0);







    public void OnDrawGizmos()
    {

    }

}
