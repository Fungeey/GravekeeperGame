using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelLoader : MonoBehaviour {

    [Tooltip("Order: [Player] [Soul] [Movable Wall]")]
    public GameObject[] objects; // Array of objects to replace sprites with

    private Grid levelGrid;
    private Tilemap main;
    private Tilemap ground;

    public List<Vector3Int> gravestoneLocations;
    public GameController gameController;

    // Use this for initialization
    void Start() {
        levelGrid = GetComponent<Grid>();
        main = levelGrid.gameObject.transform.Find("Main").GetComponent<Tilemap>();
        ground = levelGrid.gameObject.transform.Find("Ground").GetComponent<Tilemap>();

        main.CompressBounds();
        ground.CompressBounds();

        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();

        LoadLevel();
        CenterCamera();
    }

    void LoadLevel() {
        for (int i = main.cellBounds.xMin; i < main.cellBounds.xMax; i++) {
            for (int j = main.cellBounds.yMin; j < main.cellBounds.yMax; j++) {
                Vector3Int pos = new Vector3Int(i, j, 0);
                TileBase tile = main.GetTile(pos);
                string name;
                if (tile != null) {
                    name = tile.name;

                    switch (name) {
                        case "Player": {
                            main.SetTile(pos, null);
                            InstTileObject(objects[0], pos);
                            break;
                        }
                        case "Soul": {
                            main.SetTile(pos, null);
                            InstTileObject(objects[1], pos);
                            break;
                        }
                        case "Movable Wall": {
                            main.SetTile(pos, null);
                            InstTileObject(objects[2], pos);
                            break;
                        }
                    }
                }
            }
        }
    }

    void CenterCamera() {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        Vector3 center = ground.localBounds.center;
        //Utility.MarkPoint(ground.localBounds.min, Color.red, 20);
        //Utility.MarkPoint(ground.localBounds.max, Color.yellow, 20);
        center.z = -10;
        camera.transform.position = ground.transform.TransformPoint(center);
    }

    public void InstTileObject(GameObject tile, Vector3Int tilePos) {
        Debug.Log("Instantiated at " + tilePos);
        Instantiate(tile, main.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity, gameController.tileObjHolder.transform);
    }

    public BoundsInt MaximumLevelBounds() {
        BoundsInt bounds = new BoundsInt {
            xMin = System.Math.Min(main.cellBounds.xMin, ground.cellBounds.xMin) - 1,
            yMin = System.Math.Min(main.cellBounds.yMin, ground.cellBounds.yMin) - 1,
            xMax = System.Math.Max(main.cellBounds.xMin, ground.cellBounds.xMax) + 1,
            yMax = System.Math.Max(main.cellBounds.yMax, ground.cellBounds.yMax) + 1
        };
        Debug.Log(bounds);

        Utility.MarkPoint(bounds.min, Color.red, 20);
        Utility.MarkPoint(bounds.max, Color.yellow, 20);
        return bounds;
    }
}
