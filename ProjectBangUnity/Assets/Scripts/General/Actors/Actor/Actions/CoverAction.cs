namespace Bang.CharacterActions
{
    using UnityEngine;
    using System.Collections;


    public class CoverAction : CharacterActions
    {
        public enum CoverIDs { StandStill, StandPopLeft, StandPopRight }


        float m_TakeCoverDistace = 0.75f;
        LayerMask m_CoverLayer;
        float m_TakeCoverRotationSpeed = 4;
        float m_CoverOffset = 0.05f;
        Vector3 m_StrafeCoverOffset = new Vector3(0.1f, 0, 0.5f);
        Vector3 m_PopDistance = new Vector3(0.3f, 0, 3);

        RaycastHit m_Hit;

        Transform m_HighCoverDetector;
        Transform m_LowCoverDetector;
        Transform m_RightCoverPopup;
        Transform m_LeftCoverPopup;
        float m_LowCoverHeight = 0.5f;
        float m_HighCoverHeight = 1f;




        public CoverIDs CurrentCoverID{
            get;
            private set;
        }




		protected override void Awake(){
            base.Awake();

            m_CoverLayer = Layers.cover;

            m_HighCoverDetector = CreateDetectors("HighCoverDetector", Vector3.up * m_HighCoverHeight);
            m_LowCoverDetector = CreateDetectors("HighCoverDetector", Vector3.up * m_LowCoverHeight);
            m_RightCoverPopup = CreateDetectors("HighCoverDetector", Vector3.right);
            m_LeftCoverPopup = CreateDetectors("HighCoverDetector", Vector3.left);
		}


        private Transform CreateDetectors(string detectorName, Vector3 position)
        {
            var detector = new GameObject().transform;
            detector.name = detectorName;
            detector.parent = m_Transform;
            detector.localPosition = position;
            detector.localEulerAngles = Vector3.zero;
            return detector;
        }


        public override bool CanStartAction()
        {
            if (Physics.Raycast(m_Transform.position, m_Transform.forward, out m_Hit, m_TakeCoverDistace, m_CoverLayer, QueryTriggerInteraction.Collide)) {
                if (m_Hit.transform.GetComponent<BoxCollider>()){
                    return true;
                }
            }
            return false;
        }

        public override bool CanStopAction()
        {
            throw new System.NotImplementedException();
        }


		protected override void ActionStarted()
        {
            throw new System.NotImplementedException();
        }

        protected override void ActionStopped()
        {
            throw new System.NotImplementedException();
        }


        public override bool Move()
        {
            throw new System.NotImplementedException();
        }


        public override bool UpdateRotation()
        {
            Quaternion targetRot = Quaternion.FromToRotation(transform.forward, m_Hit.normal) * transform.rotation;
            float angel = Vector3.Angle(m_Hit.normal, -transform.forward);


            return true;
        }













        //protected Transform leftHelper;
        //protected Transform rightHelper;

        //private void InitializeCoverMarkers()
        //{
        //    leftHelper = new GameObject().transform;
        //    leftHelper.name = "Left cover Helper";
        //    leftHelper.parent = transform;
        //    leftHelper.localPosition = Vector3.zero;
        //    leftHelper.localEulerAngles = Vector3.zero;

        //    rightHelper = new GameObject().transform;
        //    rightHelper.name = "Right cover Helper";
        //    rightHelper.parent = transform;
        //    rightHelper.localPosition = Vector3.zero;
        //    rightHelper.localEulerAngles = Vector3.zero;
        //}


        //public void EnterCover(CoverObject cover)
        //{
        //    Color debugColor = Color.red;
        //    if (cover == null) return;

        //    float minCoverHeight = AimOrigin.y * 0.75f;
        //    float maxDistance = 1f;     //  Max distance away from cover.

        //    Vector3 origin = AimOrigin;
        //    Vector3 directionToCover = -(transform.position - cover.transform.position);
        //    //directionToCover.y = minCoverHeight;
        //    RaycastHit hit;



        //    if (Physics.Raycast(origin, directionToCover, out hit, maxDistance, Layers.cover))
        //    {
        //        //  We hit a box collider
        //        if (hit.transform.GetComponent<BoxCollider>())
        //        {
        //            Quaternion targetRot = Quaternion.FromToRotation(transform.forward, hit.normal) * transform.rotation;
        //            float angel = Vector3.Angle(hit.normal, -transform.forward);
        //            //Debug.Log(angel);
        //            transform.rotation = targetRot;

        //            States.CanShoot = true;
        //            States.InCover = true;

        //            AnimHandler.EnterCover();
        //            Debug.Log("Agent is taking cover");
        //            debugColor = Color.green;
        //        }
        //    }

        //    Debug.DrawRay(origin, directionToCover, debugColor, 0.5f);
        //    //Debug.Break();
        //}



        //public void ExitCover()
        //{
        //    States.InCover = false;
        //    States.CanShoot = true;
        //    AnimHandler.ExitCover();
        //    Debug.Log("Agent is leaving cover");
        //}


        //#region Cover

        //public void EnterCover()
        //{
        //    CoverObject cover = FindClosestCover();
        //    //EnterCover(cover);
        //}


        //private CoverObject FindClosestCover()
        //{
        //    CoverObject closestCover = null;
        //    float mDist = float.MaxValue;
        //    Collider[] colliders = Physics.OverlapSphere(transform.position, 2, Layers.cover);

        //    for (int i = 0; i < colliders.Length; i++)
        //    {
        //        var col = colliders[i];
        //        if (col == null || col.gameObject == gameObject)
        //        {
        //            continue;
        //        }

        //        if (col.GetComponent<CoverObject>())
        //        {
        //            float tDist = Vector3.Distance(colliders[i].transform.position, position);
        //            if (tDist < mDist)
        //            {
        //                mDist = tDist;
        //                closestCover = colliders[i].GetComponent<CoverObject>();
        //            }
        //        }
        //    }
        //    return closestCover;
        //}


        //public void CheckIfCanEmerge()
        //{
        //    if (CanEmergeFromCover(rightHelper, true))
        //    {
        //        Debug.Log("Can emerge from right.");
        //    }
        //    else if (CanEmergeFromCover(leftHelper, false))
        //    {
        //        Debug.Log("Can emerge from left.");
        //    }
        //    else
        //    {
        //        Debug.Log("Cannot emerge");
        //    }
        //    //Debug.Break();
        //}


        //public bool CanEmergeFromCover(Transform helper, bool right)
        //{
        //    float entitySize = 0.5f;
        //    float distOffset = entitySize * 0.5f;
        //    Vector3 origin = transform.position;
        //    Vector3 side = (right == true) ? transform.right : -transform.right;
        //    //side.y = origin.y;
        //    Vector3 direction = side - origin;
        //    Vector3 helpPosition = side + (direction.normalized * 0.025f);
        //    helpPosition.y = 1f;
        //    helper.localPosition = helpPosition;
        //    Vector3 outDir = (-helper.transform.forward) + helper.position;


        //    float scanDistance = (outDir - helper.position).magnitude;
        //    RaycastHit hit;

        //    if (Physics.Raycast(helper.position, outDir, out hit, scanDistance, Layers.cover))
        //    {
        //        Debug.DrawLine(helper.position, outDir, Color.red, 1f);
        //        Debug.Log(helper.name + " hit " + hit.transform.name);
        //        return false;
        //    }

        //    Debug.DrawLine(helper.position, outDir, Color.green, 1f);
        //    return true;
        //}

        //# endregion

    }

}

