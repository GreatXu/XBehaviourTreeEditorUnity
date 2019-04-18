#if UNITY_EDITOR

namespace BehaviourTreeTool
{
    class BTCompositeBase : BTNode
    {
        public BTCompositeBase()
            :base()
        {
            nodeName = "Composite";
        }
        
        public override bool CanBeMyChild( BTNode inNode )
        {
            if( inNode != this )
            {
                BTNode tempNode = parentNode;
                while( tempNode != null )
                {
                    if( tempNode == inNode )
                    {
                        return false;
                    }
                    else
                    {
                        tempNode = tempNode.GetParentNode();
                    }
                }
                return true;
            } 
            return false;
        }
        
        public override bool CanHasChild()
        {
            return true;
        }
    }
}

#endif