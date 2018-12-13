namespace Bang
{
    using UnityEngine;


    ///<summary>
    ///  1.  Create a new camera and parent it under the main camera.!--  Remove MainCamera tag as well.
    ///  2.  Camera.ClearFlags should be "DepthOnly".!--  Set Depth to 1 so it renders on top of everything.
    ///  3.  Make sure Main Camera.CullingMask does not render UI, so it doesn't render it twice.
    ///  4.  This should also be set to UI Layer.
    ///<summary>
    public class Crosshairs : MonoBehaviour
    {
        [SerializeField]
        float rotationSpeed = -40;

		private void Update()
        {
            //if (GameManager.Instance.isPaused)
            //{
            //    Cursor.visible = true;
            //}
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }



		// public void DetectTargets(Ray ray)
		// {
		//  if (Physics.Raycast (ray, 100, targetMask))
		//  {
		//      dot.color = dotHighlightColor;
		//  }
		//  else
		//  {
		//      dot.color = originalDotColor;
		//  }
		// }





    }


}


