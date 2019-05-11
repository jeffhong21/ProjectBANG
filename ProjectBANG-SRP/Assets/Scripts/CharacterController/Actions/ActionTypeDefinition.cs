namespace CharacterController
{
    public enum ActionTypeDefinition
    {
        Death = -1,
        Idle = 0,
        Sprint = 1,
        Fall = 3,
        HitReaction = 7,
        Knockdown = 8,
        GetUp = 9,

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