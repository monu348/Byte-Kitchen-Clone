using System;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour,IKitchenObjectParent
{
    public event EventHandler OnPickedSomething;
    public static Player Instance{get;private set;}
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter SelectedCounter;
    }
    private bool isWalking;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField]private GameInput gameInput;
    [SerializeField] private LayerMask counterLayerMask; 
    [SerializeField] private Transform kitchenObjectHoldPoint;
    private Vector3 lastInteractDir;
    private BaseCounter SelectedCounter;
    private KitchenObject kitchenObject;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more then one player");
        }
        Instance = this;
    }
    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsPlaying()) return;
        if (SelectedCounter != null)
        {
            SelectedCounter.InteractAlternate(this);

        }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!KitchenGameManager.Instance.IsPlaying()) return;
        if (SelectedCounter != null)
        {
            SelectedCounter.Interact(this);
        }
        
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }
    private void HandleInteractions()
    {
        Vector2 inputVector = gameInput.GameMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        if (moveDir != Vector3.zero)
        {
            lastInteractDir = moveDir;
        }
        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, counterLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != SelectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                    
                }
                
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
        
    }
    public bool IsWalking()
    {
        return isWalking;
    }
    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GameMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        float moveDistance = moveSpeed * Time.deltaTime;
        float PlayerRadius = 0.7f;
        float PlayerHeight = 2f;
        bool CanMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PlayerHeight, PlayerRadius, moveDir, moveDistance);
        if (!CanMove)
        {
            Vector3 moveDirx = new Vector3(moveDir.x, 0, 0).normalized;
            CanMove = moveDir.x!= 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PlayerHeight, PlayerRadius, moveDirx, moveDistance);
            if (CanMove)
            {
                moveDir = moveDirx;
            }
            else
            {
                Vector3 moveDirz = new Vector3(0, 0, moveDir.z).normalized;
                CanMove = moveDir.z!= 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * PlayerHeight, PlayerRadius, moveDirz, moveDistance);
                if (CanMove)
                {
                    moveDir = moveDirz;
                }
                else
                {

                }
            }
        }
        if (CanMove)
        {
            transform.position += moveDir * Time.deltaTime * moveSpeed;
        }

        float rotateSpeed = 10f;
        isWalking = moveDir != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }
    private void SetSelectedCounter(BaseCounter SelectedCounter)
    {
        this.SelectedCounter = SelectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            SelectedCounter = SelectedCounter
        });
    }
    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            OnPickedSomething?.Invoke(this, EventArgs.Empty);
        }
    }
    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }
    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }
    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

}
