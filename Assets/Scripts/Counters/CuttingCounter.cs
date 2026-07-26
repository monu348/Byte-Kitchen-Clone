using System;
using UnityEngine;

public class CuttingCounter : BaseCounter,IHasProgress
{
    public static event EventHandler OnAnyCut;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;
    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }
    public class OnProgressChangedEventArgs : EventArgs
    {
        public float ProgressNormalized;
    }
    [SerializeField] private CuttingRecepieSO[] cuttingRecepieSOArray;
    private int cuttingProgress;
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {//There is no kitchenObject here
            if (player.HasKitchenObject())
            {
                if (HasRecepieWithInput(player.GetKitchenObject().GetKitchenObjectSO())){
                    //THE PLAYER CARRYING SOMETHING THAT CAN BE DROPPED
                    //The Player has kitchenObject but the counter doesnt 
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;
                    CuttingRecepieSO cuttingRecepieSO = GetCuttingRecepieWithInput(GetKitchenObject().GetKitchenObjectSO());
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        ProgressNormalized = (float)cuttingProgress / cuttingRecepieSO.cuttingProgressMax
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
                    }
                }
                //both player and counter carrying KitchenObject
            }
            else
            {
                //player doesnt carry the kitchenObject but the counter have a kitchenObject
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecepieWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;
            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);
            CuttingRecepieSO cuttingRecepieSO = GetCuttingRecepieWithInput(GetKitchenObject().GetKitchenObjectSO());
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                ProgressNormalized = (float)cuttingProgress / cuttingRecepieSO.cuttingProgressMax
            });
            if (cuttingProgress >= cuttingRecepieSO.cuttingProgressMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                //there is a kitchenobject here AND it can be cut
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }

        }
    }
    private bool HasRecepieWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecepieSO cuttingRecepieSO = GetCuttingRecepieWithInput(inputKitchenObjectSO);
        return cuttingRecepieSO != null;

    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecepieSO cuttingRecepieSO = GetCuttingRecepieWithInput(inputKitchenObjectSO);
        if (cuttingRecepieSO != null)
        {
            return cuttingRecepieSO.output;
        }
        else
        {
            return null;
        }
    }
    private CuttingRecepieSO GetCuttingRecepieWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecepieSO cuttingRecepieSO in cuttingRecepieSOArray)
        {
            if (cuttingRecepieSO.input == inputKitchenObjectSO)
            {
                return cuttingRecepieSO;
            }
        }
        return null;
    }
}
    
