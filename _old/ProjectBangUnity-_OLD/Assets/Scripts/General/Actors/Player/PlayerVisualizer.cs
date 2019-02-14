namespace Bang
{
    using UnityEngine;

    public class PlayerVisualizer : MonoBehaviour
    {
        private PlayerController playerCtrl;
        private PlayerInputHandler playerInput;
        [SerializeField]
        private Color playerAimColor = new Color(0, 1f, 1f, 0.5f);
        [SerializeField]
        private Color weaponAimColor = new Color(1, 0.3f, 0, 0.5f);

        [SerializeField]
        private bool onDrawSelected = true;




        Vector3 cursorPosition;
        Vector3 weaponShootDir;
        float distanceToCursor;



        private void Awake()
        {
            playerCtrl = GetComponent<PlayerController>();
            playerInput = GetComponent<PlayerInputHandler>();
        }


        private void OnDrawGizmos()
        {
            if (!onDrawSelected)
                DrawGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            if (onDrawSelected)
                DrawGizmos();
        }


        private void DrawGizmos()
        {
            cursorPosition = playerInput.CursorPosition;
            cursorPosition.y = playerCtrl.AimOrigin.y;
            Gizmos.color = playerAimColor;
            Gizmos.DrawLine(playerCtrl.AimOrigin, cursorPosition);
            Gizmos.DrawLine(cursorPosition, playerInput.CursorPosition);

            Gizmos.color = weaponAimColor;
            distanceToCursor = Vector3.Distance(playerCtrl.weapon.ProjectileSpawn.position, cursorPosition);
            weaponShootDir = playerCtrl.weapon.ProjectileSpawn.position + playerCtrl.weapon.ProjectileSpawn.forward * distanceToCursor;
            Gizmos.DrawLine(playerCtrl.weapon.ProjectileSpawn.position, weaponShootDir);
            Gizmos.DrawLine(weaponShootDir, playerInput.CursorPosition);
        }
    }

}

