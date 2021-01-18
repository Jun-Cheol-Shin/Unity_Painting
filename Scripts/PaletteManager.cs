using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteManager : FuncManager
{

    public PlayerLineManager manager;

    void test(Ray ray)
    {
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            Texture2D TextureMap = hit.transform.GetComponent<Renderer>().material.mainTexture as Texture2D;
            Color color = TextureMap.GetPixel((int)(hit.textureCoord.x * TextureMap.width), (int)(hit.textureCoord.y * TextureMap.height));// w*h = 512*512(file)

            player.l_color = color;
            manager.textManager.color = color;
        }
    }

    private void Update()
    {
        if(CheckKeepClick())
        {
            test(Camera.main.ScreenPointToRay(Input.mousePosition));
        }
    }
}
