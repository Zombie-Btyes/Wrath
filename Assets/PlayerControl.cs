using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PlayerControl : MonoBehaviour
{
    // Movement variables
    float currentSpeed = 50;
    float rotateSpeed = 40;

    // Damage and health variables
    public float PlayerDamage = 0;
    float PlayerHealth= 3;
    public Image healthBar;

    // Animation and combo variables
    Animator animator;
    int comboStep;
    float comboResetTime = 1.0f; // Time window to continue the combo
    float lastAttackTime;

    // Score variable
    public static int PlayerScore = 0;

    // Enum for attack states
    public enum AttackState
    {
        None,       // No attack is happening
        FirstAttack, // First attack in the combo
        SecondAttack // Second attack in the combo
    }

    private AttackState currentAttackState = AttackState.None; // Tracks the current attack state

    void Start()
    {
        animator = GetComponent<Animator>();
        comboStep = 0;
    }

    void Update()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        // Rotate tank based on horizontal input
        transform.Rotate(0, hor * Time.deltaTime * rotateSpeed, 0);

        // Move tank forward/backward based on vertical input
        currentSpeed = ver * 50;
        transform.Translate(0, 0, currentSpeed * Time.deltaTime);

        // Check if there's vertical input
        if (Mathf.Abs(ver) > 0.1f)
        {
            // Check if the "Run" input is pressed (keyboard or controller)
            if (Input.GetButton("Run")) // Use the input defined in the Input Manager
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", true);
                currentSpeed = 100; // Increase speed for running
            }
            else
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
                currentSpeed = 15; // Normal speed for walking
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            currentSpeed = 0;
        }
        bool isDefending = Input.GetButton("Defend"); // Check if the "Defend" input is pressed (keyboard or controller)
        animator.SetBool("isDefending", isDefending); // Update the Animator parameter
        if (isDefending)
        {
            currentSpeed *= 0.5f; // Stop moving when defending
        }

        // Handle attack input
        if (Input.GetButtonDown("Attack")) // Use the input defined in the Input Manager
        {
            PerformComboAttack();
        }

        // Reset combo if the time window has passed
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
            animator.SetInteger("ComboStep", comboStep);
            Debug.Log("Combo Reset"); // Debugging
        }

        // When the player gets hit
        animator.SetTrigger("GetHit");
    }

    void PerformComboAttack()
    {
        lastAttackTime = Time.time;

        comboStep++;
        if (comboStep > 2)
        {
            comboStep = 1; // Loop back to the first attack
        }

        animator.SetInteger("ComboStep", comboStep); // Update the Animator parameter
        Debug.Log("ComboStep: " + comboStep); // Debugging
    }


    void OnCollisionEnter(Collision hitobj)
    {
        if (hitobj.transform.tag == "bullet")
        {
            PlayerDamage = PlayerDamage + 1; // Increase the damage when tank gets hit by a bullet

            // Update health bar
            healthBar.fillAmount = 1 - (PlayerDamage / PlayerHealth);

            // Check if the tank is destroyed
            if (PlayerDamage >= PlayerHealth)
            {
                Destroy(transform.gameObject); // Destroy the tank when max damage is reached
            }
        }
    }
}