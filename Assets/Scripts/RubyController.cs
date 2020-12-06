using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 5;
    
    public GameObject projectilePrefab;
    public GameObject bossEnemy;
    public GameObject projectilePrefab2;
    public GameObject projectilePrefab3;
    public AudioClip throwSound;
    public AudioClip secondthrowSound;
    public AudioClip hitSound;
    public AudioClip collectedClip;

    public AudioClip winMusic;
    public AudioClip loseMusic;
    public AudioClip bossMusic;
    public AudioClip backgroundMusic;

    public AudioSource audioSource2;
    
    public int health { get { return currentHealth; }}
    int currentHealth;
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    public ParticleSystem healthIncrease;
    public ParticleSystem healthDecrease;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    public AudioSource audioSource;
    
    public Text scoreText;
    private int score;

    public Text GameOverText;
    public Text RestartText;

    public Text LevelText;
    public Text ammoText;
    public Text lightammoText;

    private bool gameOver; // when its true it unlocks the ability to restart
    public static int level;
    private int ammo;
    private int lightammo;

    private bool bossdefeated;

    private int enemyCount; //keeps boss enemy from infinitely spawning
    private int soundPlaying; //keeps songs from repeating

    // Start is called before the first frame update
    void Start()
    {   
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        currentHealth = maxHealth;
        audioSource2.clip = backgroundMusic;
        audioSource2.Play();
        
        score = 0;
        gameOver = false;
        ammo = 4;
        ammoText.text = "[C] Cogs :" + ammo.ToString();
        lightammoText.text = "";
        level = level + 1;
        //LevelText.text = "Level: " + level.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        
        if(Input.GetKeyDown(KeyCode.C))
        {
            if (ammo <= 0)
            {
                return;
            }
            Launch();
        }
        if(Input.GetKeyDown(KeyCode.V))
        {
            if(lightammo <= 0)
            {
                return;
            }
            Launch2();
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    if( level == 1 && score == 4 )
                    {
                    
                        SceneManager.LoadScene("MainScene3");
                    }
                
                else
                    {
                    character.DisplayDialog();
                    }
                }
            }
        }
        if (level == 1 && score == 4)
        {
            GameOverText.text = "Congrats on Level 1! Talk to Jambi to Continue.";
            
        }
        if(level >= 2 && score == 5)
        {
            audioSource2.Stop();
            while(soundPlaying < 1)
            {
            PlaySound(bossMusic, 1.0f);
            soundPlaying +=1;
            }
            while( enemyCount < 1)
            {
            Instantiate(bossEnemy, new Vector2(14,-8), Quaternion.identity);
            Instantiate(projectilePrefab3, new Vector2(15,-12), Quaternion.identity);
            enemyCount += 1;
            }
        }
        if(level >= 2 && score == 5 && bossdefeated == true)
        {
            audioSource2.Stop();
            GameOverText.text = "You Win! Game made by Jordan Carswell";
            RestartText.text = "Press R to restart the game.";
            gameOver = true;
            while(soundPlaying < 2)
            {
            PlaySound(winMusic, 1.0f);
            soundPlaying +=1;
            }
        }
        if(currentHealth == 0)
        {
            audioSource2.Stop();
            GameOverText.text = "You Lose!";
            RestartText.text = "Press R to restart the game.";
            speed = 0.0f;
            gameOver = true;
            while( soundPlaying < 1)
            {
            PlaySound(loseMusic, 1.0f);
            soundPlaying += 1;
            }
        }
        if (Input.GetKey(KeyCode.R))

        {

            if (gameOver == true)

            {

              SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene

            }

        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            healthDecrease = Instantiate(healthDecrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound, 1.0f);
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        if (amount > 0 )
        {
            healthIncrease = Instantiate(healthIncrease, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
    }
    public void BossDefeated(bool yes)
    {
        if (yes = true)
        {
            bossdefeated = true;
        }
    }
    public void changeScore(int point)
    {
        score = score + point;
        scoreText.text = "Robots Fixed: " + score.ToString ();
    }
    
    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        ammo = ammo - 1;
        ammoText.text = "[C] Cogs: " + ammo.ToString();
        
        PlaySound(throwSound, 1.0f);
    } 
    void Launch2()
    {
        GameObject bossprojectileObject = Instantiate(projectilePrefab2, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        BossProjectile bossprojectile = bossprojectileObject.GetComponent<BossProjectile>();
        bossprojectile.Launch2(lookDirection, 100);

        animator.SetTrigger("Launch");

        lightammo = lightammo - 1;
        lightammoText.text = "[V] Light Energy: " + ammo.ToString();
        
        PlaySound(secondthrowSound, 1.0f);
    } 
    
    public void PlaySound(AudioClip clip, float volumeScale)
    {
        audioSource.PlayOneShot(clip, volumeScale);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if( other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            PlaySound(collectedClip, 1.0f);
            ammo = ammo + 3;
            ammoText.text = "[C] Cogs: " + ammo.ToString();
        }
        if (other.gameObject.CompareTag("PickUp2"))
        {
            other.gameObject.SetActive(false);
            lightammo = lightammo + 5;
            lightammoText.text = "[V] Light Energy:" + lightammo.ToString();
        }
    }

    //public void PlaySound2(AudioClip clip2,float volumeScale )
    //{
        //audioSource2.PlayOneShot(clip2, volumeScale);
    //}
}