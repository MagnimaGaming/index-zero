using System.Data;
using UnityEngine;


[System.Serializable]
public class GridRegion : MonoBehaviour
{

    public string gridRegion;
    public ConnectivityType connectivityType;


    [Header("Grid parameters")]
    public Vector3 localOrigin;
    public int rows;
    public int columns;
    public float rowSpacing;
    public float columnSpacing;


    public float snapRange = 0.05f;

    [HideInInspector]
    public Bounds localBounds;


    public void CalculateBounds()
    {
        Vector3 size = new Vector3(rows * rowSpacing, 0.01f, columns * columnSpacing);

        Vector3 center = new Vector3(size.x / 2, 0, size.z / 2);

        localBounds = new Bounds(center, size);
    }


    public enum ConnectivityType
    {
        columnsConnected,
        rowsConnected
    }

}
