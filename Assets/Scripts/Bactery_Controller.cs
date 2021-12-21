using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bactery_Controller : MonoBehaviour
{
    [SerializeField]
    private float maxHealth, knockbackSpeedX, knockbackSpeedY, knockbackDuration, knockbackDeathSpeedX, knockbackDeathSpeedY, deathTorque;
    [SerializeField]
    private bool applyKnockback;

    private float currentHealth, knockbackStart;

    private int playerFacingDirection;

    private bool playerOnLeft, knockback;

    private PlayerMov pc;
    private GameObject aliveGO, brokenTopGO, brokenBotGO;
    private Rigidbody2D bodyAlive, bodyBrokenTop, bodyBrokenBot;
    private Animator aliveAnim;

    private void Start()
    {
        currentHealth = maxHealth;

        pc = GameObject.Find("Player").GetComponent<PlayerMov>();

        aliveGO = transform.Find("Alive").gameObject;
        brokenTopGO = transform.Find("Broken Top").gameObject;
        brokenBotGO = transform.Find("Broken Bottom").gameObject;

        aliveAnim = aliveGO.GetComponent<Animator>();
        bodyAlive = aliveGO.GetComponent<Rigidbody2D>();
        bodyBrokenTop = aliveGO.GetComponent<Rigidbody2D>();
        bodyBrokenBot = aliveGO.GetComponent<Rigidbody2D>();

        aliveGO.SetActive(true);
        brokenTopGO.SetActive(true);
        brokenBotGO.SetActive(true);

    }

    private void Update()
    {
        CheckKnockback();
    }

    private void Damage(float amount)
    {
        currentHealth -= amount;
        playerFacingDirection = pc.GetFacingDirection();

        if (playerFacingDirection == 1)
        {
            playerOnLeft = true;
        } else
        {
            playerOnLeft = false;
        }

        aliveAnim.SetBool("playerOnLeft", playerOnLeft);
        aliveAnim.SetTrigger("damage");

        if(applyKnockback && currentHealth >0.0f)
        {
            Knockback();
        }
        if (currentHealth <= 0.0f)
        {
            Die();
        }
    }

    private void Knockback()
    {
        knockback = true;
        knockbackStart = Time.time;
        bodyAlive.velocity = new Vector2(knockbackSpeedX * playerFacingDirection, knockbackSpeedY);
    }

    private void CheckKnockback()
    {
        if (Time.time >= knockbackStart && knockback)
        {
            knockback = false;
            bodyAlive.velocity = new Vector2(0.0f, bodyAlive.velocity.y);
        }
    }

    private void Die ()
    {
        aliveGO.SetActive(false);
        brokenTopGO.SetActive(true);
        brokenBotGO.SetActive(true);

        brokenTopGO.transform.position = aliveGO.transform.position;
        brokenBotGO.transform.position = aliveGO.transform.position;

        bodyBrokenBot.velocity = new Vector2(knockbackSpeedX * playerFacingDirection, knockbackDeathSpeedY);
        bodyBrokenTop.velocity = new Vector2(knockbackDeathSpeedX * playerFacingDirection, knockbackDeathSpeedY);
        bodyBrokenTop.AddTorque(deathTorque * -playerFacingDirection, ForceMode2D.Impulse);
        
    }
}
