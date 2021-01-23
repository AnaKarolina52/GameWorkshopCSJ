using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Atributtes")]
    public float health;
    public float speed; //declaramos variavel do tipo float com o nome velocidade
    public float jumpForce;
    public float atkRadius;
    public float recoveryTime;
    

    bool isJumping;
    bool isAttacking;
    bool isDead;


    float recoveryCount;

    [Header("Componets")]
    public Rigidbody2D rig;
    public Animator anim;
    public Transform firePoint;
    public LayerMask enemyLayer;
    public Image healthBar;
    public GameController gc;

    [Header("Audio Settings")]

    public AudioSource audioSource;
    public AudioClip sfx;
    


    // E chamado uma vez qdo o jogo e inicializado
    void Start()
    {
        
    }

    // e chamado a cada frame
    void Update()
    {
        if(isDead == false)
        {
            Jump();
            OnAttack();
        }
    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump"))
        {
            if(isJumping==false)
            {

            anim.SetInteger("transition", 2);
            rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;
            }
    
        }

    }

void OnAttack()
{
    if(Input.GetButtonDown("Fire1"))
    {
        isAttacking = true;
        anim.SetInteger("transition", 3);
        audioSource.PlayOneShot(sfx);

        Collider2D hit = Physics2D.OverlapCircle(firePoint.position, atkRadius, enemyLayer);

        if(hit != null)
        {
            hit.GetComponent<FlightEnemy>().OnHit();
    

        }

        StartCoroutine(OnAttacking());

    }
}

IEnumerator OnAttacking()
{
    yield return new WaitForSeconds(0.5f);
    isAttacking = false;
}

private void OnDrawGizmosSelected()
{
    Gizmos.DrawWireSphere(firePoint.position, atkRadius);
}

public void OnHit(float damage)
{
    recoveryCount += Time.deltaTime;

    if(recoveryCount >= recoveryTime && isDead == false)
    {

        anim.SetTrigger("hit");
        health -= damage;

        healthBar.fillAmount = health / 100;
        

        GameOver();

        recoveryCount = 0f;
    }
}

    void GameOver()
    {
        if(health <= 0)
        {
            anim.SetTrigger("die");
            isDead = true;
            gc.ShowGameOver();
        }
    }

   //e chamado pela fisica do jogo
    void FixedUpdate()
    {
        if(isDead ==false)
        {
            OnMove();

        }
       
    }

    void OnMove()
    {
         float direcao = Input.GetAxis("Horizontal");//variavel que armazena o input horizontal

        rig.velocity = new Vector2(direcao * speed, rig.velocity.y);// move o player na direcao do input
    
        if(direcao > 0 && isJumping == false && isAttacking == false)
        {
            transform.eulerAngles = new Vector2(0,0);//passa o valor de 0,0 no rotation do player
            anim.SetInteger("transition", 1);
        }
        if(direcao < 0 && isJumping == false && isAttacking == false)
        {
            transform.eulerAngles = new Vector2(0,180);//passa o valor de 0,180 no rotation do player
            anim.SetInteger("transition", 1);
        }

        if(direcao == 0 && isJumping == false && isAttacking == false)
        {
            anim.SetInteger("transition", 0);
        }
    }

 

    void OnCollisionEnter2D(Collision2D collision)
    {
        //se a codicao for atendida, player esta tocando o chao
        if(collision.gameObject.layer == 8)
        {
            isJumping= false;
        }
    }

}
