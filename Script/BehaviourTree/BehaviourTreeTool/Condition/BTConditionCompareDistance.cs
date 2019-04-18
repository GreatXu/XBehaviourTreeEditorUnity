#if UNITY_EDITOR

using LitJson;
using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BehaviourTreeTool
{
    class BTConditionCompareDistance: BTConditionBase
    {
        public enum EPurposeType
        {
            NearestBallPlayer,
            MinNearBall,
            MaxNearBall,
            NearestFood,
            NearestFeed,
            NearestWheel,
            MaxSelfBall,
        };
        public enum ECompareType
        {
            AbsoluteValue,
            RelativeMultiplier,
        }

        public EPurposeType purposeType;
        public EPurposeType sourceType;
        public ECompareType compareType;
        public double value;

        public BTConditionCompareDistance():
            base()
        {
            purposeType = EPurposeType.NearestBallPlayer;
            sourceType = EPurposeType.MaxSelfBall;
            compareType = ECompareType.AbsoluteValue;
            value = 1;
            nodeName = "CompareDistance";
            nodeDestricption = "对球的距离进行比较.当compareType为AbsoluteValue时，比较purpose的中心到source边缘的距离（如在source内则为负值）,大于value则返回成功；当compareType为RelativeMultiplier时，比较purpose的中心到source边缘的距离（如在source内则为负值）与source的半径的比值，大于value则返回成功";
        }

        override public void ReadJson( JsonData inJsonData )
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
                    object tempValue = null;

                    if( fieldInfos[i].FieldType == typeof( double ) )
                    {
                        tempValue = double.Parse( str );
                    }
                    else if( fieldInfos[i].FieldType == typeof( EPurposeType ) )
                    {
                        switch( str )
                        {
                            case "NearestBallPlayer":
                                tempValue = EPurposeType.NearestBallPlayer;break;
                            case "MinNearBall":
                                tempValue = EPurposeType.MinNearBall;break;
                            case "MaxNearBall":
                                tempValue = EPurposeType.MaxNearBall;break;
                            case "NearestFood":
                                tempValue = EPurposeType.NearestFood;break;
                            case "NearestFeed":
                                tempValue = EPurposeType.NearestFeed;break;
                            case "NearestWheel":
                                tempValue = EPurposeType.NearestWheel;break;
                            case "MaxSelfBall":
                                tempValue = EPurposeType.MaxSelfBall;break;
                            default:
                                tempValue = EPurposeType.NearestBallPlayer;break;
                        }
                    }
                    else if( fieldInfos[i].FieldType == typeof( ECompareType ) )
                    {
                        tempValue =  ( str == "AbsoluteValue") ? ECompareType.AbsoluteValue : ECompareType.RelativeMultiplier;
                    }
                    fieldInfos[i].SetValue( this , tempValue );
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

                    if( fieldInfos[i].FieldType == typeof( double ) )
                    {
                        GUI.Label( new Rect( inX , inY + i * 20 , 100 , 20 ) , fieldInfos[i].Name );

                        string str = fieldInfos[i].GetValue( this ).ToString();
                        str = GUI.TextField( new Rect( inX + 100 , inY + i *20 , 100 , 20 ) , str );
                        value = double.Parse( str );
                    }
                    else if( fieldInfos[i].FieldType == typeof( EPurposeType ) )
                    {
                        GUI.Label( new Rect( inX , inY + i * 20 , 100 , 20 ) , fieldInfos[i].Name );

                        EPurposeType tempParallelType = ( EPurposeType )fieldInfos[i].GetValue( this );
                        int tempIndex = GetParallelTypeIndex( tempParallelType );
                        string[] str = new string[7];
                        str[0] = "NearestBallPlayer";
                        str[1] = "MinNearBall";
                        str[2] = "MaxNearBall";
                        str[3] = "NearestFood";
                        str[4] = "NearestFeed";
                        str[5] = "NearestWheel";
                        str[6] = "MaxSelfBall";
                        tempIndex = EditorGUI.Popup( new Rect( inX + 100 , inY + i *20 , 100 , 20 ) ,tempIndex , str );
                        value = tempIndex;
                    }
                    else if( fieldInfos[i].FieldType == typeof( ECompareType ) )
                    {
                        GUI.Label( new Rect( inX , inY + i * 20 , 100 , 20 ) , fieldInfos[i].Name );

                        ECompareType tempParallelType = ( ECompareType )fieldInfos[i].GetValue( this );
                        int tempIndex = ( tempParallelType == ECompareType.AbsoluteValue ) ? 0 : 1;
                        string[] str = new string[2];
                        str[0] = "AbsoluteValue";
                        str[1] = "RelativeMultiplier";
                        tempIndex = EditorGUI.Popup( new Rect( inX + 100 , inY + i *20 , 100 , 20 ) ,tempIndex , str );
                        value = tempIndex;
                    }

                    fieldInfos[i].SetValue( this , value );
                }
            }
            catch( System.Exception ex )
            {
                Debug.Log( " Render BTNode "+ nodeName + " failed! " );
            }
        }

        private int GetParallelTypeIndex( EPurposeType inParallelType )
        {
            switch( inParallelType )
            {
                case  EPurposeType.NearestBallPlayer: return 0;
                case  EPurposeType.MinNearBall: return 1;
                case  EPurposeType.MaxNearBall: return 2;
                case  EPurposeType.NearestFood: return 3;
                case  EPurposeType.NearestFeed: return 4;
                case  EPurposeType.NearestWheel: return 5;
                case  EPurposeType.MaxSelfBall: return 6;
            }
            return 0;
        }
    }
}

#endif