using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class FighterParameterTable : MonoBehaviour
{
    TextAsset FighterParameter;
    string strPath = "Data/TableAssets/FighterParameterTable";

    private void Awake()
    {
        TextAsset csvText = Resources.Load<TextAsset>(strPath);
        if (!csvText)
        {
            Debug.LogError("读取" + strPath + "失败");
            //return null;
        }
        List<XElement> lst = new List<XElement>();
        string[] lineArray = csvText.text.Split("\r"[0]);
        string[][] Array = new string[lineArray.Length][];
        for (int i = 2; i < lineArray.Length; i++)
        {
            Array[i] = lineArray[i].Split(',');
            for (int j = 0; j < Array[i].Length; j++)
            {
                //Debug.Log(Array[i][j].ToString());//然后把数据存到list里
            }
        }
    }

    //public static List<XElement> InitTable(string strPath, string strData = "")
    //{
    //    TextAsset csvText = Resources.Load<TextAsset>(strPath);
    //    if (!csvText)
    //    {
    //        Debug.LogError("读取" + strPath + "失败");
    //        return null;
    //    }
    //    List<XElement> lst = new List<XElement>();
    //    string[] lineArray = csvText.text.Split("\r"[0]);
    //    string[][] Array = new string[lineArray.Length][];
    //    for (int i = 0; i < lineArray.Length; i++)
    //    {
    //        Array[i] = lineArray[i].Split(',');
    //    }

    //}

}
