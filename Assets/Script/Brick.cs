using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour {

    private AudioSource source;
    private SpriteRenderer sr;
    private BoxCollider2D boxCollider;

    public AudioClip breakSound;
    public int Hitpoints = 1;
    public ParticleSystem DestroyEffect;
    public static event Action<Brick> OnBrickDestruction;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        this.sr = this.GetComponent<SpriteRenderer>();
        this.boxCollider = this.GetComponent<BoxCollider2D>();

        Ball.OnLightningBallEnable += OnLightningBallEnable;
        Ball.OnLightningBallDisable += OnLightningBallDisable;
    }

    private void OnLightningBallDisable(Ball obj)
    {
        if (this != null)
        {
            this.boxCollider.isTrigger = false;
        }
    }

    private void OnLightningBallEnable(Ball obj)
    {
        if (this != null)
        {
            this.boxCollider.isTrigger = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        bool instantKill = false;

        if (collision.collider.tag == "Ball")
        {
            Ball ball = collision.gameObject.GetComponent<Ball>();
            instantKill = ball.isLightningBall;
        }

        if (collision.collider.tag == "Ball" || collision.collider.tag == "Bullet")
        {
            this.TakeDamage(instantKill);
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool instantKill = false;

        if (collision.tag == "Ball")
        {
            Ball ball = collision.gameObject.GetComponent<Ball>();
            instantKill = ball.isLightningBall;
        }

        if (collision.tag == "Ball" || collision.tag == "Bullet")
        {
            this.TakeDamage(instantKill);
        }
    }

    private void TakeDamage(bool instantKill)
    {
        this.Hitpoints--;

        if (this.Hitpoints <= 0 || instantKill)
        {
            BricksManager.Instance.RemainingBricks.Remove(this);
            OnBrickDestruction?.Invoke(this);
            OnBrickDestroy(); 
            SpawnDestroyEffect();
            Destroy(this.gameObject);
        }
        else
        {
            this.sr.sprite = BricksManager.Instance.Sprites[this.Hitpoints - 1];
            source.PlayOneShot(breakSound, 1F);
        }
    }

    private void OnBrickDestroy()
    {
        float buffSpawnChance = UnityEngine.Random.Range(0, 100f);
        float DebuffSpawnChance = UnityEngine.Random.Range(0, 100f);

        bool alreadySpawned = false;

        if (buffSpawnChance <= CollectablesManager.Instance.BuffChance)
        {
            alreadySpawned = true;

            Collectable newBuff = this.SpawnCollectable(true);
        }

        if (DebuffSpawnChance <= CollectablesManager.Instance.BuffChance && !alreadySpawned)
        {
            Collectable newBuff = this.SpawnCollectable(false);
        }
    }

    private Collectable SpawnCollectable(bool isBuff)
    {
        List <Collectable> collection;

        if (isBuff)
        {
            collection = CollectablesManager.Instance.AvailableBuffs;
        }
        else
        {
            collection = CollectablesManager.Instance.AvailableDebuffs;
        }

        //Debug.Log(collection.Count);

        int buffIndex = UnityEngine.Random.Range(0, collection.Count);

        Collectable prefab = collection[buffIndex];
        Collectable newCollectable = Instantiate(prefab, this.transform.position, Quaternion.identity) as Collectable;

        return newCollectable;
    }

    private void SpawnDestroyEffect()
    {
        Vector3 brickPosition = gameObject.transform.position;
        Vector3 spawnPosition = new Vector3(brickPosition.x, brickPosition.y, brickPosition.z - 0.2f);
        GameObject effect = Instantiate(DestroyEffect.gameObject, spawnPosition, Quaternion.identity);

        ParticleSystem.MainModule mm = effect.GetComponent<ParticleSystem>().main;
        mm.startColor = this.sr.color;
        Destroy(effect, DestroyEffect.main.startLifetime.constant);
    }

    public void Init(Transform containerTransform, Sprite sprite, int hitpoints)
    {
        this.transform.SetParent(containerTransform);
        this.sr.sprite = sprite;
        this.Hitpoints = hitpoints;
    }

    private void OnDisable()
    {
        Ball.OnLightningBallEnable -= OnLightningBallEnable;
        Ball.OnLightningBallDisable -= OnLightningBallDisable;
    }
}