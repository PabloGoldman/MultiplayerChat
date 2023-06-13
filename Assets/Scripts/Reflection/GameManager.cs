using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[NodeHeader]
public class GameManager : MonoBehaviourSingleton<GameManager>
{
     public GameObject playerPrefab;
     public GameObject enemyPrefab;

    [Net] Player player1;

    private void Start()
    {
        player1 = Instantiate(playerPrefab).AddComponent<Player>();
        player1.life = 44;
        player1.damage = 76;
        player1.playerName = "pepe";
        player1.playerDescription = "Ni idea";
    }
}


