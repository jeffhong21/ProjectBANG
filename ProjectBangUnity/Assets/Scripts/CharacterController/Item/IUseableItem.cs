namespace CharacterController
{

    public interface IUseableItem
    {

        bool TryUse();


        bool CanUse();


        bool InUse();


        void TryStopUse();


    }

}