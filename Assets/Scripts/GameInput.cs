using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{

  public event EventHandler OnInteractAction;
  private PlayerInputActions playerInputActions;

  private void Awake(){
    playerInputActions = new PlayerInputActions();
    playerInputActions.Player.Enable();

    playerInputActions.Player.Interact.performed += Interact_performed;
    // attention :: not a function call.. just passing the function it self.
  }
  private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
    OnInteractAction?.Invoke(this, EventArgs.Empty);
  }

  public Vector2 GetMovementVectorNormalized(){
    Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
    
    inputVector = inputVector.normalized;
    // Debug.Log(inputVector);
    return inputVector;
  }

}
