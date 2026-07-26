using UnityEngine;

public class TransitionController : MonoBehaviour
{
    [SerializeField] private Material transitionMaterial;
    [SerializeField] private float duration = 1f;

    private float timer;
    private bool isTransitioning;

    private void Start()
    {
        StartTransition();
    }

    private void Update()
    {
        if (!isTransitioning) return;

        timer += Time.deltaTime;

        float progress = timer / duration;
        progress = Mathf.Clamp01(progress);

        transitionMaterial.SetFloat("_Progress", progress);

        if (progress >= 1f)
        {
            isTransitioning = false;
        }
        Debug.Log(progress);
    }

    public void StartTransition()
    {
        timer = 0f;
        isTransitioning = true;
        transitionMaterial.SetFloat("_Progress", 0f);
    }

}
