using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SoulController : TileObject {

    public Tile ground;
    public Tile fullGravestone;
    private TileObject soulTileObject;

    private void Start() {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    private void Update() {
        // If you are on a gravestone
        TileBase tile = tilemaps[0].GetTile(tilePos);
        if (tile != null && !moving && tile.name == "Gravestone") {
            // Check if all the gravestones are full

            // Then delete the gravestone
            tilemaps[0].SetTile(tilePos, ground);
            // Put a gravestone in the Main (solid) layer
            tilemaps[1].SetTile(tilePos, fullGravestone);
            // Deactivate it
            gameObject.SetActive(false);
        }
    }

    public override TileObjectState GetState() {
        Dictionary<string, int> dict = new Dictionary<string, int> {
            { "active", gameObject.activeSelf ? 1 : 0 }
        };
        return new TileObjectState(tilePos, dict);
    }

    public override void SetState(TileObjectState state) {
        base.SetState(state);
        gameObject.SetActive(state.additionalData["active"] == 1);
    }
}
