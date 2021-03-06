﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public struct LevelData {

    // a structure that holds data about the level
    // GameController holds a list of these, so you can undo.
    // Whenever the player moves, make one of these based on level

    // Things to keep track of: 
    // main / ground layers
    // states of all TileObjects

    public TileBase[][] maps; // Hold copies of level tiles
    public BoundsInt bounds;
    public TileObject.TileObjectState[] states; // Hold positions of souls

    public LevelData(BoundsInt totalLevelBounds, Tilemap[] maps, TileObject.TileObjectState[] states) {
        this.maps = new TileBase[2][];
        this.bounds = totalLevelBounds;
        for (int i = 0; i < 2; i++) {
            this.maps[i] = maps[i].GetTilesBlock(totalLevelBounds);
        }
        this.states = states;
    }
}
