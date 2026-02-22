using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int rows = 5;
    public int columns = 5;

    public float snapRange = 0.1f;
    public float spacing = 0.2f;


    public bool showGizmos = true;
    private Vector3 gridOrigin;

    private void Start()
    {
        gridOrigin = transform.position;
    }

    public bool TryGetSnapPoint(Vector3 objWorldPos, out Vector3 snapPoint)
    {
        Vector3 localPos = objWorldPos - gridOrigin;

        int col = Mathf.RoundToInt(localPos.x / spacing);
        int row = Mathf.RoundToInt(localPos.z / spacing);

        col = Mathf.Clamp(col, 0, columns - 1);
        row = Mathf.Clamp(row, 0, rows - 1);

        snapPoint = gridOrigin + new Vector3(col * spacing, 0, row * spacing);

        float distance = Vector3.Distance(objWorldPos, snapPoint);
        return distance <= snapRange;

    }


    private void OnDrawGizmos()
    {
        if(!showGizmos) return;
        Vector3 origin = Application.isPlaying ? gridOrigin : transform.position;
        Gizmos.color = Color.yellow;

        for(int row = 0; row < rows;  row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 pointPos = gridOrigin + new Vector3(col * spacing, 0, row * spacing);
                Gizmos.DrawWireSphere(pointPos, snapRange);
            }
        }
    }
}
