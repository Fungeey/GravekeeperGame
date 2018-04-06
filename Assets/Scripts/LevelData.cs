using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelData {

    // a class that holds data about the level
    // GameController holds a list of these, so you can undo.
    // Whenever the player moves, make one of these based on level

    // Things to keep track of: 
    // main / ground layers
    // Souls / Player objects

    public Tilemap main, ground; // Hold copies of level tiles
    public List<GameObject> souls; // Hold positions of souls
    public GameObject player; // Hold position of Player

    public LevelData(Tilemap main, Tilemap ground, List<GameObject> souls, GameObject player) {
        this.main = main;
        this.ground = ground;
        this.souls = souls;
        this.player = player;
    }
}
