#if UNITY_EDITOR

using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BehaviourTreeTool
{
    class BehaviourTreeFactory
    {
        public List<Type> listComposite = new List<Type>();
        public List<Type> listCondition = new List<Type>();
        public List<Type> listAction = new List<Type>();

        public Dictionary<string , BTTree> treeMap = new Dictionary<string, BTTree>();

        public static BehaviourTreeFactory _Instance = new BehaviourTreeFactory();

        public BehaviourTreeFactory()
        {
            listComposite = GetSubClass( typeof( BTCompositeBase ) );
            listCondition = GetSubClass( typeof( BTConditionBase ) );
            listAction = GetSubClass( typeof( BTActionBase ) );
        }

        private List<Type> GetSubClass( Type inType )
        {
            List<Type> tempList = new List<Type>();
            
            string pathStr = Application.dataPath + "/";
            List<string> fileStrs = GetAllFiles( pathStr );

            foreach( string str in fileStrs )
            {
                string classStr = str.Split( '.' )[0];
                Type type = Type.GetType( "BehaviourTreeTool."+ classStr );
                if( type != null )
                {
                    if( type.IsSubclassOf( inType ) )
                    {
                         tempList.Add( type );
                    }
                }
            }

            return tempList;
        }

        private List<string> GetAllFiles( string inPath )
        {
            List<string> tempList = new List<string>();

            DirectoryInfo dirInfo = new DirectoryInfo( inPath );

            foreach( FileInfo fileInfo in dirInfo.GetFiles( "*.cs" ) )
            {
                tempList.Add( fileInfo.Name );
            }

            string[] subPaths = Directory.GetDirectories( inPath );
            for( int i = 0 ; i < subPaths.Length ; ++i )
            {
                tempList.AddRange( GetAllFiles( subPaths[i] ) );
            }

            return tempList;
        }

        public BTNode CreateRoot( int inNodeTypeId )
        {
            if( inNodeTypeId < listComposite.Count )
            {
                Type type = listComposite[inNodeTypeId];
                BTNode newNode = Activator.CreateInstance( type ) as BTNode;
                return newNode;
            }
            else
            {
                Debug.LogError( "A error type chosen : "+ inNodeTypeId );
                return null;
            }
        }

        public string[] GetCompostieNodeNamesList()
        {
            string[] tempStrings = new string[listComposite.Count];

            for( int i = 0 ; i < listComposite.Count ; ++i )
            {
                tempStrings[i] = listComposite[i].Name;
            }
            return tempStrings;
        }

        public void LodeTrees( string inJson )
        {
            JsonData jsonData = JsonMapper.ToObject( inJson );
            jsonData = jsonData["trees"];
            for( int i = 0 ; i < jsonData.Count ; ++i )
            {
                BTTree newTree = new BTTree();
                newTree.ReadJson( jsonData[i] );
                treeMap.Add( newTree.treeName , newTree );
            }
        }

        public void EditorSave()
        {
            string filePath = EditorUtility.SaveFilePanel( "Behaviour Tree" , Application.dataPath , "" , "json" );
            JsonData jsonData = new JsonData();
            jsonData["trees"] = new JsonData();
            jsonData["trees"].SetJsonType( JsonType.Array );
            foreach( KeyValuePair<string , BTTree> temp in treeMap )
            {
                temp.Value.WriteJson( jsonData["trees"] );
            }
            File.WriteAllText( filePath , jsonData.ToJson() );
            Debug.Log( "Save behaviour tree json : " +  filePath );
        }

        public void EditorLoad()
        {
            string filePath = EditorUtility.OpenFilePanel( "Behaviour Tree" , Application.dataPath , "json" );
            if( filePath == "" )
            {
                Debug.Log( " no path chosen , cancel this saving ");
                return;
            }
            else
            {
                treeMap.Clear();
                string str = File.ReadAllText( filePath );
                LodeTrees( str );
            }
        }

        public BTTree GetTree( string inName )
        {
            if( treeMap.ContainsKey( inName ) )
            {
                return treeMap[inName];
            }
            return null;
        }

        public List<BTTree> GetAllTrees()
        {
            List<BTTree> tempList = new List<BTTree>();   
            tempList.AddRange( treeMap.Values );
            return tempList;
        }

        public void AddTree( BTTree inTree )
        {
            if( treeMap.ContainsValue( inTree ) )
            {
                EditorUtility.DisplayDialog( "Error" , "The tree named " + inTree.treeName+ " is already existed." , "OK" );
                return;
            }
            treeMap.Add( inTree.treeName , inTree );
        }

        public void RemoveTree( BTTree inTree )
        {
            if( treeMap.ContainsKey( inTree.treeName ) )
            {
                treeMap.Remove( inTree.treeName );
            }
        }
    }
}

#endif
