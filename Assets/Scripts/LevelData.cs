using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct LevelData {

    // a class that holds data about the level
    // GameController holds a list of these, so you can undo.
    // Whenever the player moves, make one of these based on level

    // Things to keep track of: 
    // main / ground layers
    // Souls / Player objects

    public TileBase[][] maps; // Hold copies of level tiles
    public TileObject.TileObjectState[] states; // Hold positions of souls

    public LevelData(Tilemap[] maps, TileObject.TileObjectState[] states) {
        this.maps = new TileBase[2][];
        for (int i = 0; i < 2; i++) {
            this.maps[i] = maps[i].GetTilesBlock(maps[i].cellBounds);
            Debug.Log(i);
            foreach (TileBase tile in this.maps[i]) {
                Debug.Log(tile != null ? tile.name : "empty");
            }
        }
        this.states = states;
    }
}
