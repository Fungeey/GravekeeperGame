using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SoulController : TileObject {

    public Tile ground;
    public Tile fullGravestone;

    protected override void FixedUpdate() {
        base.FixedUpdate();

        // If you are on a gravestone
        TileBase tile = tilemaps[0].GetTile(tilePos);

        if (tile == null && !moving) {
            gameObject.SetActive(false);
        }

        if (tile != null && !moving && !grabComp.isHeld && tile.name == "Gravestone") {
            // fill gravestone
            tilemaps[0].SetTile(tilePos, ground);
            tilemaps[1].SetTile(tilePos, fullGravestone);

            // Deactivate it
            gameObject.SetActive(false);
            // For some reason putting this after CheckWin() causes it not to fire

            gameController.CheckWin();
            
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
