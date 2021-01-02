using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    public static Paddle Instance { get; private set; }

    public bool PaddleIsTransforming { get; set; }
    public bool PaddleIsShooting { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private float paddleInitialY;
    private float defaultPaddleWidthInPixels = 100;
    private float defaultleftClamp = 50;
    private float defaultrightClamp = 270;

    private Camera mainCamera;
    private SpriteRenderer sr;

    public float extendShrinkDuration = 10;
    public float paddleWidth = 1.0f;
    public float paddleHeight = 0.28f;

    public BoxCollider2D boxCol;

    public GameObject leftMuzzle;
    public GameObject rightMuzzle;

    public Bullet bulletPrefab;

    void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        paddleInitialY = this.transform.position.y;

        sr = GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        PaddleMovement();
        UpdateMuzzlePosition();
    }

    private void UpdateMuzzlePosition()
    {
        leftMuzzle.transform.position = new Vector3(this.transform.position.x - (this.sr.size.x / 3) + 0.1f, this.transform.position.y + 0.1f, this.transform.position.z);
        rightMuzzle.transform.position = new Vector3(this.transform.position.x + (this.sr.size.x / 3) - 0.1f, this.transform.position.y + 0.1f, this.transform.position.z);
    }

    public void StartWidthAnimation(float newWidth)
    {
        StartCoroutine(AnimatePaddleWidth(newWidth));
    }

    public IEnumerator AnimatePaddleWidth(float width)
    {
        this.PaddleIsTransforming = true;
        this.StartCoroutine(ResetPaddleWidthAfterTime(this.extendShrinkDuration));

        float currentWidth;

        if (width > this.sr.size.x)
        {
            currentWidth = this.sr.size.x;

            while (currentWidth < width)
            {
                currentWidth += Time.deltaTime;

                this.sr.size = new Vector2(currentWidth, paddleHeight);
                boxCol.size = new Vector2(currentWidth, paddleHeight);

                yield return null;
            }
        }
        else
        {
            currentWidth = this.sr.size.x;

            while (currentWidth > width)
            {
                currentWidth -= Time.deltaTime;

                this.sr.size = new Vector2(currentWidth, paddleHeight);
                boxCol.size = new Vector2(currentWidth, paddleHeight);

                yield return null;
            }
        }

        this.PaddleIsTransforming = false;
    }

    private IEnumerator ResetPaddleWidthAfterTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        this.StartWidthAnimation(this.paddleWidth);
    }

    private void PaddleMovement()
    {
        float paddleShift = (defaultPaddleWidthInPixels - ((defaultPaddleWidthInPixels / 2) * this.sr.size.x)) / 2;
        float leftClamp = defaultleftClamp - paddleShift;
        float rightClamp = defaultrightClamp + paddleShift;
        float mousePositionPixels = Mathf.Clamp(Input.mousePosition.x, leftClamp, rightClamp);
        float mousePositionWorldX = mainCamera.ScreenToWorldPoint(new Vector3(mousePositionPixels, 0, 0)).x;

        this.transform.position = new Vector3(mousePositionWorldX, paddleInitialY, 0);
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Ball")
        {
            Rigidbody2D ballRb = coll.gameObject.GetComponent<Rigidbody2D>();

            Vector3 hitPoint = coll.contacts[0].point;
            Vector3 paddleCenter = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y);

            ballRb.velocity = Vector2.zero;

            float difference = paddleCenter.x - hitPoint.x;

            if (hitPoint.x < paddleCenter.x)
            {
                ballRb.AddForce(new Vector2(-(Mathf.Abs(difference * 200)), BallsManager.Instance.initialBallSpeed));
            }
            else
            {
                ballRb.AddForce(new Vector2(Mathf.Abs(difference * 200), BallsManager.Instance.initialBallSpeed));
            }
        }
    }

    public void StartShooting()
    {
        if (!this.PaddleIsShooting)
        {
            this.PaddleIsShooting = true;

            StartCoroutine(StartShootingRoutine());
        }
    }

    public IEnumerator StartShootingRoutine()
    {
        float fireCoolDown = .3f;
        float fireCoolDownLeft = 0;

        float shootingDuration = 10;
        float shootingDurationLeft = shootingDuration;

        //Debug.Log("Start shooting");

        while (shootingDurationLeft >= 0)
        {
            fireCoolDownLeft -= Time.deltaTime;
            shootingDurationLeft -= Time.deltaTime;

            if (fireCoolDownLeft <= 0)
            {
                this.Shoot();
                fireCoolDownLeft = fireCoolDown;

                //Debug.Log($"Shoot at {Time.time}");
            }

            yield return null;
        }

        //Debug.Log("Stop shooting");

        this.PaddleIsShooting = false;

        leftMuzzle.SetActive(false);
        rightMuzzle.SetActive(false);
    }

    private void Shoot()
    {
        leftMuzzle.SetActive(false);
        rightMuzzle.SetActive(false);

        leftMuzzle.SetActive(true);
        rightMuzzle.SetActive(true);

        this.SpawnBullet(leftMuzzle);
        this.SpawnBullet(rightMuzzle);
    }

    private void SpawnBullet(GameObject muzzle)
    {
        Vector3 spawnPosition = new Vector3(muzzle.transform.position.x, muzzle.transform.position.y + 0.2f, muzzle.transform.position.z);
        Bullet bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddForce(new Vector2(0, 225f));
    }
}