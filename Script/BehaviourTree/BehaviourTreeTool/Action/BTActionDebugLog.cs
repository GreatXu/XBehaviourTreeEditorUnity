#if UNITY_EDITOR

namespace BehaviourTreeTool
{
    class BTActionDebugLog : BTActionBase
    {
        public string logStr;

        public BTActionDebugLog():
            base()
        {
            nodeName = "DebugLog";
            nodeDestricption = "在Unity中输入log信息";
            logStr = "";
        }
    }
}

#endif