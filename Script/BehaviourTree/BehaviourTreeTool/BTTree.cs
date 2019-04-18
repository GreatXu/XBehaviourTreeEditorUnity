#if UNITY_EDITOR

using LitJson;
using System;
using System.Collections;

namespace BehaviourTreeTool
{
    class BTTree
    {
        public string treeName;
        public BTNode treeRoot;
        public double tickRate = 0.5;

        public BTTree()
        {
        }

        public void ReadJson( JsonData inJsonData )
        {
            treeName = inJsonData["name"].ToString();
            tickRate = double.Parse( inJsonData["tickrate"].ToString() );
            treeRoot = null;

            if( ( ( IDictionary )inJsonData ).Contains( "node" ) )
            {
                string str = inJsonData["node"]["type"].ToString();
                Type type = Type.GetType( str );
                treeRoot = Activator.CreateInstance( type ) as BTNode;
                treeRoot.ReadJson( inJsonData["node"] );
            }
        }

        public void WriteJson( JsonData inJsonData )
        {
            JsonData jsonData = new JsonData();
            jsonData["name"] = treeName;
            jsonData["tickrate"] = tickRate.ToString();

            if( treeRoot != null )
            {
                jsonData["node"] = new JsonData();
                jsonData["node"].SetJsonType( JsonType.Object );
                jsonData["node"] = treeRoot.WriteJson();
            }
            inJsonData.Add( jsonData );
        }
        
        public void Clear()
        {
            treeRoot = null;
        }
    }
}

#endif

