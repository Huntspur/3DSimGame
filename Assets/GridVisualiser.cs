using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float yOffset = 0.01f; // avoids z-fighting with the plane

    private void OnDrawGizmos()
    {
        if (grid == null) return;
        Gizmos.color = Color.white;

        for (int x = 0; x <= width; x++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(x, 0, 0)) + Vector3.up * yOffset;
            Vector3 end = grid.CellToWorld(new Vector3Int(x, 0, height)) + Vector3.up * yOffset;
            Gizmos.DrawLine(start, end);
        }
        for (int z = 0; z <= height; z++)
        {
            Vector3 start = grid.CellToWorld(new Vector3Int(0, 0, z)) + Vector3.up * yOffset;
            Vector3 end = grid.CellToWorld(new Vector3Int(width, 0, z)) + Vector3.up * yOffset;
            Gizmos.DrawLine(start, end);
        }
    }
}