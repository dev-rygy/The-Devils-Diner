using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Effector : MonoBehaviour
{
    private static string ANIMATION_ACTIVATE = "Activate";
    private static string ANIMATION_DEACTIVATE = "Deactivate";
    private static string ANIMATION_RESET = "Reset";

    private static Vector3 POINT_UP = new Vector3(0, 0, 180);

    [Header("Settings")]
    [SerializeField]
    private bool canBurnFood = true;
    [SerializeField]
    private float spawnTime = 1f;
    [SerializeField]
    private float indicatorTime = 2f;
    [SerializeField]
    private float activeTime = 2f;
    [SerializeField]
    private float speedModifier = 0.5f;
    [SerializeField]
    private float initialScaleMult = 1f;
    [SerializeField]
    private float scaleDuration = 1f;
    [SerializeField]
    private float buffDuration = 2;
    [SerializeField]
    private Vector3 projectileEndPointOffset = new Vector3(0, 5);

    [Header("Sfx")]
    [SerializeField]
    private AudioClip spawnSfx;
    [SerializeField]
    private AudioClip activatedSfx;
    [SerializeField]
    private AudioClip hitSfx;

    [Header("Required Components")]
    [SerializeField]
    private FoodData burnedFood;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;    
    [SerializeField]
    private GameObject projectilePrefab;

    private Transform projectileTransform;
    private SortingGroup sortingGroup;

    private Vector3 projectieSpawnPoint;
    private Vector3 projectileEndPoint;

    private bool isActivaited;

    private void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isActivaited) return;

        if (collision.TryGetComponent(out PlayerStatus playerStatus))
        {
            SoundManager.main.PlayOneShot(hitSfx);
            playerStatus.MovementEffector(speedModifier, buffDuration); 
        }

        if (canBurnFood && collision.TryGetComponent(out PlayerInteraction playerInteraction))
        {
            if (playerInteraction.CurrentDish != null && playerInteraction.CurrentDish.CurrentFood != null)
                playerInteraction.CurrentDish.CurrentFood = burnedFood;
        }       
    }

    public void Initialize(Vector3 spawnPoint)
    {
        projectieSpawnPoint = spawnPoint;
        projectileEndPoint = spawnPoint + projectileEndPointOffset;
        
        projectileTransform = PoolManager.main.Spawn(projectilePrefab).transform;
        projectileTransform.position = projectieSpawnPoint;

        StartCoroutine(BuffCoroutine());
    }

    private IEnumerator BuffCoroutine()
    {
        // Shoot upward
        sortingGroup.sortingOrder = -1;
        SoundManager.main.PlayOneShot(spawnSfx);
        projectileTransform.eulerAngles = POINT_UP;
        yield return StartCoroutine(ShootUp());

        // Fall down
        projectileTransform.eulerAngles = Vector3.zero;        
        yield return StartCoroutine(FallDown());
        PoolManager.main.Despawn(projectileTransform.gameObject);

        // Activated
        sortingGroup.sortingOrder = 0;
        SoundManager.main.PlayOneShot(activatedSfx);
        isActivaited = true;
        animator.SetTrigger(ANIMATION_ACTIVATE);
        yield return new WaitForSeconds(activeTime);

        // Deactivated
        animator.SetTrigger(ANIMATION_DEACTIVATE);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Reset
        animator.SetTrigger(ANIMATION_RESET);
        isActivaited = false;        
        PoolManager.main.Despawn(gameObject);
    }

    private IEnumerator ShootUp()
    {
        var start = Time.time;
        var t = 0f;

        StartCoroutine(ScaleOverTime());    // Scales the projectile over a set amount of time during the shoot up

        while (t < 1)
        {
            t = (Time.time - start) / spawnTime;
            projectileTransform.position = Vector3.Lerp(projectieSpawnPoint, projectileEndPoint, t);

            yield return null;
        }

        projectileTransform.position = projectileEndPoint;
    }

    private IEnumerator ScaleOverTime()     // Scale projectiles over a set amount of time
    {
        Vector3 initScale = Vector3.one * initialScaleMult;
        Vector3 targetScale = Vector3.one;
        float elapsedTime = 0f;

        while (elapsedTime < scaleDuration)
        {
            projectileTransform.localScale = Vector3.Lerp(initScale, targetScale, elapsedTime / scaleDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        projectileTransform.localScale = targetScale;
    }

    private IEnumerator FallDown()
    {
        var startPoint = new Vector3(transform.position.x, projectileEndPoint.y);
        var endPoint = transform.position;
        var start = Time.time;
        var t = 0f;

        while (t < 1)
        {
            t = (Time.time - start) / indicatorTime;
            projectileTransform.position = Vector3.Lerp(startPoint, endPoint, t);

            yield return null;
        }

        projectileTransform.position = endPoint;
    }
}
