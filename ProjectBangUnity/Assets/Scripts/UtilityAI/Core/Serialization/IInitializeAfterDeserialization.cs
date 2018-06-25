namespace UtilityAI.Serialization
{

    public interface IInitializeAfterDeserialization
    {
        
        void InitializeAfterDeserialization(object rootObject);

    }
}