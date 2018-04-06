using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileObject))]
public class SoulController : MonoBehaviour {
    
    public Tile fullGravestone;
    public GameController gameController;
    private TileObject soulTileObject;

    private void Start() { 

        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        soulTileObject = gameObject.GetComponent<TileObject>();
    }

    private void Update() {
        // If you are on a gravestone
        TileBase tile = soulTileObject.tilemaps[0].GetTile(soulTileObject.tilePos);
        if (tile != null && !soulTileObject.moving && tile.name == "Gravestone") {
            // Check if all the gravestones are full

            // Then delete the gravestone
            soulTileObject.tilemaps[0].SetTile(soulTileObject.tilePos, null);
            // Put a gravestone in the Main (solid) layer
            soulTileObject.tilemaps[1].SetTile(soulTileObject.tilePos, fullGravestone);
            // Destroy this soul
            Destroy(gameObject);
        }
    }
}
