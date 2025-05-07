using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase[] openTile;
    [SerializeField] private Vector3Int[] doorPositions;

    public void OpenWeaponRoomDoors()
    {
        tilemap.SetTile(doorPositions[0], openTile[0]);
        tilemap.SetTile(doorPositions[1], openTile[1]);
    }
    //Help method to get coordinates for specific tiles
    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
    //        Debug.Log($"Tile clicked at: {cellPos}");
    //    }
    //}
}
