namespace Bang
{
    using System.Collections;
    using UnityEngine;


    public class Revolver : ShootableWeapon
    {

		//public override void Shoot()
		//{
		//	base.Shoot();

		//}



		protected override IEnumerator PlayReloadAnim(float time)
        {
            //Material material = GetComponentInChildren<MeshRenderer>().material;
            //Color originalColor = material.color;
            //Color tempColor = Color.red;
            //material.color = tempColor;

            //float remainingTime = time;

            //while(remainingTime > 0)
            //{
            //    //Debug.Log(remainingTime + " before done rreloading");
            //    material.color = Color.Lerp(tempColor, originalColor, Time.deltaTime);
            //    remainingTime -= Time.deltaTime;
            //    yield return null;
            //}

            //material.color = originalColor;


            yield return new WaitForSeconds(time);
        }




        protected override IEnumerator PlayShootAnim(float time)
        {
            throw new System.NotImplementedException();
        }
    }
}


