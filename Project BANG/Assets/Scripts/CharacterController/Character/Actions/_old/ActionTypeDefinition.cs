namespace CharacterController
{
    public enum ActionTypeDefinition
    {
        
        Idle = 0,
        StartMovement = 1,
        StopMovement = 2,
        QuickTurn = 3,
        Fall = 4,
        Crouch = 5,
        Pickup = 6,
        HitReaction = 7,
        Knockdown = 8,
        GetUp = 9,
        Die = 10,

        Punch = 11,


        Jump = 21,
        Roll = 22,
        Slide = 23,
        Cover = 24,
        Vault = 25,
        Climb = 26
    };

    public enum ItemActionTypeDefinition
    {
        MeleeAttack,
        Aim,
        Shoot,
        Reload,
        Equip,
        Unequip,
        Drop,
        Throw,
    };
}