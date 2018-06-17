namespace UtilityAI.Visualization
{
    using System;

    public interface ICustomVisualizer
    {
        void EntityUpdate(object aiEntity, IAIContext context);

        //void EntityUpdate(System.Object aiEntity, IAIContext context, System.Guid aiID);

    }
}

