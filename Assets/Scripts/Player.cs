using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {
  
  public static Player Instance { get; private set; }


  public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
  public class OnSelectedCounterChangedEventArgs : EventArgs {
    public ClearCounter selectedCounter;
  }


  [SerializeField] private float moveSpeed = 7f;
  [SerializeField] private GameInput gameInput;
  [SerializeField] private LayerMask countersLayerMask;
  
  [SerializeField] private Transform kitchenObjectHoldPoint;

  private KitchenObject kitchenObject;



  private bool isWalking;
  private Vector3 lastInteractDir;
  private ClearCounter selectedCounter;

  private void Awake() {
    if (Instance != null) {
      Debug.LogError("There is more than one Player instance");
    }
    Instance = this;
  }

  private void Start() {
    gameInput.OnInteractAction += GameInput_OnInteractAction;
  }

  private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
    if (selectedCounter != null) {
      selectedCounter.Interact(this);
    }
  }
  private void Update() {
    HandleMovement();
    HandleInteractions();
  }

  public bool IsWalking() {
    return isWalking;
  }

  private void HandleInteractions(){
    Vector2 inputVector = gameInput.GetMovementVectorNormalized();

    Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

    if (moveDir != Vector3.zero) {
      lastInteractDir = moveDir;
    } 

    float interactDistance = 2f;
    if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)){

      if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)){
        // Has ClearCounter
        if (clearCounter != selectedCounter) {
          SetSelectedCounter(clearCounter);
        } 
      } else {
        SetSelectedCounter(null);          
      }

    } else {
      SetSelectedCounter(null);          
    }

  }

  private void HandleMovement() {
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
      canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance); 

      if (canMove){
        moveDir = moveDirX;
      } else {
        // Cannot move only on the X
        
        // Attempt only Z movement
        Vector3 moveDirZ  = new Vector3(0, 0, moveDir.z).normalized;
        canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance); 

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

  private void SetSelectedCounter(ClearCounter selectedCounter){
    
    this.selectedCounter = selectedCounter;
    
    OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
      selectedCounter = selectedCounter
    });

  }

  public Transform GetKitchenObjectFollowTransfrom() {
        return kitchenObjectHoldPoint;
    }

  public void SetKitchenObject(KitchenObject kitchenObject) {
      this.kitchenObject = kitchenObject;
  }
  
  public KitchenObject GetKitchenObject() {
      return kitchenObject;
  }
  
  public void ClearKitchenObject() {
      kitchenObject = null;
  }
  
  public bool HasKitchenObject() {
      return kitchenObject != null;
  }
}

