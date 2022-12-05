using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerAnim : MonoBehaviour
{
    static PlayerAnim _instance;
    public static PlayerAnim xInstance { get { return _instance; } }
    [HideInInspector] public Animator _anim;
    [HideInInspector] public string _animState = "PEEPEESTATE";
    
    enum enumPeePeeState
    {
        idle = 0,
        lwork = 1,
        rwork = 2,
        jump = 3,
        hit = 4,
        fall = 5,
        escapeTry = 6
    }


    void zPeePeeState()
    {
        if (TrPuzzlePeePee.xInstance._currJumping == true)
        {
            _anim.SetInteger(_animState, (int)enumPeePeeState.jump);
            if (TrPuzzlePeePee.xInstance._currJumping == false && Player.xInstance._isFall == false)
            {
                _anim.SetInteger(_animState, (int)enumPeePeeState.idle);
            } else if (TrPuzzlePeePee.xInstance._currJumping == false && Player.xInstance._isFall == true)
            {
                _anim.SetInteger(_animState, (int)enumPeePeeState.fall);
            }

        }
        //È÷Æ®
        else if (Player.xInstance._peepeeHit == true)
        {
            _anim.SetInteger(_animState, (int)enumPeePeeState.hit);
        }
        else if (Player.xInstance._isFall == true)
        {
            _anim.SetInteger(_animState, (int)enumPeePeeState.fall);
        }
        else if (TrPuzzlePeePee.xInstance._isPeePeeEscapeTry == true)
        {
            _anim.SetInteger(_animState, (int)enumPeePeeState.escapeTry);
        }
        else if (TrPuzzlePeePee.xInstance._currJumping == true && Player.xInstance._isFall == true) 
        {
            _anim.SetInteger(_animState, (int)enumPeePeeState.fall);
        }
        else
        {
            _anim.SetInteger(_animState, (int)enumPeePeeState.idle);
        }

        
    }

    void yPeePeeEscapeTry()
    {
        if (Player.xInstance._isFall == true && Player.xInstance._jumpKey == true)
        {
            TrPuzzlePeePee.xInstance._isPeePeeEscapeTry = true;
            _anim.SetInteger(_animState, (int)enumPeePeeState.escapeTry);
        }
        else
            TrPuzzlePeePee.xInstance._isPeePeeEscapeTry = false;
    }


    void Update()
    {
        if (TrUI_PuzzleNotice.xInstance._isGameOver == false)
        {
            zPeePeeState();
        }
        yPeePeeEscapeTry();

        if (TrUI_PuzzleNotice.xInstance._isGameOver == true)
        {
            _anim.SetInteger(_animState, (int)enumPeePeeState.idle);
        }

    }
    void Awake()
    {
        _anim = GetComponent<Animator>();
        if (_instance == null)
        {
            _instance = this;
        }
    }
}
