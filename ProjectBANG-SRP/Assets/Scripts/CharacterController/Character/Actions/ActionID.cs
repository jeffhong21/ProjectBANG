namespace CharacterController
{

    public static class ActionTypeID
    {

        public static readonly int Idle = 0;
        public static readonly int StartMovement = 1;
        public static readonly int StopMovement = 2;
        public static readonly int QuickTurn = 3;
        public static readonly int Fall = 4;
        public static readonly int Crouch = 5;
        public static readonly int Pickup = 6;
        public static readonly int HitReaction = 7;
        public static readonly int Knockdown = 8;
        public static readonly int GetUp = 9;
        public static readonly int Die = 10;


        public static readonly int Punch = 11;



        public static readonly int Jump = 21;
        public static readonly int Roll = 22;
        public static readonly int Slide = 23;
        public static readonly int Cover = 24;
        public static readonly int Vault = 25;
        public static readonly int Climb = 26;
    }
}