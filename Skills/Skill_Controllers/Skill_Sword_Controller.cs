using System.Collections.Generic;
using UnityEngine;

public class Skill_Sword_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRotate = true;
    private bool isReturning;

    private float freezeTimeDuration;
    private float returnSpeed = 30; // Speed at which the sword returns to the player

    [Header("Pierce Info")]
    private float piercedTargets;

    [Header("Ricochet Info")]
    private float ricochetSpeed;
    private bool isRicocheting;
    private int numberOfRicochets;
    private List<Transform> enemyTarget;
    private int targetIndex;

    [Header("Spin Info")]
    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;

    private float hitTimer;
    private float hitCooldown;

    // Perpetual motion bug
    //private float spinDirection;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

    public void SetupSword(Vector2 _dir, float _gravityScale, Player _player, float _freezeTimeDuration, float _returnSpeed)
    {
        player = _player;
        freezeTimeDuration = _freezeTimeDuration;
        returnSpeed = _returnSpeed;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;

        if (piercedTargets <= 0) // Sets the Animation on Pierce to idle rather than spin
            anim.SetBool("Rotation", true);

        // Perpetual motion bug
        //spinDirection = Mathf.Clamp(rb.velocity.x, -1, 1);

        Invoke("DestroyMe", 7.0f);
    }

    public void SetupRicochet(bool _isRicocheting, int _numberOfRicochets, float _ricochetSpeed)
    {
        isRicocheting = _isRicocheting;
        numberOfRicochets = _numberOfRicochets;
        ricochetSpeed = _ricochetSpeed;

        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _piercedTargets)
    {
        piercedTargets = _piercedTargets;
    }
    
    public void SetupSpin(bool _isSpinning, float _maxTravelDistance, float _spinDuration, float _hitCooldown)
    {
        isSpinning = _isSpinning;
        maxTravelDistance = _maxTravelDistance;
        spinDuration = _spinDuration;
        hitCooldown = _hitCooldown;
    }

    public void ReturnSword()
    {
        if (rb == null)
            Debug.Log("Rigidbody2D on sword is null!!");
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        // rb.isKinematic = false;
        transform.parent = null; // Make the sword a child of the Player parent
        isReturning = true;
    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity; // Sword arcs on a given trajectory path

        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 1)
                player.CatchTheSword();
        }

        RicochetLogic();
        SpinLogic();
    }

    private void SpinLogic()
    {
        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position, transform.position) > maxTravelDistance && !wasStopped)
            {
                StopWhenSpinning();
            }

            if (wasStopped)
            {
                spinTimer -= Time.deltaTime;

                // Perpetual motion bug
                //transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + spinDirection, transform.position.y), 1.5f * Time.deltaTime);

                if (spinTimer < 0)
                {
                    isReturning = true;
                    isSpinning = false;
                }

                hitTimer -= Time.deltaTime;

                if (hitTimer < 0)
                {
                    hitTimer = hitCooldown;

                    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1); // Check for circle colliders in a radius of 10

                    foreach (var hit in colliders)
                    {
                        if (hit.GetComponent<Enemy>() != null)
                            SwordSkillDamage(hit.GetComponent<Enemy>());
                    }
                }

            }
        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition; // Animation continues but x and y are frozen
        spinTimer = spinDuration;
    }

    private void RicochetLogic()
    {
        if (isRicocheting && enemyTarget.Count > 0) // If ricocheting and target count is not 0
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, ricochetSpeed * Time.deltaTime); // Move the sword to the position of the next enemy in the list multiplied by the ricochetSpeed

            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < 0.1f)
            {
                SwordSkillDamage(enemyTarget[targetIndex].GetComponent<Enemy>());

                targetIndex++; // Iterate to next target in list
                numberOfRicochets--; // Decrement the number of ricochets by one

                if (numberOfRicochets <= 0)
                {
                    isRicocheting = false; // No longer bouncing
                    isReturning = true; // Return sword to player
                }

                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return; // If sword is being retrieved, exit function

        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            SwordSkillDamage(enemy);
        }

        SetupLogicForRicochet(collision);

        StuckInto(collision);
    }

    private void SwordSkillDamage(Enemy enemy)
    {
        enemy.Damage();
        enemy.StartCoroutine("FreezeTimerFor", freezeTimeDuration);
    }

    private void SetupLogicForRicochet(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null) // If an enemy exists
        {
            if (isRicocheting && enemyTarget.Count <= 0) // If ricocheting and there are no targets
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10); // Check for circle colliders in a radius of 10

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);
                }
            }
        }
    }

    private void StuckInto(Collider2D collision)
    // If pierced targets allowed is still greater than 0 and the sword collides with an ememy, decrement pierced targets allowed and escape the StuckInto function
    {

        if (piercedTargets > 0 && collision.GetComponent<Enemy>() != null)
        {
            piercedTargets--;
            return;
        }

        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }   


        canRotate = false; // Stop sword object rotation
        cd.enabled = false; // Disable CircleCollider2D

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll; // Freeze all constraints

        if (isRicocheting && enemyTarget.Count > 0)
            return;

        anim.SetBool("Rotation", false);
        transform.parent = collision.transform; // Make the sword a child of the object it collided with
    }    
}