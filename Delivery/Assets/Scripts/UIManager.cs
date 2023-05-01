using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Transform winText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        GameManager.OnWin += OnWin;
    }

    private void OnDisable()
    {
        GameManager.OnWin -= OnWin;
    }

    void OnWin()
    {
        winText.DOScale(Vector3.one, 1).SetLoops(2, LoopType.Yoyo);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
