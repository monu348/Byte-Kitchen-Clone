using System;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter,IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }
    private State state;
    [SerializeField] private FryingReceipeSO[] fryingReceipeSOArray;
    [SerializeField] private BurningReceipeSO[] burningReceipeSOArray;
    private float fryingTimer;
    private float burningTimer;
    private FryingReceipeSO fryingReceipeSO;
    private BurningReceipeSO burningReceipeSO;
    private void Start()
    {
        state = State.Idle;
    }
    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        ProgressNormalized = fryingTimer / fryingReceipeSO.fryingTimerMax
                    });

                    if (fryingTimer > fryingReceipeSO.fryingTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingReceipeSO.output, this);
                      
                        state = State.Fried;
                        burningTimer = 0f;
                        burningReceipeSO = GetBurningReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            ProgressNormalized = fryingTimer / fryingReceipeSO.fryingTimerMax
                        });
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        ProgressNormalized = burningTimer / burningReceipeSO.burningTimerMax
                    });
                    if (burningTimer > burningReceipeSO.burningTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningReceipeSO.output, this);
                        
                        state = State.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            ProgressNormalized = 0f
                        });
                    }
                        break;
                case State.Burned:
                    break;
            }
        }
        
    }
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {//There is no kitchenObject here
            if (player.HasKitchenObject())
            {
                if (HasRecepieWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    //THE PLAYER CARRYING SOMETHING THAT CAN BE DROPPED
                    //The Player has kitchenObject but the counter doesnt 
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingReceipeSO = GetFryingReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    state = State.Frying;
                    fryingTimer = 0f;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });

                }
            }
            else
            {
                //nor the player nor the counter have KitchenObject
            }
        }
        else
        {
            //Counter have a kitchenObject
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                        state = State.Idle;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            ProgressNormalized = 0f
                        });
                    }
                }
                //both player and counter carrying KitchenObject
            }
            else
            {
                //player doesnt carry the kitchenObject but the counter have a kitchenObject
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state 
                });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    ProgressNormalized = 0f
                });
            }
        }
    }
    private bool HasRecepieWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingReceipeSO fryingReceipeSO = GetFryingReceipeSOWithInput(inputKitchenObjectSO);
        return fryingReceipeSO != null;

    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingReceipeSO fryingReceipeSO = GetFryingReceipeSOWithInput(inputKitchenObjectSO);
        if (fryingReceipeSO != null)
        {
            return fryingReceipeSO.output;
        }
        else
        {
            return null;
        }
    }
    private FryingReceipeSO GetFryingReceipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingReceipeSO fryingReceipeSO in fryingReceipeSOArray)
        {
            if (fryingReceipeSO.input == inputKitchenObjectSO)
            {
                return fryingReceipeSO;
            }
        }
        return null;
    }
    private BurningReceipeSO GetBurningReceipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningReceipeSO burningReceipeSO in burningReceipeSOArray)
        {
            if (burningReceipeSO.input == inputKitchenObjectSO)
            {
                return burningReceipeSO;
            }
        }
        return null;
    }
    public bool IsFried()
    {
        return state == State.Fried;    
    }
}

