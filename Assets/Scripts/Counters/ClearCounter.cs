using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    
    
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {//There is no kitchenObject here
            if (player.HasKitchenObject())
            {
                //The Player has kitchenObject but the counter doesnt 
                player.GetKitchenObject().SetKitchenObjectParent(this);
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
                //both player and counter carrying KitchenObject
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
                else
                {
                    //player is not carrying plate but something else
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        //Counter is holding a plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
            {
                //player doesnt carry the kitchenObject but the counter have a kitchenObject
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
   
}
