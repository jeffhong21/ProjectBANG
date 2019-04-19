namespace uUtilityAI.Scheduler
{
    /// <summary>
    /// Interface for the client.
    /// </summary>
    public interface IClientScheduler
    {
        //
        // Methods
        //
        float? ExecuteUpdate(float deltaTime, float nextInterval);
    }
}

