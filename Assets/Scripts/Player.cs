using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour {
  [SerializeField] private float moveSpeed = 7f;
  [SerializeField] private GameInput gameInput;

  private bool isWalking;
  private void Update() {

    Vector2 inputVector = gameInput.GetMovementVectorNormalized();

    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

    float playerSize = .7f;

    bool canMove = !Physics.Raycast(transform.position, moveDir, playerSize); 
    // TODO : 벽 왼쪽으로 지나갈 때 player 몸이 일부 겹치는 문제
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
