using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeFailed;
    public event EventHandler OnRecipeSuccess;
    public int successfulReciepesAmount;
    public static DeliveryManager Instance { get; private set; }
    [SerializeField] private ReceipeSOList receipeListSO;
    public List<ReceipeSO> waitingReceipeSOList;
    private float spawnReceipeTimer;
    private float spawnReceipeTimerMax=6f;
    private int waitingReceipeMax=6;
    private void Awake()
    {
        Instance = this;
        waitingReceipeSOList = new List<ReceipeSO>();
    }
    private void Update()
    {
        spawnReceipeTimer -= Time.deltaTime;
        if (spawnReceipeTimer <= 0f)
        {
            if (KitchenGameManager.Instance.IsPlaying() && waitingReceipeSOList.Count < waitingReceipeMax)
            {
                spawnReceipeTimer = spawnReceipeTimerMax;
                ReceipeSO waitingReceipeSO = receipeListSO.receipeSOList[UnityEngine.Random.Range(0, receipeListSO.receipeSOList.Count)];
                waitingReceipeSOList.Add(waitingReceipeSO);
                OnRecipeSpawned?.Invoke(this , EventArgs.Empty);
            }
        }
    }
    public void DeliverReceipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingReceipeSOList.Count; i++)
        {
            ReceipeSO waitingReceipeSO = waitingReceipeSOList[i];
            if (waitingReceipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                bool plateContentMatchesReceipe = true;
                //Has the same no of ingredients
                foreach (KitchenObjectSO receipeKitchenObjectSO in waitingReceipeSO.kitchenObjectSOList)
                {
                    //Cycling through all ingredients in receipe
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        //Cycling through all ingredients in plate
                        if (plateKitchenObjectSO == receipeKitchenObjectSO)
                        {
                            //ingredients does match
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        plateContentMatchesReceipe = false;
                    }
                }
                if (plateContentMatchesReceipe)
                {
                    successfulReciepesAmount++;
                    //player Delivered the correct Receipe
                    waitingReceipeSOList.RemoveAt(i);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }
        //No matches found!
        //player didnt delivered the correct receipe!
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }
    public List<ReceipeSO> GetWaitingReceipeSOList()
    {
        return waitingReceipeSOList;
    }
    public int GetsuccessfulReciepesAmount()
    {
        return successfulReciepesAmount;
    }
}
