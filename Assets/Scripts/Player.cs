using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour {
  [SerializeField] private float moveSpeed = 7f;
  [SerializeField] private GameInput gameInput;

  private bool isWalking;
  private void Update() {

    Vector2 inputVector = gameInput.GetMovementVectorNormalized();

    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

    float moveDistance = moveSpeed * Time.deltaTime;
    float playerRadius = .7f;
    float playerHeight = 2f;


    bool canMove;
    canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance); 
    
    if (!canMove){
      // Cannot move towards moveDir

      // Attempt only X movement
      Vector3 moveDirX  = new Vector3(moveDir.x, 0, 0).normalized;
      canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance); 

      if (canMove){
        moveDir = moveDirX;
      } else {
        // Cannot move only on the X
        
        // Attempt only Z movement
        Vector3 moveDirZ  = new Vector3(0, 0, moveDir.z).normalized;
        canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance); 

        if (canMove){
          moveDir = moveDirZ;
        } else {
          // Cannot move in any direnction
          
          // TODO : 대각선 모서리에서 부자연스럽게 진행 못하는 문제.
          // TODO : 움직임 방향이 8방향 밖에 없는 문제.
        }
      }
    }
    
    if (canMove){
      transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
    
    isWalking = moveDir != Vector3.zero;
    
    float rotateSpeed = 10f;

    transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);

  }

  public bool IsWalking() {
    return isWalking;
  }
}
