using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[NodeHeader]
public class GameManager : MonoBehaviourSingleton<GameManager>
{
     public Player playerPrefab;
     public GameObject enemyPrefab;

    [Net] public Player player1;
     public Dictionary<int, Player> players;


    private void Awake()
    {
        players = new Dictionary<int, Player>();
    }


    private void Start()
    {
        player1 = Instantiate(playerPrefab);
        player1.life = 44;
        player1.damage = 76;
        player1.playerName = "pepe";
        player1.playerDescription = "Ni idea";
    }
}


