using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncManager : MonoBehaviour
{

    public PlayerDrawLine player;
    public float distance;

    protected bool CheckClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, distance))
            {
                if(hit.transform == transform)
                {
                    return true;
                }
            }

        }
        return false;
    }

    protected bool CheckRightClick()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, distance))
            {
                if(hit.transform == transform)
                {
                    return true;
                }
            }

        }
        return false;
    }


    protected bool CheckKeepClick()
    {
        if(Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, distance))
            {
                if(hit.transform == transform)
                {
                    return true;
                }
            }

        }
        return false;
    }
}
