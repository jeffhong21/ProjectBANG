//namespace CharacterController
//{
//    using UnityEngine;


//    public class Shoot : CharacterAction
//    {

//        protected ShootableWeapon m_ShootableWeapon;

//		//
//		// Methods
//		//
//		public override bool CanStartAction()
//		{
//            if(m_Controller.Aiming)
//                return base.CanStartAction();
//            return false;
//		}


//		protected override void ActionStarted()
//        {
//            if(CameraController.Instance != null){
//                m_ShootableWeapon = (ShootableWeapon)m_Inventory.GetCurrentItem(m_Inventory.EquippedItemType);
//                Ray ray = CameraController.Instance.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
//                RaycastHit hit;
//                if(Physics.Raycast(ray.origin, ray.direction, out hit, 50f, m_ShootableWeapon.ImpactLayers)){
//                    m_ShootableWeapon.SetFireAtPoint(hit.point);
//                    m_Inventory.UseItem(m_Inventory.EquippedItemType, 1);
//                    return;
//                }
//            }
//            m_Inventory.UseItem(m_Inventory.EquippedItemType, 1);
//            Debug.Log("Shoot Action raycast did not hit anything to provide gun a cheat target.");
//        }

//        protected override void ActionStopped()
//        {
//            //Debug.Log("Shooting action done");
//        }
//    }

//}