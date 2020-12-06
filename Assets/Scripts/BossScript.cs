using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossScript : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    public int maxHealth = 5;
    int currentHealth;

    public AudioClip hitSound;

    public AudioSource audioSource;
    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;
    bool broken = true;

    public Text damageText;
    
    Animator animator;
    
    private RubyController controller; 
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        damageText.text = "";
        GameObject controllerObject = GameObject.FindWithTag("RubyController"); //this line of code finds the RubyController script by looking for a "RubyController" tag on Ruby

        if (controllerObject != null)

        {

            controller = controllerObject.GetComponent<RubyController>(); //and this line of code finds the rubyController and then stores it in a variable

        }
    }

    void Update()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if(!broken)
        {
            return;
        }
        
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }
    
    void FixedUpdate()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if(broken == false)
        {
            return;
        }
        
        Vector2 position = rigidbody2D.position;
        
        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }
        
        rigidbody2D.MovePosition(position);
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-4);
        }
    }
    
   //Public because we want to call it from elsewhere like the projectile script
    public void Fix()
    {
        broken = false;
        rigidbody2D.simulated = false;
        //optional if you added the fixed animation
        animator.SetTrigger("Fixed");
        //smokeEffect.Stop();

         if (controller != null)
        {
                controller.BossDefeated(true); //this line of code is increasing Ruby's health by 1!
        }
    }
     public void PlaySound(AudioClip clip, float volumeScale)
    {
        audioSource.PlayOneShot(clip, volumeScale);
    }

    public void damage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (amount < 0)
        {
            //healthDecrease = Instantiate(healthDecrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound, 0.3f);
            damageText.text = currentHealth.ToString();
        }
        if (currentHealth <= 0)
        {
            damageText.text = "";
            Fix();
        }
    }
}