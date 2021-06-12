using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask objectsLayer;
    public LayerMask waterLayer;

    private bool isMoving;
    private Vector2 input;
    private float moveSpeed = 5;
    private Animator animator;


    private void Awake() {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving){
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
                    
                    if(IsDead(targetPos))
                        Debug.Log("Player is dead");
                }
                                    
            }
        }

        animator.SetBool("isMoving", isMoving);
    }

    private bool CanWalk(Vector3 targetPos){
        if(Physics2D.OverlapCircle(targetPos, 0.3f, objectsLayer) != null){
            return false;
        }

        return true;
    }

    private bool IsDead(Vector3 targetPos){
        if(Physics2D.OverlapCircle(targetPos, 0.3f, waterLayer) != null){
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
