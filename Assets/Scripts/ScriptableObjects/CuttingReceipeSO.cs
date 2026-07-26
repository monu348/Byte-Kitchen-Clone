using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecepieSO : ScriptableObject
{
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public int cuttingProgressMax;
}
