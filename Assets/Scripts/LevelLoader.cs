using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelLoader : MonoBehaviour {

    [Tooltip("Order: [Player] [Soul] [Movable Wall]")]
    public GameObject[] objects; // Array of objects to replace sprites with

    private Grid levelGrid;
    private Tilemap tileMap;

    public List<Vector3Int> gravestoneLocations;
    public GameController gameController;

	// Use this for initialization
	void Start () {
        levelGrid = GetComponent<Grid>();
        tileMap = levelGrid.gameObject.transform.Find("Main").GetComponent<Tilemap>();
        tileMap.CompressBounds();

        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();

        LoadLevel();
        CenterCamera();
    }
	
	void LoadLevel () {
		for(int i = tileMap.cellBounds.xMin; i < tileMap.cellBounds.xMax; i++) {
            for(int j = tileMap.cellBounds.yMin; j < tileMap.cellBounds.yMax; j++) {
                Vector3Int pos = new Vector3Int(i, j, 0);
                TileBase tile = tileMap.GetTile(pos);
                string name;
                if (tile != null) {
                    name = tile.name;

                    switch (name) {
                        case "Player": {
                            tileMap.SetTile(pos, null);
                            InstTileObject(objects[0], pos);
                            break;
                        }
                        case "Soul": {
                            tileMap.SetTile(pos, null);
                            InstTileObject(objects[1], pos);
                            break;
                        }
                        case "Movable Wall": {
                            tileMap.SetTile(pos, null);
                            InstTileObject(objects[2], pos);
                            break;
                        }
                    }
                }
            }
        }
    }

    void CenterCamera() {
        Tilemap ground = levelGrid.transform.Find("Ground").GetComponent<Tilemap>();
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        ground.CompressBounds();
        Vector3 center = ground.localBounds.center;
        Utility.MarkPoint(ground.localBounds.min, Color.red, 20);
        Utility.MarkPoint(ground.localBounds.max, Color.yellow, 20);
        center.z = -10;
        camera.transform.position = ground.transform.TransformPoint(center);
    }

    public void InstTileObject(GameObject tile, Vector3Int tilePos) {
        Debug.Log("Instantiated at " + tilePos);
        Instantiate(tile, tileMap.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity, gameController.tileObjHolder.transform);
    }
    
}
