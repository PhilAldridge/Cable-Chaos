using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pipeScript : MonoBehaviour
{
    public GameObject edge1, edge2, edge3, edge4, edge5, edge6, edge7, edge8, edge9, edge10, edge11, edge12, corner1, corner2, corner3, corner4, corner5, corner6, corner7, corner8;
    // Start is called before the first frame update
    void Start()
    {
        //Resize(new Vector3(4, 4, 4), new Vector3(3, 4, 5));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Resize(Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 mins = new Vector3(Mathf.Min(startPoint.x, endPoint.x), Mathf.Min(startPoint.y,endPoint.y),Mathf.Min(startPoint.z,endPoint.z));
        Vector3 maxs = new Vector3(Mathf.Max(startPoint.x, endPoint.x), Mathf.Max(startPoint.y, endPoint.y), Mathf.Max(startPoint.z, endPoint.z));
        Vector3 gaps = new Vector3((maxs.x - mins.x), (maxs.y - mins.y), (maxs.z - mins.z));

        //upwards edges
        edge1.transform.localPosition = mins;
        edge1.transform.localScale = new Vector3(1, gaps.y, 1);

        edge2.transform.localPosition = new Vector3(mins.x, mins.y, maxs.z);
        edge2.transform.localScale = new Vector3(1, gaps.y, 1);

        edge3.transform.localPosition = new Vector3(maxs.x, mins.y, maxs.z);
        edge3.transform.localScale = new Vector3(1, gaps.y, 1);

        edge4.transform.localPosition = new Vector3(maxs.x, mins.y, mins.z);
        edge4.transform.localScale = new Vector3(1, gaps.y, 1);

        //top edges
        edge5.transform.localPosition = new Vector3(mins.x, maxs.y, mins.z);
        edge5.transform.localScale = new Vector3(1, gaps.z, 1);

        edge6.transform.localPosition = new Vector3(maxs.x, maxs.y, mins.z);
        edge6.transform.localScale = new Vector3(1, gaps.x, 1);

        edge7.transform.localPosition = new Vector3(maxs.x, maxs.y, maxs.z);
        edge7.transform.localScale = new Vector3(1, gaps.z, 1);

        edge8.transform.localPosition = new Vector3(mins.x, maxs.y, maxs.z);
        edge8.transform.localScale = new Vector3(1, gaps.x, 1);

        //bottom edges
        edge9.transform.localPosition = new Vector3(maxs.x, mins.y, maxs.z);
        edge9.transform.localScale = new Vector3(1, gaps.x, 1);

        edge10.transform.localPosition = new Vector3(mins.x, mins.y, maxs.z);
        edge10.transform.localScale = new Vector3(1, gaps.z, 1);

        edge11.transform.localPosition = new Vector3(maxs.x, mins.y, mins.z);
        edge11.transform.localScale = new Vector3(1, gaps.z, 1);

        edge12.transform.localPosition = new Vector3(mins.x, mins.y, mins.z);
        edge12.transform.localScale = new Vector3(1, gaps.x, 1);

        //corners
        corner1.transform.localPosition = new Vector3(mins.x, mins.y, mins.z);
        corner2.transform.localPosition = new Vector3(mins.x, maxs.y, mins.z);
        corner3.transform.localPosition = new Vector3(mins.x, maxs.y, maxs.z);
        corner4.transform.localPosition = new Vector3(mins.x, mins.y, maxs.z);
        corner5.transform.localPosition = new Vector3(maxs.x, mins.y, maxs.z);
        corner6.transform.localPosition = new Vector3(maxs.x, mins.y, mins.z);
        corner7.transform.localPosition = new Vector3(maxs.x, maxs.y, mins.z);
        corner8.transform.localPosition = new Vector3(maxs.x, maxs.y, maxs.z);

        //setactive
    }
}
