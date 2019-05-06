namespace CharacterController
{
    public enum ActionTypeDefinition
    {
        Idle = 0,
        Sprint = 1,
        Fall = 3,
        TakeDamage = 9,
        Death = 10,
        PickupItem = 11,
        Cover = 12,
        Vault = 15,
        Climb = 16,
        Roll = 18,
        Slide = 19
    };

    public enum ItemActionTypeDefinition
    {
        Aim,
        Reload
    };
}