using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
public class LineData
{
    public List<Vector2> mousePos = new List<Vector2>();
    public float width;
    public Color color;
}


public class DataList
{
    public List<LineData> list;
}

public class PlayerDrawLine : MonoBehaviour
{

    GameObject line;
    LineRenderer lineRenderer;

    [HideInInspector]
    public List<Vector2> mousePositions;

    [HideInInspector]
    public DataList lineDatas;

    bool d_enable = false;

    public LayerMask layerMask;
    public LayerMask pageMask;
    public float drawDistance;

    [HideInInspector]
    public Transform hitPage;

    float boardZbuffer = -0.08f;

    [HideInInspector]
    public float LineZpos;

    [Header("[그리기 선의 옵션]")]
    public float l_width;
    public Color l_color;


    public LineUndo lineUndo;

    private void Start()
    {
        mousePositions = new List<Vector2>();
        lineDatas = new DataList();
        lineDatas.list = new List<LineData>();
    }

    void CompositeMethod()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(CheckClick())
            {
                CreateNewLine();
            }
        }

        else if(Input.GetMouseButton(0))
        {
            if(CheckClick())
            {
                if(line != null)
                {
                    DrawingLine();
                }

                else if(line == null)
                {
                    CreateNewLine();
                }
            }

            else
            {
                EndDrawing();
            }
        }

        else if(Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(CheckClick())
            {
                EndDrawing();
            }
        }
    }

    void FixedUpdate()
    {
        CheckBoard();
    }

    void Update()
    {
        if(d_enable)
        {
            CompositeMethod();
        }
    }

    // 선을 새로 만드는 함수
    void CreateNewLine()
    {
        if(line == null)
        {

            line = new GameObject();
            lineRenderer = line.AddComponent<LineRenderer>();


            lineRenderer.material = new Material(Shader.Find("UI/Default"));

            line.transform.SetParent(hitPage);
            
            // 라인의 배열개수 지정
            lineRenderer.positionCount = 2;
            
            // 두께 지정
            lineRenderer.startWidth = l_width;
            lineRenderer.endWidth = l_width;

            // 색깔지정
            lineRenderer.material.color = l_color;
            // 라운딩 지정
            lineRenderer.numCapVertices = 5;

            // 라인의 그림자 끄기
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            LineZpos = hitPage.root.position.z + boardZbuffer;


            mousePositions = new List<Vector2>();
            // 마우스 배열 클리어
            mousePositions.Clear();

            // 마우스의 클릭지점을 받아서
            Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            Vector3 addVec = new Vector3(point.x, point.y, LineZpos);

            // 라인렌더러와 리스트에 저장
            mousePositions.Add(addVec);
            mousePositions.Add(addVec);
            lineRenderer.SetPosition(0, addVec);
            lineRenderer.SetPosition(1, addVec);
        }
    }

    // 선을 그리는 함수
    void DrawingLine()
    {
        if(line != null)
        {
            Vector2 temp = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            //Debug.Log(temp);

            // 0.1단위로 움직이면 렌더러 리스트카운터 추가
            if(Vector2.Distance(temp, mousePositions[mousePositions.Count - 1]) > Mathf.Abs(0.1f))
            {
                Vector3 addVec = new Vector3(temp.x, temp.y, LineZpos);
                mousePositions.Add(addVec);
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, addVec);
            }
        }
    }

    // 끝내는 함수
    void EndDrawing()
    {
        if(line != null)
        {
            Vector3 temp = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z + boardZbuffer));

            Vector3 addVec = new Vector3(temp.x, temp.y, LineZpos);

            mousePositions.Add(addVec);
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, addVec);


            LineData data = new LineData();

            data.mousePos = mousePositions;
            data.width = l_width;
            data.color = l_color;

            lineDatas.list.Add(data);

            DataList tempList = new DataList();
            tempList.list = new List<LineData>();

            for(int i=0; i < lineDatas.list.Count; i++)
            {
                tempList.list.Add(lineDatas.list[i]);
            }

            if(lineUndo.mementos.Count < lineUndo.ListLimited)
            {
                lineUndo.mementos.Add(tempList);
            }

            else
            {
                lineUndo.mementos.RemoveAt(0);
                lineUndo.mementos.Add(tempList);
            }

            mousePositions = null;
            lineRenderer = null;
            line = null;

            lineUndo.undoCount = lineUndo.mementos.Count - 1;
        }
    }


    // 일정 거리 안에 보드가 있는지 체크
    void CheckBoard()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, drawDistance, pageMask))
        {
            if(hitPage != hit.transform)
            {
                hitPage = hit.transform;
            }
            d_enable = true;
        }

        else
        {
            d_enable = false;
        }
    }

    // 클릭한 오브젝트가 보드인지 확인
    bool CheckClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, pageMask))
        {
            return true;
        }

        return false;
    }

}
