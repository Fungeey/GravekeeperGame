using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SoulController : MonoBehaviour {

    private Tilemap[] tilemaps;
    private Vector3Int tilePos;
    public Tile gravestone;
    private Grid levelGrid;
    public GameController gameController;
    private TileObject soulTileObject;

    private void Start() { 

        levelGrid = GameObject.FindGameObjectWithTag("LevelGrid").GetComponent<Grid>();
        tilemaps = levelGrid.GetComponentsInChildren<Tilemap>();
        tilePos = levelGrid.WorldToCell(transform.position);

        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        soulTileObject = gameObject.GetComponent<TileObject>();
    }

    private void Update() {
        // If you are on a gravestone
        if (tilemaps[0].GetTile(soulTileObject.tilePos).name == "Gravestone") {
            // Check if all the gravestones are full
            gameController.CheckWin();

            // Then delete the gravestone
            tilemaps[0].SetTile(soulTileObject.tilePos, null);
            // Put a gravestone in the Main (solid) layer
            tilemaps[1].SetTile(soulTileObject.tilePos, gravestone);
            // Destroy this soul
            Destroy(gameObject);
        }
    }
}
