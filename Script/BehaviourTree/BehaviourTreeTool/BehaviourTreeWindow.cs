#if UNITY_EDITOR

using UnityEditor;
using BehaviourTreeTool;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

class BehaviourTreeWindow : EditorWindow
{
    public static int NODE_WIDTH = 20;
    public static int NODE_HEIGH = 20;
    public static int GUI_WIDTH = 240;

    public static BehaviourTreeWindow _Instance = null;

    public BTTree currentTree;
    public BTNode currentNode;
    public BTNode selectedNode;

    private int currentTreeIndex = -1;
    private int lastTreeIndex = -1;
    private int creatNodeId = -1;

    private Vector2 scrollPosition = new Vector2( 0 , 0 );
    private string inputName = "";
    private string tickRateStr = "";

    [MenuItem( "BehavoirTree/Editor" )]
    static void initwin()
    {
        if( _Instance == null )
        {
            _Instance = ( BehaviourTreeWindow )GetWindow( typeof( BehaviourTreeWindow ) );
        }
    }

    void OnGUI()
    {
        //draw behaviour tree
        scrollPosition = GUI.BeginScrollView( new Rect( 0 , 0 , position.width - 240 , position.height ) , scrollPosition , new Rect( 0 , 0 , maxSize.x , maxSize.y ) );
        Texture2D text1 = new Texture2D( 1 , 1 );
        text1.SetPixel( 0 , 0 , Color.white );
        text1.Apply();
        //Texture2D text2 = new Texture2D( 1 , 1 );
        //text2.SetPixel( 0 , 0 , Color.gray );
        //text2.Apply();

        for( int i = 0 ; i < 1000 ; ++i )
        {
            GUI.DrawTexture( new Rect( 0 , i * NODE_HEIGH , 2 * _Instance.position.width , NODE_HEIGH ) , text1 );
            //++i;
            //GUI.DrawTexture( new Rect( 0 , i * NODE_HEIGH , 2 * _Instance.position.width , NODE_HEIGH ) , text2 );
        }

        if( currentTree != null && currentTree.treeRoot != null )
        {
            int xx = 0 ,yy = 0;
            currentTree.treeRoot.RenderNode( xx , ref yy );
        }
        GUI.EndScrollView();

        //draw editor
        GUI.BeginGroup( new Rect( position.width - GUI_WIDTH , 0 , 300 ,1000 ) );
        int x = 20 , y = 20;
        List<BTTree> treeList = BehaviourTreeFactory._Instance.GetAllTrees();
        if( GUI.Button( new Rect( x , y , 200 , 40 ) , "Load Json") )
        {
            selectedNode = null;
            currentNode = null;
            currentTree = null;
            currentTreeIndex = -1;
            lastTreeIndex = -1;
            creatNodeId = -1;

            BehaviourTreeFactory._Instance.EditorLoad();
        }
        y += 60;
        if( GUI.Button( new Rect( x , y , 200 , 40 ) , "Save Json") )
        {
            BehaviourTreeFactory._Instance.EditorSave();
            AssetDatabase.Refresh();
        }
        y += 40;
        GUI.Label( new Rect( x , y , 200 , 20 ) , "==========================" );
        y += 20;
        inputName = GUI.TextField( new Rect( x , y + 10 , 200 , 20 ) , inputName );
        y += 40;
        if( GUI.Button( new Rect( x , y , 200 , 40 ) , "creat tree" ) )
        {
            if( inputName != "" )
            {
                currentNode = null;
                BTTree newTree = new BTTree();
                newTree.treeName = inputName;
                BehaviourTreeFactory._Instance.AddTree( newTree );
                currentTree = newTree;
                tickRateStr = "";
                treeList = BehaviourTreeFactory._Instance.GetAllTrees();
                for( int i = 0 ; i < treeList.Count ; ++i )
                {
                    if( treeList[i].treeName == newTree.treeName )
                    {
                        currentTreeIndex = i;
                        break;
                    }
                }
                lastTreeIndex = currentTreeIndex;
                Repaint();
            }
        }

        if( treeList.Count != 0 )
        {
             y += 40;
            GUI.Label( new Rect( x , y ,200 ,20 ) , "==========================");

            y += 20;
            string[] treeNames = new string[treeList.Count];
            for( int i = 0 ; i < treeNames.Length ; ++i )
            {
                treeNames[i] = treeList[i].treeName;
            }
            currentTreeIndex = EditorGUI.Popup( new Rect( x , y , 200 , 45 ) , currentTreeIndex , treeNames );
            if( currentTreeIndex != lastTreeIndex )
            {
                lastTreeIndex = currentTreeIndex;
                currentTree = treeList[currentTreeIndex];
                tickRateStr = "";
                currentNode = null;
            }
            y += 40;
            if( GUI.Button( new Rect( x , y , 200 , 40 ) , "remove tree" ) )
            {
                currentNode = null;
                BehaviourTreeFactory._Instance.RemoveTree( currentTree );
                currentTree = null;
                currentTreeIndex = -1; 
                lastTreeIndex = -1;
                treeList = BehaviourTreeFactory._Instance.GetAllTrees();
                Repaint();
            }
        }
        if( currentTree != null )
        {
            y += 40;
            GUI.Label( new Rect( x , y , 200 ,20 ) , "==========================" );
            y += 20;
            GUI.Label( new Rect( x , y , 100 , 20 ) , "Tree name : ");
            currentTree.treeName = GUI.TextField( new Rect( x + 100 , y , 100 ,20 ) , currentTree.treeName );
            y += 20;

            GUI.Label( new Rect( x , y , 100 , 20 ) , "Tree Tick Rate : ");
            if( tickRateStr == "" )
            {
                tickRateStr = currentTree.tickRate.ToString();
            }
            tickRateStr = GUI.TextField( new Rect( x + 100 , y , 100 ,20 ) , tickRateStr );
            currentTree.tickRate = double.Parse( tickRateStr );

            if( currentTree.treeRoot == null )
            {
                creatNodeId = EditorGUI.Popup( new Rect( x ,y + 20 , 200 , 40 ) , creatNodeId , BehaviourTreeFactory._Instance.GetCompostieNodeNamesList() );
                y += 40;
                if( creatNodeId != -1 )
                {
                    if( GUI.Button( new Rect( x , y , 200 ,40 ) , "Create Root" ) )
                    {
                        BTNode newNode = BehaviourTreeFactory._Instance.CreateRoot( creatNodeId );
                        currentTree.treeRoot = newNode;
                        currentNode = newNode;
                        y += 40;
                    }
                }
            }
        }
        if( currentNode != null )
        {
            y += 40;
            GUI.Label( new Rect( x , y , 200 ,20 ) , "==========================" );
            y += 20;
            GUI.Label( new Rect( x , y , 300 ,20 ) , "Node Type : " + currentNode.GetType().FullName );
            y += 20;
            GUI.Label( new Rect( x , y , 200 ,20 ) , "Node Name : " + currentNode.GetType().Name );
            y += 20;
            
            GUI.Label( new Rect( x , y , 100 , 20 ) , "Node Description : " );
            y += 20;
            string tempNodeDestricption = currentNode.GetNodeDestricption();
            tempNodeDestricption = GUI.TextField( new Rect( x , y , 200 ,40 ) , tempNodeDestricption );
            currentNode.SetNodeDestricption( tempNodeDestricption );
            y += 40;

            GUI.Label( new Rect( x , y , 100 , 20 ) , "Reverse Result : " );
            bool tempReverseResult = currentNode.IsReverseResult();
            tempReverseResult = GUI.Toggle( new Rect( x + 100 , y , 100 , 20 ) , tempReverseResult ,""  );
            currentNode.SetReverseResult( tempReverseResult );

            y += 15;
            currentNode.RenderSelfProperty( x ,y );
        }
        GUI.EndGroup();
    }

    void Update()
    {
        _Instance = this;
        if( selectedNode != null )
        {
            Repaint();
        }
    }
}

#endif
