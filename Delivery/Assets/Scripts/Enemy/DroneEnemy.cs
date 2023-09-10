using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class DroneEnemy : Enemy
{

    [SerializeField]
    float damageDistance = 2f;

    [SerializeField]
    float randomMovementInterval = 0.25f;

    [SerializeField]
    public float maxFollowDistance = 8f;


    [SerializeField]
    float bounceOnBoundsDistance = 2f;

    [SerializeField]
    float startStallTime = 1;

    [SerializeField]
    ParticleSystem onAttack;

    [SerializeField]
    Transform spriteTransform;

    [SerializeField]
    float heightOffset = 2f;


    SpriteRenderer[] sprites;

    Collider2D collider;

    bool inSameFloor;

    float distanceToPlayer;

    int spriteOrder = 2;


    Vector2 startPos;
    private void Start()
    {

    }


    protected override void OnEnable()
    {

        base.OnEnable();

        startPos = transform.position;
        startPos.y = transform.position.y + heightOffset;

        healthBar.parent.gameObject.SetActive(false);

        // incase it died in a diff position
        spriteTransform.localPosition = Vector2.zero;

        collider = GetComponent<Collider2D>();
        collider.enabled = false;

        sprites = GetComponentsInChildren<SpriteRenderer>();

        SetSpritesSortingOrder(2);
        // first 1 sec is stalling time, use 0.5sec of that to move up
        StartCoroutine(MovePlayerUp());

        Invoke("ShakeEnemy", 1);


    }

    protected override void OnDisable()
    {
        base.OnDisable();

        spriteTransform.DOKill(false);
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.instance.GetCurrentState() == GameManager.GameState.InGame)
        {
            int playerFloor = GameManager.instance.player.GetCurrentFloor();
            inSameFloor = CurrentFloor == playerFloor;

            // start moving after initial stall time
            if (Time.time - startTime > startStallTime)
            {
                if (inSameFloor)
                {
                    Vector2 playerPosition = GameManager.instance.player.GetPlayerPosition();
                    distanceToPlayer = Mathf.Abs(transform.position.x - playerPosition.x);



                    int direction = transform.position.x < playerPosition.x ? 1 : -1;

                    if (distanceToPlayer > damageDistance)
                    {
                        transform.Translate(Vector2.right * direction * enemySpeed * Time.deltaTime);
                    }

                    HandleFiring();

                }
                else
                {
                    if (Vector2.Distance(transform.position, startPos) > 0.1f)
                    {
                        Vector2 moveDirection = (startPos - (Vector2)transform.position).normalized;
                        transform.Translate(moveDirection * enemySpeed * Time.deltaTime);
                    }

                }
                
            }
        }
    }

    void HandleFiring()
    {
        if (Time.time - lastShot > shootInterval && inSameFloor)
        {
            lastShot = Time.time;

            if (distanceToPlayer < damageDistance)
            {     // TODO Get player with collider overlap circle
                GameManager.instance.player.TakeDamage(damage);
                onAttack.Play();

                Vector2 direction = (GameManager.instance.player.transform.position - spriteTransform.transform.position).normalized * 1.5f;

                spriteTransform.DOLocalMove(direction, 0.2f).SetLoops(2, LoopType.Yoyo);

            }
        }

        if (!collider.enabled)
        {
            collider.enabled = true;
        }
    }

    void ShakeEnemy()
    {
        spriteTransform.DOShakePosition(1, 0.4f, 0, 90).SetLoops(-1);
    }


    IEnumerator MovePlayerUp()
    {
        yield return new WaitForSeconds(0.4f);

        SetSpritesSortingOrder(5);

        healthBar.parent.gameObject.SetActive(true);
        transform.DOMoveY(transform.position.y + heightOffset, 0.3f);
    }


    void SetSpritesSortingOrder(int order)
    {
        spriteOrder = order;
        sprites[0].sortingOrder = order;

        //for (int i = 0; i < sprites.Length; i++)
        //{
        //    sprites[i].sortingOrder = order;
        //}
    }
}










