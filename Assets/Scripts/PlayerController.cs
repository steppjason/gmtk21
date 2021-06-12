using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask objectsLayer;
    public LayerMask waterLayer;

    public GameController gameController;

    private bool isMoving;
    private bool isDead;
    private Vector2 input;
    private float moveSpeed = 5;
    private Animator animator;


    private void Awake() {
        animator = GetComponent<Animator>();
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        if(!isMoving && !isDead && gameController.State == GameState.Game){
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if(input.x != 0) input.y = 0;

            if(input != Vector2.zero){
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if(CanWalk(targetPos)){
                    StartCoroutine(Move(targetPos));
                    isDead = IsDead(targetPos);
                }
                                    
            }
        }

        if(gameController.State == GameState.Win){
            animator.SetBool("isWin", true);
        }

        if(!isMoving){
            animator.SetBool("isDead",isDead);
        }
        
        animator.SetBool("isMoving", isMoving);
    }

    public void Reset(){
        isMoving = false;
        isDead = false;
        animator.SetBool("isWin", false);
        animator.SetFloat("moveY", -1f);
    }

    private bool CanWalk(Vector3 targetPos){
        if(Physics2D.OverlapCircle(targetPos, 0.3f, objectsLayer) != null){
            return false;
        }
        return true;
    }

    private bool IsDead(Vector3 targetPos){
        if(Physics2D.OverlapCircle(targetPos, 0.3f, waterLayer) != null){
            gameController.UpdateGameState(GameState.Dead);
            gameController.deathSound.Play();
            return true;
        }
        return false;
    }

    IEnumerator Move(Vector3 targetPos){
        isMoving = true;

        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon){
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed *Time.deltaTime);
            yield return null;
        }

        isMoving = false;
    }
}
