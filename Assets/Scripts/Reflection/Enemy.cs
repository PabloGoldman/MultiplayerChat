using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
    public string enemyName;
    public string enemyDescription;

    [Net] public int life;
    [Net] public int damage;
}
