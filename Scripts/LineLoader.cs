using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class LineLoader : FuncManager
{
    LineRenderer lineRenderer;

    public LineRegister lineRegister;
    public DeleteFunc deleteFunc;

    List<Vector2> pos = new List<Vector2>();
    float width;
    Color color;

    AudioSource myAudio;

    //string tstr = "0.815915,5.560159|0.815915,5.560159|0.7981752,5.544475;0.05;RGBA(0.000, 0.000, 0.000, 0.000)0.713953,5.547259|0.815915,5.560159|0.7981752,5.544475;0.05;RGBA(0.000, 0.000, 0.000, 0.000)";


    private void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    public void ParsingData(string str)
    {
        if(str == "")
        {
            return;
        }
        deleteFunc.DeleteChilds();
        string[] linecount = str.Split(')');

        for(int i = 0; i < linecount.Length - 1; i++)
        {
            string[] paramstr = linecount[i].Split(';');
            string[] valarr = paramstr[0].Split('|');


            pos.Clear();
            Vector2 temppos = new Vector2();

            foreach(var item in valarr)
            {
                string[] parsingArr = item.Split(',');
                temppos.x = float.Parse(parsingArr[0]);
                temppos.y = float.Parse(parsingArr[1]);
                pos.Add(temppos);
                //Debug.LogFormat("{0}, {1}", temppos.x, temppos.y);
            }

            width = float.Parse(paramstr[1]);
            //Debug.LogFormat("width : {0}", width);

            // 글자제거
            paramstr[2] = paramstr[2].Substring(5, paramstr[2].Length - 5);
            string[] colorstr = paramstr[2].Split(',');

            color.r = float.Parse(colorstr[0]);
            color.g = float.Parse(colorstr[1]);
            color.b = float.Parse(colorstr[2]);
            color.a = float.Parse(colorstr[3]);

           // Debug.LogFormat("{0}, {1}, {2}, {3}", color.r, color.g, color.b, color.a);


            GameObject a = new GameObject();
            lineRenderer = a.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("UI/Default"));

            a.transform.SetParent(player.hitPage);


            // 라인의 배열개수 지정
            lineRenderer.positionCount = pos.Count;

            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            // 색깔지정
            lineRenderer.material.color = color;

            // 라운딩 지정
            lineRenderer.numCapVertices = 5;

            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;

            for(int j = 0; j < pos.Count; j++)
            {
                lineRenderer.SetPosition(j, new Vector3(pos[j].x, pos[j].y, player.LineZpos));
            }

            lineRenderer = null;
        }

        DataList tempList = new DataList();
        tempList.list = new List<LineData>();

        for(int i = 0; i < player.lineDatas.list.Count; i++)
        {
            tempList.list.Add(player.lineDatas.list[i]);
        }

        if(player.lineUndo.mementos.Count < player.lineUndo.ListLimited)
        {
            player.lineUndo.mementos.Add(tempList);
        }

        else
        {
            player.lineUndo.mementos.RemoveAt(0);
            player.lineUndo.mementos.Add(tempList);
        }


        //player.lineUndo.undoCount = player.lineUndo.mementos.Count - 1;
    }

    private void Update()
    {
        if(CheckClick())
        {
            myAudio.Play();
            ParsingData(lineRegister.tempstr);
        }
    }
}
