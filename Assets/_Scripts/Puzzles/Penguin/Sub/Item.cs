using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Item : MonoBehaviour
{

    
    [Header("æ∆¿Ã≈€")]
    public Sprite _spriteMaskSet;
    public SpriteMask _spriteMask;
    public SpriteRenderer _randItemSR;
    public Sprite[] _randItemSP;
    public float _fishJump;
    public float _randItemSRDoPosYTerm;
    public float _randItemSRDoPosXTerm;
    public float _itemSclaeTerm;
    public float _itemTagTerm;
    public bool _isFish;
    public GameObject _item;
    public List<Tween> _setItemDTList = new List<Tween>();
    public Tween _inputDT;


}
