using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[NodeHeader]
public class GameManager : MonoBehaviour
{
     public GameObject playerPrefab;
     public GameObject enemyPrefab;

    [Net] Player player;
    [Net] List<Enemy> enemies;

    private void Start()
    {
        enemies = new List<Enemy>();

        player = Instantiate(playerPrefab).AddComponent<Player>();
        player.life = 44;
        player.damage = 76;
        player.playerName = "pepe";
        player.playerDescription = "Ni idea";

        for (int i = 0; i < 5; i++)
        {
            Enemy newEnemy = Instantiate(enemyPrefab).AddComponent<Enemy>();

            newEnemy.life = 88;
            newEnemy.damage = 188;
            newEnemy.enemyName = "jose " + i;
            newEnemy.enemyDescription = "nose";

            enemies.Add(newEnemy);
        }
    }
}


