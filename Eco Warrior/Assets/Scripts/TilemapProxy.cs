using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapProxy : MonoBehaviour
{
    public Tilemap tilemap; 
    public TileBase floortile; 
    public GameObject proxyPrefab;
    public Transform parentContainer; 
    public bool clearOldProxies = true;
    private HashSet<Vector3> positions = new HashSet<Vector3>();
    [ContextMenu("Generate Proxies")]
    public void GenerateProxies()
    {
        
            
            if (tilemap == null || proxyPrefab == null) return;

            if (clearOldProxies && parentContainer != null)
            {
                foreach (Transform child in parentContainer)
                {
                    DestroyImmediate(child.gameObject);
                }
                positions.Clear();
            }

            BoundsInt bounds = tilemap.cellBounds;
            int count = 0;

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, 0);
                   
                    TileBase tile = tilemap.GetTile(cellPos);

                    if (tile == null) continue;

                    Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);



                    Vector3Int test = tilemap.WorldToCell(worldPos);
                    tile = tilemap.GetTile(test);
                    if (tile == null) continue;
                    if (positions.Contains(worldPos))
                    {
                        continue;
                    }
                    GameObject proxy = (GameObject)PrefabUtility.InstantiatePrefab(proxyPrefab);
                    proxy.transform.position = worldPos;
                    proxy.transform.SetParent(parentContainer != null ? parentContainer : this.transform);
                    count++;
                    positions.Add(worldPos);
                }
            }
        

    }
}
