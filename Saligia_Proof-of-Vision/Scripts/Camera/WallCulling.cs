using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCulling : MonoBehaviour
{
    public Transform Top;
    public Transform Bottom;
    public Transform Camera;

    private bool oldState = true;

    private void OnDrawGizmosSelected()
    {
        bool top = false;
        bool bottom = false;

        if (Top != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(Top.position, 1f);
            top = true;
        }

        if (Bottom != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Bottom.position, 1f);
            bottom = true;
        }

        if (top && bottom)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Bottom.position, Top.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        var top = new Vector2(Top.position.x, Top.position.z);
        var bottom = new Vector2(Bottom.position.x, Bottom.position.z);
        var camera = new Vector2(Camera.position.x, Camera.position.z);

        //has Changed?
        if (OrientationTest.IsLeftOf(camera, bottom, top) != oldState)
        {
            if (oldState)
            {
                //show
                setTransparancy(0.3f);
                oldState = false;
            }
            else
            {
                //hide
                setTransparancy(1f);
                oldState = true;
            }
        }
    }

    private void setTransparancy(float alpha)
    {
        foreach (var item in GetComponentsInChildren<Renderer>())
        {
            foreach (var material in item.materials)
            {
                Color current = material.color;
                Color newColor = new Color(current.r, current.g, current.b, alpha);
                material.color = newColor;
            }
        }
    }
}
