using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    Animator animator;
    float currentSpeed = 5; // Speed for random movement
    float attackRange = 500; // Increased attack range (adjust as needed)
    float detectionRange = 20; // Distance at which the zombie detects the player
    int timer = 0;
    int Maxtimer;

    public float xmin = 100;
    public float xmax = 900;
    public float ymin = 0f;
    public float ymax = 200;
    public float zmin = 100;
    public float zmax = 900;
    Vector3 MovePoint;

    private bool isChasing = false; // Whether the zombie is chasing the player
    private Transform player; // Reference to the player's transform
    private bool isDead = false; // Track if the zombie is dead

    public int health = 100; // Zombie's health

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player by tag
        MovePoint = GetRandomPositionWithinBounds(); // Set initial random destination within bounds
        Maxtimer = Random.Range(200, 2000);
    }

    void Update()
    {
        if (isDead) return; // Stop all logic if the zombie is dead

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Update timer and set new MovePoint if timer exceeds Maxtimer
        if (timer > Maxtimer)
        {
            MovePoint = GetRandomPositionWithinBounds(); // Get a new random position within bounds
            timer = 0;
            Maxtimer = Random.Range(200, 2000);
        }
        timer++;

        // Move toward the MovePoint
        transform.position = Vector3.MoveTowards(transform.position, MovePoint, currentSpeed * Time.deltaTime);

        // Rotate toward the MovePoint
        Vector3 direction = (MovePoint - transform.position).normalized;
        if (direction != Vector3.zero) // Ensure direction is valid
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        }

        // Clamp the zombie's position to stay within the specified range
        ClampPosition();

        // Check if the zombie should chase the player
        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
            if (distanceToPlayer <= attackRange)
            {
                AttackPlayer();
            }
        }

        // Update animations
        animator.SetBool("isWalking", Vector3.Distance(transform.position, MovePoint) > 1);
    }

    void ChasePlayer()
    {
        // Move toward the player
        transform.position = Vector3.MoveTowards(transform.position, player.position, currentSpeed * Time.deltaTime);

        // Rotate toward the player
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero) // Ensure direction is valid
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10);
        }
    }

    void AttackPlayer()
    {
        // Stop moving and attack
        currentSpeed = 0;
        animator.SetTrigger("Attack"); // Trigger the attack animation

        // Resume movement after attack animation finishes
        StartCoroutine(ResumeMovementAfterAttack());
    }

    IEnumerator ResumeMovementAfterAttack()
    {
        // Wait for the attack animation to finish
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Reset speed after attack
        currentSpeed = 5;
    }

    Vector3 GetRandomPositionWithinBounds()
    {
        // Generate a random position within the specified x and z bounds
        float randomX = Random.Range(xmin, xmax);
        float randomZ = Random.Range(zmin, zmax);
        return new Vector3(randomX, transform.position.y, randomZ);
    }

    void ClampPosition()
    {
        // Clamp the zombie's position to stay within the specified x and z bounds
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, xmin, xmax);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, zmin, zmax);
        transform.position = clampedPosition;
    }

    // Call this method to trigger the death animation and destroy the zombie
    public void Die()
    {
        if (isDead) return; // Prevent multiple calls

        isDead = true; // Mark the zombie as dead
        animator.SetTrigger("Death"); // Trigger the death animation
        StartCoroutine(DestroyAfterDeath()); // Destroy the object after 3 seconds
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(3); // Wait for 3 seconds
        Destroy(gameObject); // Destroy the zombie
    }

    // Call this method to apply damage to the zombie
    public void TakeDamage(int damage)
    {
        if (isDead) return; // Ignore damage if already dead

        health -= damage; // Reduce health
        animator.SetTrigger("TakeDamage"); // Trigger the damage animation

        if (health <= 0)
        {
            Die(); // Trigger death if health reaches 0
        }
    }
}