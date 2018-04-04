using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelLoader : MonoBehaviour {

    [Tooltip("Order: [Player] [Soul] [Movable Wall]")]
    public GameObject[] objects; // Array of objects to replace sprites with

    private Grid levelGrid;
    private Tilemap tileMap;
    private int width, length;
	// Use this for initialization
	void Start () {
        levelGrid = GetComponent<Grid>();
        tileMap = levelGrid.gameObject.transform.Find("Main").GetComponent<Tilemap>();
        width = tileMap.size.x;
        length = tileMap.size.y;
        LoadLevel();
    }
	
	// Update is called once per frame
	void LoadLevel () {
		for(int i = 0; i < width; i++) {
            for(int j = 0; j < length; j++) {
                Vector3Int pos = new Vector3Int(i, j, 0);
                TileBase tile = tileMap.GetTile(pos);
                string name;
                if (tile != null) {
                    name = tile.name;

                    switch (name) {
                        case "Player": {
                            tileMap.SetTile(pos, null);
                            Instantiate(objects[0], tileMap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
                            break;
                        }
                        case "Soul": {
                            tileMap.SetTile(pos, null);
                            Instantiate(objects[1], tileMap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
                            break;
                        }
                        case "Movable Wall": {
                            tileMap.SetTile(pos, null);
                            Instantiate(objects[2], tileMap.CellToWorld(pos), Quaternion.identity);
                            break;
                        }
                    }
                }
            }
        }
	}
}
