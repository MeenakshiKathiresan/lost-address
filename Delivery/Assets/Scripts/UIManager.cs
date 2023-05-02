using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Transform winText;

    [SerializeField]
    Transform startScreen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    void OnGameStateChange(GameManager.GameState gameState)
    {

        if (gameState == GameManager.GameState.GameOver)
            winText.DOScale(Vector3.one, 1).SetLoops(2, LoopType.Yoyo).OnComplete(() => {
                startScreen.gameObject.SetActive(true);
            });

        else if (gameState == GameManager.GameState.InGame)
            startScreen.gameObject.SetActive(false);
        else
            startScreen.gameObject.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
