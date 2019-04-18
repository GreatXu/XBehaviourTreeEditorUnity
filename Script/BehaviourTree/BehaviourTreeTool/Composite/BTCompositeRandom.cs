using System;
#if UNITY_EDITOR

namespace BehaviourTreeTool
{
    class BTCompositeRandom : BTCompositeBase
    {
        public BTCompositeRandom()
            :base()
        {
            nodeName = "Random";
            nodeDestricption = "随机选择一个子节点，返回其运行结果";
        }
    }
}

#endif
