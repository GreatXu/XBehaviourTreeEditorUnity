#if UNITY_EDITOR

namespace BehaviourTreeTool
{
    class BTConditionCooldown : BTConditionBase
    {
        public double cooldown;

        public BTConditionCooldown():
            base()
        {
            nodeName = "Cooldown";
            nodeDestricption = "每隔一定周期返回一次成功";
            cooldown = 5.0;
        }
    }
}

#endif