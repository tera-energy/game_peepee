using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeePeeAddObstacle1 : MonoBehaviour
{
    static PeePeeAddObstacle1 _instance;
    public static PeePeeAddObstacle1 xInstance { get { return _instance; } }

    public Sprite[] _spAddObstacle;
    public SpriteRenderer _spriteRenderer;
    [SerializeField] GameObject _addObstacle1;
    void Update()
    {
        /*if (Player.xInstance._isObstacleTag == true)
        {
            _addObstacle1.tag = "Untagged";
        }
        else if (Player.xInstance._isObstacleTag == false)
        {
            _addObstacle1.tag = "Obstacle";
        }*/

    }


    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
}
