using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Valve.VR.InteractionSystem;

public class LineUndo : FuncManager
{
    public enum MODE
    {
        NEXT = 1,
        PREV,
    }

    public MODE mode;

    public List<DataList> mementos = new List<DataList>();
    public int undoCount = 0;


    // 리스트 개수 제한
    public int ListLimited;
    

    void Memento()
    {
        switch(mode)
        {
            case MODE.NEXT:
            SetLine(mementos[undoCount]);
            break;

            case MODE.PREV:
            if(undoCount == 0)
            {
                SetLine(null);
            }
            else
            {
                SetLine(mementos[undoCount - 1]);
            }

            break;
        }
    }

    void SetLine(DataList data)
    {
        // data는 Memento 즉 객체의 상태를 저장한 리스트
        Debug.LogFormat("호출 {0}", undoCount);
        for(int i=0; i<player.hitPage.childCount; i++)
        {
            Destroy(player.hitPage.GetChild(i).gameObject);
            player.lineDatas.list.Clear();
        }

        if(data != null)
        {
            for(int i = 0; i < data.list.Count; i++)
            {
                GameObject a = new GameObject();
                LineRenderer lineRenderer = a.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("UI/Default"));

                a.transform.SetParent(player.hitPage);


                // 라인의 배열개수 지정
                lineRenderer.positionCount = data.list[i].mousePos.Count;

                lineRenderer.startWidth = data.list[i].width;
                lineRenderer.endWidth = data.list[i].width;

                // 색깔지정
                lineRenderer.material.color = data.list[i].color;

                // 라운딩 지정
                lineRenderer.numCapVertices = 5;

                lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lineRenderer.receiveShadows = false;

                for(int j = 0; j < data.list[i].mousePos.Count; j++)
                {
                    lineRenderer.SetPosition(j, new Vector3(data.list[i].mousePos[j].x, data.list[i].mousePos[j].y, player.LineZpos));
                }


                LineData temp = new LineData();
                temp.width = data.list[i].width;
                temp.color = data.list[i].color;
                temp.mousePos = data.list[i].mousePos;

                player.lineDatas.list.Add(temp);

                a = null;
                lineRenderer = null;
            }
        }


        switch(mode)
        {
            case MODE.NEXT:
            undoCount += 1;
            break;

            case MODE.PREV:
            undoCount -= 1;
            break;
        }

        if(undoCount < 0)
        {
            undoCount = 0;
        }

        else if(undoCount == mementos.Count)
        {
            undoCount = mementos.Count - 1;
        }
    }


    private void Update()
    {
        if(CheckClick())
        {
            mode = MODE.PREV;
            Memento();
        }

        else if(CheckRightClick())
        {
            mode = MODE.NEXT;
            Memento();
        }
    }
}
