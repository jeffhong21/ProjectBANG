namespace UtilityAI
{

    public interface IUtilityAIClient
    {

        IUtilityAI ai{
            get;
            set;
        }

        UtilityAIClientState state{
            get;
        }


        IAction activeAction{
            get;
        }


        //
        // Methods
        //
        void Execute();

        void Pause();

        void Resume();

        void Start();

        void Stop();


    }
}