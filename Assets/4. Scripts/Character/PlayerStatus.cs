using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("Movement Effectos")]
    [SerializeField]
    private ParticleSystem speedBuffPS;
    [SerializeField]
    private ParticleSystem slowDebuffPS;

    private PlayerMovement playerMovement;
    private Coroutine movementEffectorCoroutine;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void MovementEffector(float speedModifier, float slowDuration)
    {
        if (movementEffectorCoroutine != null) return;
        movementEffectorCoroutine = StartCoroutine(MovementEffectorCoroutine(speedModifier, slowDuration));
    }

    private IEnumerator MovementEffectorCoroutine(float speedModifier, float effectorDuration)
    {
        ParticleSystem effectorPS = speedModifier > 1 ? speedBuffPS : slowDebuffPS;
        effectorPS.Play();
        playerMovement.SetSpeedModifier(speedModifier);
        yield return new WaitForSeconds(effectorDuration);
        effectorPS.Stop();
        playerMovement.SetSpeedModifier(1);
        movementEffectorCoroutine = null;
    }
}
