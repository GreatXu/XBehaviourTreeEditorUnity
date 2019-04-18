#if UNITY_EDITOR

namespace BehaviourTreeTool
{
    class BTCompositeSequence : BTCompositeBase
    {
        public BTCompositeSequence()
            :base()
        {
            nodeName = "Sequence";
            nodeDestricption = "顺序执行子节点，直到其中一个执行失败时返回失败或全部成功时返回成功";
        }
    }
}

#endif