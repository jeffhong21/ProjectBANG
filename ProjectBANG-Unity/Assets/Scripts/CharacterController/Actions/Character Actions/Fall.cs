namespace CharacterController
{
    using UnityEngine;
    using System.Collections;


    public class Fall : CharacterAction
    {
        [SerializeField]
        protected float m_MinFallHeight = 1f;

        private Vector3 startFallPosition;
        private Vector3 endFallPosition;
        private float m_Heightfall;

        private RaycastHit groundCheck;
        //
        // Methods
        //

        public LayerMask groundLayer;
        RaycastHit hit;

        public override bool CanStartAction()
        {
            //if(m_Controller.Moving && m_Controller.Grounded)
            //{
            //    if (Physics.Raycast(m_Transform.position + (m_Transform.forward * 0.2f), Vector3.down, out hit, 3, m_Layers.GroundLayer))
            //    {
            //        Debug.DrawRay(m_Transform.position + (m_Transform.forward * 0.2f), Vector3.down * 3, Color.green);
            //    }
            //    else
            //    {
            //        Debug.DrawRay(m_Transform.position + (m_Transform.forward * 0.2f), Vector3.down * 3, Color.white);
            //        return true;
            //    }
            //}
            if(m_Controller.Grounded == false && Mathf.Abs(m_Rigidbody.velocity.y) > m_MinFallHeight)
            {
                return true;
            }

            return false;
        }


		public override bool CanStopAction()
		{
            if (m_Controller.Grounded)
                return true;
            if (m_Rigidbody.velocity.y <= m_MinFallHeight){
                return true;
            }

            return false;
		}


		protected override void ActionStarted()
        {
            m_AnimatorMonitor.SetActionID(2);
            //startFallPosition = m_Transform.position;
            //m_Heightfall = 0;
        }


        //  Returns the state the given layer should be on.
        public override string GetDestinationState(int layer)
        {
            if(layer == 0){
                //return "JumpingDown.JumpingDown";
                return "Fall";
            }
            return "";
        }



        protected override void ActionStopped()
        {
            //endFallPosition = m_Transform.position;
            //m_Heightfall = Vector3.Distance(startFallPosition, endFallPosition);

            //m_Heightfall = Mathf.Abs(m_Rigidbody.velocity.y);
            m_AnimatorMonitor.SetActionID(0);
        }


        //GUIStyle style = new GUIStyle();
        //GUIContent content = new GUIContent();
        //Vector2 size;
        ////Color debugTextColor = new Color(0, 0.6f, 1f, 1);
        //GUIStyle textStyle = new GUIStyle();
        //Rect location = new Rect();
        //private void OnGUI()
        //{
        //    if (Application.isPlaying)
        //    {
        //        GUI.color = Color.black;
        //        textStyle.fontStyle = FontStyle.Bold;

        //        content.text = "";
        //        //content.text += string.Format("Hit: {0}\n", hit.transform.name);

        //        size = new GUIStyle(GUI.skin.label).CalcSize(content);
        //        location.Set(5, 15 + size.y * 2, size.x * 2, size.y * 2);
        //        GUILayout.BeginArea(location, GUI.skin.box);
        //        GUILayout.Label(content);
        //        //GUILayout.Label(string.Format("Normalized Time: {0}", normalizedTime.ToString()));
        //        GUILayout.EndArea();
        //    }

        //}
	}

}

