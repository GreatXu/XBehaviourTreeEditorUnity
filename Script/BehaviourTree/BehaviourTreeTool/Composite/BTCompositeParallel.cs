/* We don't need Parallel node ,becasue all actions are instant , Sequence and Selector can take place.

using LitJson;
using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

namespace BehaviourTreeTool
{
    class BTCompositeParallel : BTCompositeBase
    {
        public enum EParallelType
        {
            OneSuccessed,
            AllSuccessed 
        };

        public EParallelType parallelType;
        public BTCompositeParallel()
            :base()
        {
            parallelType = EParallelType.OneSuccessed;
            nodeName = "Parallel";
        }

        override public void ReadJson( JsonData inJsonData )
        {

            Type type = GetType();
            FieldInfo[] fieldInfos = type.GetFields();

            JsonData property = inJsonData["property"];
            IDictionary dict = property as IDictionary;
            for ( int i = 0 ; i < fieldInfos.Length; ++i )
            { 
                if( ( ( IDictionary )property ).Contains( fieldInfos[i].Name ) )
                {
                    string str = property[fieldInfos[i].Name].ToString();
                    object value = null;

                    if( fieldInfos[i].FieldType == typeof( EParallelType ) )
                    {
                        value =  ( str == "OneSuccessed") ? EParallelType.OneSuccessed : EParallelType.AllSuccessed;
                    }

                    fieldInfos[i].SetValue( this , value );
                }
            }
    
            ReadChildren( inJsonData );
        }

        override public void RenderSelfProperty( int inX , int inY )
        {
            try
            {
                Type type = GetType();
                FieldInfo[] fieldInfos = type.GetFields();
                for( int i = 0 ; i < fieldInfos.Length ; ++i )
                {
                    object value = null;
                    if( fieldInfos[i].FieldType == typeof( EParallelType ) )
                    {
                        GUI.Label( new Rect( inX , inY + i * 20 , 100 , 20 ) , fieldInfos[i].Name );

                        EParallelType tempParallelType = ( EParallelType )fieldInfos[i].GetValue( this );
                        int tempIndex = ( tempParallelType == EParallelType.OneSuccessed) ? 0 : 1;
                        //int tempIndex = ( int )fieldInfos[i].GetValue( this );
                        string[] str = new string[2];
                        str[0] = "OneSuccessed";
                        str[1] = "AllSuccessed";
                        tempIndex = EditorGUI.Popup( new Rect( inX + 100 , inY + i *20 , 100 , 20 ) ,tempIndex , str );
                        value = tempIndex;
                    }

                    //currentTreeIndex = EditorGUI.Popup( new Rect( x , y , 200 , 45 ) , currentTreeIndex , treeNames );

                    fieldInfos[i].SetValue( this , value );
                }
            }
            catch( System.Exception ex )
            {
                Debug.Log( " Render BTNode "+ nodeName + " failed! " );
            }
        }
    }
}

#endif
*/