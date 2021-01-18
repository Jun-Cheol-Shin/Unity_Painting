using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LineRegister : FuncManager
{
    [HideInInspector]
    public string tempstr;

    [HideInInspector]
    public DataList SaveData;

    AudioSource myAudio;


    private void Start()
    {
        SaveData = new DataList();
        SaveData.list = new List<LineData>();
        myAudio = GetComponent<AudioSource>();
    }

    public string GetStringData(DataList data)
    {
        tempstr = "";
        // (0.0, 0.0, 0.0)(0.0, 1.0, 1.0)
        // (0.0, 0.0, 2.0)(0.0, 1.0, 3.0)

        for(int i = 0; i < data.list.Count; i++)
        {
            foreach(var item in data.list[i].mousePos)
            {
                tempstr += string.Format("{0},{1}|", item.x, item.y);
            }

            tempstr = tempstr.Substring(0, tempstr.Length - 1);

            tempstr += ";" + data.list[i].width.ToString();
            tempstr += ";" + data.list[i].color.ToString();
        }
        // "0.0,0.0|0.0,2.6;0.0;(1, 1, 1)"

        Debug.LogFormat("확인값 : {0}", tempstr);

        return tempstr;
    }


    private void Update()
    {
        if(CheckClick())
        {
            myAudio.Play();
            SaveData.list.Clear();
            for(int i=0; i<player.lineDatas.list.Count; i++)
            {
                SaveData.list.Add(player.lineDatas.list[i]);
            }

            GetStringData(SaveData);
        }
    }

}
