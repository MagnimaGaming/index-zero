using System.Data;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class GridRegion
{
    [Header("Identity")]
    public string regionName;
    public ConnectivityType connectivityType;


    [Header("Grid Related parameters")]
    public Vector3 localOrigin;
    public Transform originMaker;
    public int rows;
    public int columns;
    public float rowSpacing;
    public float columnSpacing;

    public bool isSegmentedCol = false;
    public int segmentedColSize = 5;
    public float segmentedColGap = 0.0592f;

    public float snapRange = 1f;
    Vector3 size;
    Vector3 center;

    [HideInInspector]
    public Bounds localBounds;


    public void CalculateBounds(Transform breadboardRoot)
    {
        if(originMaker != null)
        {
            localOrigin = breadboardRoot.InverseTransformPoint(originMaker.position);
        }

        float totalWidth;

        if (isSegmentedCol)
        {
            int lastCol = columns - 1;
            totalWidth = GetColumnXOffset(lastCol);
        }
        else
        {
            totalWidth = columns * columnSpacing;
        }


        size = new Vector3(rows * rowSpacing, 0.01f,totalWidth);

        center = localOrigin + new Vector3(size.x / 2, 0, size.z / 2);

        localBounds = new Bounds(center, size);


    }


    public float GetColumnXOffset(int col)
    {
        if (isSegmentedCol)
        {
            int colBlockIndex = col / segmentedColSize;
            return col * columnSpacing + colBlockIndex * segmentedColGap;
        }
        else
        {
            return col * columnSpacing;
        }
    }


    public enum ConnectivityType
    {
        columnsConnected,
        rowsConnected
    }

}
