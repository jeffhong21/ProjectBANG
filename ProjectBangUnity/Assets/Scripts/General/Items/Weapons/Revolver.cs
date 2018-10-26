namespace Bang
{
    using System.Collections;
    using UnityEngine;


    public class Revolver : Gun
    {


        protected override IEnumerator ReloadAnim(float time)
        {
            Material material = GetComponentInChildren<MeshRenderer>().material;
            Color originalColor = material.color;
            Color tempColor = Color.red;
            material.color = tempColor;

            float remainingTime = time;

            while(remainingTime > 0)
            {
                //Debug.Log(remainingTime + " before done rreloading");
                material.color = Color.Lerp(tempColor, originalColor, Time.deltaTime);
                remainingTime -= Time.deltaTime;
                yield return null;
            }

            material.color = originalColor;

            yield return null;
        }




        protected override IEnumerator Shoot(float time)
        {
            throw new System.NotImplementedException();
        }
    }
}


