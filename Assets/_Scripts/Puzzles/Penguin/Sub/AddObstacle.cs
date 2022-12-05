using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class AddObstacle : MonoBehaviour
{
    static AddObstacle _instance;
    public static AddObstacle xInstance { get { return _instance; } }

    [Space]
    [Header("추가 장애물")]
    public AddObstacle _addObstacle;
    public AddObstacle _addInfoObstacle;
    [HideInInspector] public AddObstacle[] _arrAddObstacle;
    public int _maxAddIndex;
    float _addObstaclePosX;
    int _addObstacleRandPosX;
    bool _isAddObstacle = false;
    Vector3 _addObstacleFirPos;
    Vector3 _addObstacleFirScale;

    [HideInInspector] public List<AddObstacle> _listAddObstacle = new List<AddObstacle>();



    int _addObstacleType;



    public void ySetListAddObstacle(int index)
    {
        Vector3 scale = new Vector3(1.3f, 1.3f, 1.3f);
        Vector3 tartgetL = new Vector3(-11f, -10f, 0f);
        Vector3 tartgetR = new Vector3(11f, -10f, 0f);
        Vector3 tartgetM = new Vector3(0, -10f, 0f);

        _listAddObstacle.Add(_arrAddObstacle[index]);
        _arrAddObstacle[index].tag = "Untagged";
        _addObstacleType = Random.Range(0, 1);
        int randAdd = Random.Range(0, 1);
        _addObstacleRandPosX = Random.Range(0, 2);
        _addObstacleFirScale = new Vector3(0f, 0f, 0f);


        if (randAdd == 0)
        {
            if (_addObstacleType == 0)
            {
                _arrAddObstacle[index].transform.localScale = _addObstacleFirScale;
                Obstacle.xInstance.zObstacleSprite(0);
                if (Obstacle.xInstance._obstacleRandPosX == 0)
                {
                    if (_addObstacleRandPosX == 0)
                        _addObstaclePosX = 0f;
                    else if (_addObstacleRandPosX == 1)
                        _addObstaclePosX = 0.7f;
                }
                else if (Obstacle.xInstance._obstacleRandPosX == 1)
                {
                    if (_addObstacleRandPosX == 0)
                        _addObstaclePosX = -0.7f;
                    else if (_addObstacleRandPosX == 1)
                        _addObstaclePosX = 0.7f;
                }
                else if (Obstacle.xInstance._obstacleRandPosX == 2)
                {
                    if (_addObstacleRandPosX == 0)
                        _addObstaclePosX = -0.7f;
                    else if (_addObstacleRandPosX == 1)
                        _addObstaclePosX = 0f;
                }
            }

        }

        _addObstacleFirPos = new Vector3(_addObstaclePosX, 1.45f, 0);
        _arrAddObstacle[index].transform.localPosition = _addObstacleFirPos;
        _arrAddObstacle[index].gameObject.SetActive(true);
        _isAddObstacle = true;

        if (_isAddObstacle == true)
        {
            if (_addObstaclePosX == 0)
            {
                _listAddObstacle[index].transform.DOScale(scale, Obstacle.xInstance._scaleSpeed).SetEase(Ease.Linear);
                _listAddObstacle[index].transform.DOLocalMove(tartgetM, Obstacle.xInstance._obstacleSpeed).SetEase(Ease.Linear);
            }
            else if (_addObstaclePosX > 0)
            {
                _listAddObstacle[index].transform.DOScale(scale, Obstacle.xInstance._scaleSpeed).SetEase(Ease.Linear);
                _listAddObstacle[index].transform.DOLocalMove(tartgetR, Obstacle.xInstance._obstacleSpeed).SetEase(Ease.Linear);
            }
            else if (_addObstaclePosX < 0)
            {
                _listAddObstacle[index].transform.DOScale(scale, Obstacle.xInstance._scaleSpeed).SetEase(Ease.Linear);
                _listAddObstacle[index].transform.DOLocalMove(tartgetL, Obstacle.xInstance._obstacleSpeed).SetEase(Ease.Linear);
            }
        }

    }

    void yTagObstacle()
    {
        if (Player.xInstance._peepeeHit == false)
        {

            if (_addObstacle.transform.localPosition.y < -3.2f && AddObstacle.xInstance._addObstacle.transform.localPosition.y > -4.4f)
            {
                _addObstacle.tag = "Obstacle";
            }
            
        }
        else if (Player.xInstance._peepeeHit == true)
        {           
            _addObstacle.tag = "Untagged";
        }

        if (_addObstacle.transform.localPosition.y < -3.6f)
        {
            _addObstacle.tag = "Untagged";
        }

    }

    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    void Update()
    {
        yTagObstacle();
    }
}
