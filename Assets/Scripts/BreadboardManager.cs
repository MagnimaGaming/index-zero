using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BreadboardManager : MonoBehaviour
{

    public Transform breadboardRoot;

    public GridRegion topPowerRail;
    public GridRegion bottomPowerRail;
    public GridRegion mainArea;

    public List<GridRegion> allRegions;

    public bool showGizmos = true;


    private void Start()
    {
        InitializeAllRegions();
    }


    private void InitializeAllRegions()
    {
        topPowerRail.CalculateBounds();
        bottomPowerRail.CalculateBounds();
        mainArea.CalculateBounds();


        allRegions = new List<GridRegion> { topPowerRail, mainArea,  bottomPowerRail };
    }




    public bool TryGetSnapPoint(Vector3 legWorldPos, out Vector3 snapPoint, out GridRegion snapRegion)
    {
        Vector3 legLocalPos = legWorldPos - breadboardRoot.transform.position;

        foreach (GridRegion region in allRegions)
        {
            if (region.localBounds.Contains(legLocalPos))
            {
                Vector2Int gridCoord = CalculateNearestCoord(legLocalPos, region);
                Vector3 localHolePos = GetLocalHolePosition(gridCoord,  region);

                float distance = Vector3.Distance(localHolePos, legLocalPos);

                if(distance <= region.snapRange)
                {
                    snapPoint = breadboardRoot.transform.position + localHolePos;
                    snapRegion = region;
                    return true;
                }
            }
            snapPoint = Vector3.zero;
            snapRegion = null;
            return false;
        }
    }




    private Vector2Int CalculateNearestCoord(Vector3 legLocalPos, GridRegion region)
    {
        Vector3 legPosRelToRegion = legLocalPos - region.localOrigin;

        int col = Mathf.RoundToInt(legPosRelToRegion.x / region.columnSpacing); 
        int row = Mathf.RoundToInt(legPosRelToRegion.z / region.rowSpacing);

        col = Mathf.Clamp(col, 0, region.columns - 1);
        row = Mathf.Clamp(row, 0, region.rows - 1);

        return new Vector2Int(row, col);

    }

    private Vector3 GetLocalHolePosition(Vector2Int gridCoord, GridRegion region)
    {
        float x = region.localOrigin.x + gridCoord.x * region.columnSpacing;
        float y = region.localOrigin.z + gridCoord.y * region.rowSpacing;
        float z = region.localOrigin.y;

        return new Vector3(x, y, z);
    }



    private void OnDrawGizmos()
    {
        if(!showGizmos) return;
        if (breadboardRoot == null) return;
        if (!Application.isPlaying) return;

        Color[] colors = { Color.blue, Color.green, Color.red, };
        int colorIndex = 0;

        foreach(GridRegion region in allRegions)
        {
            Gizmos.color = colors[colorIndex];

            for(int row = 0; row < region.rows; row++)
            {
                for(int col = 0; col < region.columns; col++)
                {
                    Vector2Int grodCoord = new Vector2Int(row, col);
                    Vector3 localHolePos = GetLocalHolePosition(grodCoord, region);
                    Vector3 worldPos = breadboardRoot.transform.position + localHolePos;

                    Gizmos.DrawWireSphere(worldPos, 0.01f);
                }
            }
            colorIndex++;
        }

    }
}
