using System;

namespace uUtilityAI.AIEditor
{

    public interface INode
    {
        //string description{
        //    get;
        //    set;
        //}

        string name{
            get;
            set;
        }

        AIUI parentUI{
            get;
        }
    }
}
