using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteFunc : FuncManager
{
    AudioSource myAudio;


    private void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    public void DeleteChilds()
    {
        if(player.hitPage != null)
        {
            for(int i = 0; i < player.hitPage.childCount; i++)
            {
                Destroy(player.hitPage.GetChild(i).gameObject);
            }

            player.lineDatas.list.Clear();

            if(player.hitPage.childCount > 0)
            {
                if(player.lineUndo.mementos.Count < player.lineUndo.ListLimited)
                {
                    player.lineUndo.mementos.Add(null);
                }

                else
                {
                    player.lineUndo.mementos.RemoveAt(0);
                    player.lineUndo.mementos.Add(null);
                }
            }
        }

        //player.lineUndo.undoCount = player.lineUndo.mementos.Count - 1;
    }

    private void Update()
    {
        if(CheckClick())
        {
            myAudio.Play();
            DeleteChilds();
        }
    }


}
