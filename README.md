# #### Unity_VR_Painting
<img src="https://user-images.githubusercontent.com/58795584/100821297-6dafb280-3493-11eb-9cf2-129267085fbd.PNG">

+ SteamVR Player을 기반으로 라인렌더러를 이용해 선을 그리는 툴을 만들어 보았습니다.
------

# 1. 기능 소개
## ⓐ 저장, 로드, 지우기
+ 각각의 ATM기를 왼쪽 클릭하면 ATM기에 맞는 기능이 실행됩니다.
+ SAVE는 현재 그려진 그림을 저장합니다.
+ LOAD는 가장 최근에 저장된 세이브 지점을 불러옵니다.
+ CLEAN은 그려진 그림이 모두 지워집니다.

<img src="https://user-images.githubusercontent.com/58795584/100821645-2544c480-3494-11eb-8c33-df1989d71d84.PNG">

## ⓑ 뒤로, 앞으로, 굵기, 색깔
+ 현재 그려진 그림에서 뒤로가기, 앞으로 가기 기능을 구현했습니다.
+ **좌 클릭시 뒤로, 우 클릭시 앞으로 입니다.**
+ 오른쪽 0.10은 라인의 굵기, 텍스트 컬러는 현재 라인의 색깔을 의미합니다.
+ 좌 클릭은 숫자가 증가 우 클릭은 숫자가 감소하며 숫자에 따라 라인의 두께가 달라집니다.

<img src="https://user-images.githubusercontent.com/58795584/100821773-64731580-3494-11eb-99b3-239ab151e00e.PNG">

## ⓒ 팔레트
+ 클릭한 팔레트의 색깔에 따라 라인의 색깔이 달라집니다.
+ **왼 클릭**시 색깔이 바뀝니다.

<img src="https://user-images.githubusercontent.com/58795584/100821930-b0be5580-3494-11eb-809f-b8a87a08bde4.PNG">

------

# 2. 구현 요약
## ⓐ 저장, 로드 구현
+ 기본적으로 라인은 벡터의 집합으로 이루어져 있습니다. 저장 데이터를 벡터의 집합을 string의 한 문장으로 만들어봤습니다.
```C#
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
```
+ 이러한 세이브 데이터(string)을 이용하여 파싱을 통해 다시 데이터를 받습니다.
```C#
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
         }
```
+ 마지막으로 파싱한 데이터들을 이용해 오브젝트 생성 및 컴포넌트 추가, 데이터 변경을 합니다.

## ⓑ 팔레트 클릭 구현
+ 팔레트는 오브젝트는 Plane에 텍스쳐를 입힌 뒤 생성시켰습니다.
+ 클릭 시 색깔을 가져오는 부분을 간단하게 구현했습니다.
```C#
        if(Physics.Raycast(ray, out hit))
        {
            Texture2D TextureMap = hit.transform.GetComponent<Renderer>().material.mainTexture as Texture2D;
            Color color = TextureMap.GetPixel((int)(hit.textureCoord.x * TextureMap.width), (int)(hit.textureCoord.y * TextureMap.height));// w*h = 512*512(file)

            player.l_color = color;
            manager.textManager.color = color;
        }
```
## ⓒ Undo, Redo
+ 마지막으로 뒤로, 앞으로 구현은 **메멘토 패턴**을 이용하여 구현했습니다.
+ 객체의 상태 즉, 라인이 그려진 상태를 저장하는 리스트를 따로 만듭니다.
+ 다시 말해, 라인의 집합을 다시 리스트로 집합시키는 쪽으로 구현해보았습니다.
+ 최대로 저장할 수 있는 개수를 제한하며, 지울 때도 메멘토에 추가됩니다.
+ 전부 지우고 객체에 저장된 상태를 그대로 다시 그려내도록 구현했습니다.
``` C#
// data는 Memento 즉 객체(라인) 상태를 저장한 리스트
for (int i = 0; i < player.hitPage.childCount; i++)
    {
        Destroy(player.hitPage.GetChild(i).gameObject);
        player.lineDatas.list.Clear();
    }

if (data != null)
{
    for (int i = 0; i < data.list.Count; i++)
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

        for (int j = 0; j < data.list[i].mousePos.Count; j++)
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
```

