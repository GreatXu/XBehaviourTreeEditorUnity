using System;
#if UNITY_EDITOR

namespace BehaviourTreeTool
{
    class BTCompositeSelector : BTCompositeBase
    {
        public BTCompositeSelector()
            :base()
        {
            nodeName = "Selector";
            nodeDestricption = "顺序执行子节点，直到其中一个执行成功时返回成功或全部失败时返回失败";
        }
    }
}

#endif