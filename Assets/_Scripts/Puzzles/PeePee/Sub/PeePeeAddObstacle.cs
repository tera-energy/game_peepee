using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeePeeAddObstacle : MonoBehaviour
{
    static PeePeeAddObstacle _instance;
    public static PeePeeAddObstacle xInstance { get { return _instance; } }

    public Sprite[] _spAddObstacle;
    public SpriteRenderer _spriteRenderer;
    [SerializeField] GameObject _addObstacle;

    void Update()
    {

    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
}
