using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

[System.Serializable] public class FloatEvent : UnityEvent<float> { }
public class Damageable : MonoBehaviour
{

    public FloatEvent onDamage;
    [SerializeField] int maxHealth = 100;
    int currentHealth;
    public bool isHit;

    [Header("Polish")]
    public Rig hitRig;
    public ParticleSystem hitParticle;

    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isHit)
        {
            hitRig.weight = Mathf.Lerp(hitRig.weight, Random.Range(.8f, 1), .5f);
        }
        else
        {
            hitRig.weight = Mathf.Lerp(hitRig.weight, 0, .1f);
        }
    }

    public void Damage(int points)
    {
        currentHealth -= points;
        //TODO: Add hit animation
        onDamage.Invoke((float)currentHealth / maxHealth);

        isHit = true;
        StopAllCoroutines();
        StartCoroutine(CheckStopHit());
        StartCoroutine(RendererBlink());

        if (currentHealth <= 0)
        {
            //TODO: Add death animation? Optional
            print("Dead");
        }

        hitParticle.Play();
    }

    IEnumerator CheckStopHit()
    {
        yield return new WaitForSeconds(1);
        isHit = false;
    }

    IEnumerator RendererBlink()
    {
        yield return new WaitForSeconds(.3f);
        isHit = false;
    }
}
