#if UNITY_EDITOR

using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BehaviourTreeTool
{
    class BTNode
    {
        protected string typeName;
        protected string nodeName;

        protected bool reverseResult;
        protected string nodeDestricption = "";

        protected BTNode parentNode;
        protected List<BTNode> childrenNodes = new List<BTNode>();

        public BTNode()
        {
            typeName = GetType().FullName;
            nodeName = GetType().Name;
        }

        public string GetNodeName()
        {
            return nodeName;
        }

        public string GetNodeDestricption()
        {
            return nodeDestricption;
        }

        public void SetNodeDestricption( string inDestricption )
        {
            nodeDestricption = inDestricption;
        }

        public bool IsReverseResult()
        {
            return reverseResult;
        }

        public void SetReverseResult( bool inReverseResult )
        {
            reverseResult = inReverseResult;
        }

        virtual public void ReadJson( JsonData inJsonData )
        {
            reverseResult =   bool.Parse( inJsonData["reverse"].ToString() );
            nodeDestricption = inJsonData["destricption"].ToString();


            Type type = GetType();
            FieldInfo[] fieldInfos = type.GetFields();

            JsonData property = inJsonData["property"];
            for ( int i = 0 ; i < fieldInfos.Length; ++i )
            { 
                if( ( ( IDictionary )property ).Contains( fieldInfos[i].Name ) )
                {
                    string str = property[fieldInfos[i].Name].ToString();
                    object value = null;

                    if( fieldInfos[i].FieldType == typeof( int ) )
                    {
                        value = int.Parse( str );
                    }
                    else if ( fieldInfos[i].FieldType == typeof( float ) )
                    {
                        value = float.Parse( str );
                    }
                    else if ( fieldInfos[i].FieldType == typeof( double ) )
                    {
                        value = double.Parse( str );
                    }
                    else if( fieldInfos[i].FieldType == typeof( bool ) )
                    {
                        value = bool.Parse( str );
                    }
                    else if( fieldInfos[i].FieldType == typeof( string ) )
                    {
                        value = str;
                    }

                    fieldInfos[i].SetValue( this , value );
                }
            }

            ReadChildren( inJsonData );
        }

        public void ReadChildren( JsonData inJsonData )
        {
            for( int i = 0 ; i < inJsonData["children"].Count ; ++i )
            {
                string str = inJsonData["children"][i]["type"].ToString();
                Type t = Type.GetType( str );
                BTNode newNode = Activator.CreateInstance( t ) as BTNode;
                newNode.ReadJson( inJsonData["children"][i] );
                newNode.parentNode = this;
                this.AddChild( newNode );
            }
        }

        public JsonData WriteJson()
        {
            JsonData jsonData = new JsonData();
            jsonData["type"] = typeName;
            jsonData["name"] = nodeName;
            jsonData["reverse"] = reverseResult.ToString();
            jsonData["destricption"] = nodeDestricption;

            jsonData["property"] = new JsonData();
            jsonData["property"].SetJsonType( JsonType.Object );
            Type type = GetType();
            FieldInfo[] fieldInfos = type.GetFields();
            for( int i = 0 ; i < fieldInfos.Length ; ++i )
            {
                jsonData["property"][fieldInfos[i].Name] = fieldInfos[i].GetValue( this ).ToString();
            }

            jsonData["children"] = new JsonData();
            jsonData["children"].SetJsonType( JsonType.Array );
            for( int i = 0 ; i < childrenNodes.Count ; ++i )
            {
                jsonData["children"].Add( childrenNodes[i].WriteJson() );
            }
            return jsonData;
        }

        public void AddChild( BTNode inNode )
        {
           childrenNodes.Add( inNode );
        }

        public void RemoveChild( BTNode inNode )
        {
           childrenNodes.Remove( inNode );
        }

        public void InsertChild( BTNode inNode , BTNode inPreNode )
        {
            int index = childrenNodes.FindIndex( a => a == inPreNode ); //lambda

            if( index < 0 )
                index = 0;

            childrenNodes.Insert( index , inNode );
        }

        public void ReplaceChild( BTNode inNode , BTNode inPreNode )
        {
            int index = childrenNodes.FindIndex( a => a == inPreNode ); //lambda
            if( index != -1 )
            {
                childrenNodes[index] = inNode;
            }
            else
            {
                Debug.Log( " Replace BTNode failed! " );
            }
        }

        public bool ContainChild( BTNode inNode )
        {
            return childrenNodes.Contains( inNode );
        }

        virtual public void RenderSelfProperty( int inX , int inY )
        {
            try
            {
                Type type = GetType();
                FieldInfo[] fieldInfos = type.GetFields();
                for( int i = 0 ; i < fieldInfos.Length ; ++i )
                {
                    object value = null;

                    if( fieldInfos[i].FieldType == typeof( int ) )
                    {
                        GUI.Label( new Rect( inX , inY + i * 20 , 100 , 20 ) , fieldInfos[i].Name );

                        string str = fieldInfos[i].GetValue( this ).ToString();
                        str = GUI.TextField( new Rect( inX + 100 , inY + i *20 , 100 , 20 ) , str );
                        value = int.Parse( str );
                    }
                    else if( fieldInfos[i].FieldType == typeof( float ) )
                    {
                        GUI.Label( new Rect( inX , inY + i * 20 , 100 , 20 ) , fieldInfos[i].Name );

                        string str = fieldInfos[i].GetValue( this ).ToString();
                        str = GUI.TextField( new Rect( inX + 100 , inY + i *20 , 100 , 20 ) , str );
                        value = float.Parse( str );
                    }
                    else if( fieldInfos[i].FieldType == typeof( double ) )
                    {
                        GUI.Label( new Rect( inX , inY + i * 20 , 100 , 20 ) , fieldInfos[i].Name );

                        string str = fieldInfos[i].GetValue( this ).ToString();
                        str = GUI.TextField( new Rect( inX + 100 , inY + i *20 , 100 , 20 ) , str );
                        value = double.Parse( str );
                    }
                    else if( fieldInfos[i].FieldType == typeof( bool ) )
                    {
                        GUI.Label( new Rect( inX , inY + i * 20 , 100 , 20 ) , fieldInfos[i].Name );
   
                        bool tempBool = ( bool )fieldInfos[i].GetValue( this );
                        tempBool = GUI.Toggle( new Rect( inX + 100 , inY + i *20 , 100 , 20 ) , tempBool ,""  );
                        value = tempBool;
                    }
                    else if( fieldInfos[i].FieldType == typeof( string ) )
                    {
                        GUI.Label( new Rect( inX , inY + i * 20 , 100 , 20 ) , fieldInfos[i].Name );

                        string str = fieldInfos[i].GetValue( this ).ToString();
                        str = GUI.TextField( new Rect( inX + 100 , inY + i *20 , 100 , 20 ) , str );
                        value = str;
                    }

                    fieldInfos[i].SetValue( this , value );
                }
            }
            catch( System.Exception ex )
            {
                Debug.Log( " Render BTNode "+ nodeName + " failed! " );
            }
        }

        public void RenderNode( int inX , ref int inY )
        {
            if( BehaviourTreeWindow._Instance.currentNode == this )
            {
                Texture2D texture2D = new Texture2D( 1 ,1 );
                //texture2D.SetPixel( 0 , 0 , Color.green);
                texture2D.SetPixel(0, 0, Color.gray);
                texture2D.Apply();
                GUI.DrawTexture( new Rect( 0 , inY , 2 * BehaviourTreeWindow._Instance.position.width , BehaviourTreeWindow.NODE_HEIGH ) , texture2D );
            }

            Event evt = Event.current;
            Rect moveRect = new Rect( inX , inY , 2 * BehaviourTreeWindow._Instance.position.width - BehaviourTreeWindow.GUI_WIDTH , 6 );
            bool isMovingNode = false;

            //drag a node above this
            if( moveRect.Contains( evt.mousePosition ) && BehaviourTreeWindow._Instance.selectedNode != null
                && parentNode != null && BehaviourTreeWindow._Instance.selectedNode != this && parentNode.CanBeMyChild( BehaviourTreeWindow._Instance.selectedNode ) )
            {
                isMovingNode = true;
                Texture2D texture2D = new Texture2D( 1 ,1 );;
                texture2D.SetPixel( 0 , 0 , Color.blue );
                texture2D.Apply(); 
                GUI.DrawTexture( new Rect( inX - 10 , inY , 2 * BehaviourTreeWindow._Instance.position.width + 10 , 6 ) , texture2D );
         
                if( evt.button == 0 && evt.type == EventType.MouseUp )
                {
                    BehaviourTreeWindow._Instance.selectedNode.parentNode.RemoveChild( BehaviourTreeWindow._Instance.selectedNode );
                    BehaviourTreeWindow._Instance.selectedNode.parentNode = parentNode;
                    parentNode.InsertChild(  BehaviourTreeWindow._Instance.selectedNode , this );
                    BehaviourTreeWindow._Instance.selectedNode = null;
                    BehaviourTreeWindow._Instance.Repaint();
                }
            }

            Rect rect = new Rect( inX , inY , 2 * BehaviourTreeWindow._Instance.position.width - BehaviourTreeWindow.GUI_WIDTH , BehaviourTreeWindow.NODE_HEIGH );
            if( !isMovingNode && rect.Contains( evt.mousePosition ) )
            {
                if( BehaviourTreeWindow._Instance.selectedNode != null && BehaviourTreeWindow._Instance.currentNode != this && CanBeMyChild( BehaviourTreeWindow._Instance.currentNode ) )
                {
                    Texture2D text = new Texture2D( 1 ,1 );
                    text.SetPixel( 0 , 0 ,Color.yellow );
                    text.Apply();
                    GUI.DrawTexture( new Rect( 0 , inY , 2 * BehaviourTreeWindow._Instance.position.width , BehaviourTreeWindow.NODE_HEIGH ) , text );
                }
                if( evt.type == EventType.ContextClick )
                {
                    GenericMenu menu = new GenericMenu();
                    if( CanHasChild() )
                    {
                        foreach( Type item in BehaviourTreeFactory._Instance.listComposite )
                        {
                            menu.AddItem( new GUIContent( "Create/Composite/" + item.Name ) , false , CallBackMenuAdd , item );
                        }
                        foreach( Type item in BehaviourTreeFactory._Instance.listCondition )
                        {
                            menu.AddItem( new GUIContent( "Create/Condition/" + item.Name ) , false , CallBackMenuAdd , item );
                        }
                        foreach( Type item in BehaviourTreeFactory._Instance.listAction )
                        {
                            menu.AddItem( new GUIContent( "Create/Action/" + item.Name ) , false , CallBackMenuAdd , item );
                        }
                    }
                    foreach( Type item in BehaviourTreeFactory._Instance.listComposite )
                    {
                        menu.AddItem( new GUIContent( "Switch/Composite/" + item.Name ) , false , CallBackMenuSwitch , item );
                    }
                    menu.AddItem( new GUIContent( "Delete" ) , false , CallBackMenuDelete , "" );
                    menu.ShowAsContext();
                }
                else if( evt.button == 0 && evt.type == EventType.MouseDown/* && this != BehaviourTreeWindow._Instance.currentTree.treeRoot */)
                {
                    BehaviourTreeWindow._Instance.selectedNode = this;
                    BehaviourTreeWindow._Instance.currentNode = this;
                }
                //drag a node over this
                if(  evt.button == 0 && evt.type == EventType.MouseUp && BehaviourTreeWindow._Instance.selectedNode != null)
                {
                    if( this != BehaviourTreeWindow._Instance.selectedNode && !moveRect.Contains( evt.mousePosition ) && CanBeMyChild( BehaviourTreeWindow._Instance.currentNode ) )
                    {
                        BehaviourTreeWindow._Instance.selectedNode.parentNode.RemoveChild( BehaviourTreeWindow._Instance.selectedNode );
                        BehaviourTreeWindow._Instance.selectedNode.parentNode = this;
                        AddChild( BehaviourTreeWindow._Instance.selectedNode );
                    }
                    BehaviourTreeWindow._Instance.selectedNode = null;
                    BehaviourTreeWindow._Instance.Repaint();
                }
            }

            GUIStyle tempGUIStyle = new GUIStyle();
            tempGUIStyle.normal.textColor = Color.black;
            GUI.Label( new Rect( inX , inY , 2 * BehaviourTreeWindow._Instance.position.width , BehaviourTreeWindow.NODE_HEIGH ) , nodeName + "  :" + nodeDestricption , tempGUIStyle );

            //draw line
            Vector3 pos1 = new Vector3( inX + BehaviourTreeWindow.NODE_WIDTH  / 2 , inY + BehaviourTreeWindow.NODE_HEIGH , 0 );
            Handles.color = Color.red;
            for( int i = 0 ; i < childrenNodes.Count ; ++i )
            {
                inY += BehaviourTreeWindow.NODE_HEIGH;
                Vector3 pos2 = new Vector3( inX + BehaviourTreeWindow.NODE_WIDTH  / 2 , inY + BehaviourTreeWindow.NODE_HEIGH / 2 , 0 );
                Vector3 pos3 = new Vector3( inX + BehaviourTreeWindow.NODE_WIDTH , inY + BehaviourTreeWindow.NODE_HEIGH / 2 , 0 );
                childrenNodes[i].RenderNode( inX + BehaviourTreeWindow.NODE_WIDTH , ref inY );
                Handles.DrawPolyLine( new Vector3[] { pos1 , pos2 , pos3 } );
            }
        }

        private void CallBackMenuAdd( object inObject )
        {
            Type type = inObject as Type;
            BTNode newBTNode = Activator.CreateInstance( type ) as BTNode;

            AddChild( newBTNode );
            newBTNode.parentNode = this;
            BehaviourTreeWindow._Instance.Repaint();
        }

        private void CallBackMenuSwitch( object inObject )
        {
            Type type = inObject as Type;
            BTNode newBTNode = Activator.CreateInstance( type ) as BTNode;

            newBTNode.parentNode = parentNode;
            foreach( BTNode tempNode in childrenNodes )
            {
                newBTNode.AddChild( tempNode );
            }
            if( parentNode != null )
            {
                parentNode.ReplaceChild( newBTNode , this );
            }
            else if( BehaviourTreeWindow._Instance.currentTree.treeRoot == this )
            {
                BehaviourTreeWindow._Instance.currentTree.treeRoot = newBTNode;
            }

            BehaviourTreeWindow._Instance.currentNode = newBTNode;
            BehaviourTreeWindow._Instance.Repaint();
        }

        private void CallBackMenuDelete( object inObject )
        {
            if( parentNode != null )
            {
                parentNode.RemoveChild( this );
            }
            parentNode = null;
            BehaviourTreeWindow._Instance.currentNode = null;
            BehaviourTreeWindow._Instance.selectedNode = null;
            BehaviourTreeWindow._Instance.Repaint();
        }

        public BTNode GetParentNode()
        {
            return parentNode;
        }

        public virtual bool CanBeMyChild( BTNode inNode )
        {
            return false;
        }

        public virtual bool CanHasChild()
        {
            return false;
        }
    }
}
#endif