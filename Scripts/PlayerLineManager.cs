using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLineManager : FuncManager
{
    public TextMesh textManager;

    private void Start()
    {
        textManager.text = player.l_width.ToString();
        textManager.color = player.l_color;
    }


    private void Update()
    {
        if(CheckClick())
        {
            player.l_width += 0.01f;
            textManager.text = player.l_width.ToString("F2");
        }

        else if(CheckRightClick())
        {
            player.l_width -= 0.01f;
            textManager.text = player.l_width.ToString("F2");
        }
    }
}
