using UnityEngine;

public class Chamber : MonoBehaviour
{
    public int rows = 3;
    public int cols = 3;
    public Vector3 chamberSize = new Vector3(20f, 5f, 20f);
    public bool[] gasMask;

    private void Awake()
    {
        GenerateCells();
    }

    [ContextMenu("Regenerate Cells")]
    public void GenerateCells()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(child);
            else
#endif
                Destroy(child);
        }

        int total = rows * cols;
        bool[] mask = (gasMask != null && gasMask.Length == total) ? gasMask : new bool[total];

        float cellW = chamberSize.x / cols;
        float cellD = chamberSize.z / rows;
        float cellH = chamberSize.y;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int idx = r * cols + c;

                var cellGo = new GameObject($"ChamberCell_{r}_{c}");
                cellGo.transform.SetParent(transform, false);

                float lx = -chamberSize.x / 2f + (c + 0.5f) * cellW;
                float lz = -chamberSize.z / 2f + (r + 0.5f) * cellD;
                cellGo.transform.localPosition = new Vector3(lx, 0f, lz);

                var col = cellGo.AddComponent<BoxCollider>();
                col.size = new Vector3(cellW, cellH, cellD);
                col.isTrigger = true;

                var zone = cellGo.AddComponent<Zone>();
                zone.type = Zone.ZoneType.ChamberCell;
                zone.hasGas = mask[idx];
                zone.parentChamber = this;
            }
        }
    }
}
