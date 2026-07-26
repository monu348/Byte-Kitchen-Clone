using System;
using UnityEngine;

public class PlateCounter : BaseCounter
{
    public event EventHandler OnplateSpawned;
    public event EventHandler OnplateRemoved;
    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    private float spawnPlateTimer;
    private float spawnTimerMax = 4f;
    private int plateSpawnedAmount;
    private int plateSpawnedAmountMax = 6;
    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnTimerMax)
        {
            spawnPlateTimer = 0f;
            if (KitchenGameManager.Instance.IsPlaying() && plateSpawnedAmount < plateSpawnedAmountMax)
            {
                plateSpawnedAmount++;
                OnplateSpawned?.Invoke(this , EventArgs.Empty);
            }
        }
    }
    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (plateSpawnedAmount > 0)
            {
                plateSpawnedAmount--;
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                OnplateRemoved?.Invoke(this , EventArgs.Empty);
            }
        }
    }

}
