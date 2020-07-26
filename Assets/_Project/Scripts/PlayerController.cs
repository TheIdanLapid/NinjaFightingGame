using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Events;

[System.Serializable]
public class Combo
{
    public string title;
    public float duration = 10;
    public List<int> moves = new List<int>();
    public UnityEvent onComplete = new UnityEvent();
}

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Controller Info")]
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float jumpHeight = 5;
    [SerializeField] LayerMask enemyLayer = default;
    [SerializeField] AudioSource sound;

    [Header("Combo Info")]
    [SerializeField] float comboTime = 0.9f;
    [SerializeField] Combo[] comboList = default;


    public FloatEvent onComboCharge = new FloatEvent();

    // References
    CharacterController controller;
    Animator animator;

    // Punch hitbox
    Vector3 punchBoxOffset = new Vector3(1, 1, 0);
    Vector3 punchBoxSize = new Vector3(1, 1, 0.5f);

    float currentComboCharge = 0;
    float currentComboTime = 0;
    List<int> currentCombo = new List<int>();
    Vector3 playerVelocity = Vector3.zero;
    bool groundedPlayer = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        animator.SetFloat("Walking", Mathf.Abs(playerVelocity.x));

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        playerVelocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        currentComboTime -= Time.deltaTime;
        if (currentComboTime <= 0)
            currentCombo.Clear();
    }
    void OnMove(InputValue value)
    {
        playerVelocity.x = value.Get<Vector2>().x * moveSpeed;
        // Jump - When clicking "Up" key, and grounded
        if (groundedPlayer)
            playerVelocity.y += value.Get<Vector2>().y * Mathf.Sqrt(jumpHeight * -3.0f * Physics.gravity.y);
    }
    void OnJump(InputValue value)
    {
        comboList[0].onComplete.Invoke();
        currentCombo.Clear();
    }

    void OnPunch(InputValue value)
    {
        //TODO: add punch animation here
        //CheckHit();
        //currentComboTime = comboTime;
        //CheckCombo(0);
    }
    void OnKick(InputValue value)
    {
        //TODO: add kick animation here
        //CheckHit();
        //currentComboTime = comboTime;
        //CheckCombo(1);
    }

    void CheckCombo(int move)
    {
        if (currentComboCharge < 1)
            return;

        currentCombo.Add(move);
        foreach (Combo combo in comboList)
        {
            if (currentCombo.SequenceEqual(combo.moves))
            {
                combo.onComplete.Invoke();
                currentCombo.Clear();
                break;
            }
        }
    }

    void CheckHit()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position + punchBoxOffset, punchBoxSize, Quaternion.identity, enemyLayer);
        foreach (Collider collider in hitColliders)
            if (collider.TryGetComponent<Damageable>(out Damageable damageable))
            {
                currentComboCharge += 0.1f;
                onComboCharge.Invoke(currentComboCharge / 1);
                damageable.Damage(5);
                break;
            }
    }

    public void SuperHit()
    {
        sound.Play();
        if (Physics.SphereCast(transform.position, 2, Vector3.right, out RaycastHit info, 10, enemyLayer))
        {
            if (info.transform.TryGetComponent<Damageable>(out Damageable damageable))
            {
                StartCoroutine(Attack(damageable));
            }
        }

        IEnumerator Attack(Damageable damageable)
        {
            for (int i = 0; i < comboList[0].duration; i++)
            {
                damageable.Damage(3);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position + punchBoxOffset, punchBoxSize);
    }

}
