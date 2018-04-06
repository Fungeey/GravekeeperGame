﻿using System.Collections;
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
    private Transform soulHolder; // Parent holder of all souls, so GameController can find them for undo

	// Use this for initialization
	void Start () {
        levelGrid = GetComponent<Grid>();
        tileMap = levelGrid.gameObject.transform.Find("Main").GetComponent<Tilemap>();
        tileMap.CompressBounds();

        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        soulHolder = gameController.transform.Find("SoulHolder"); //Find the SoulHolder parented under gamecontroller

        LoadLevel();
        CenterCamera();
    }
	
	// Update is called once per frame
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
                            Instantiate(objects[0], tileMap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
                            break;
                        }
                        case "Soul": {
                            tileMap.SetTile(pos, null);
                            Instantiate(objects[1], tileMap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity, soulHolder);
                            break;
                        }
                        case "Movable Wall": {
                            tileMap.SetTile(pos, null);
                            Instantiate(objects[2], tileMap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
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
}
