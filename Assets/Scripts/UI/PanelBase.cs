using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelBase : MonoBehaviour
{
    [SerializeField]
    protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Show(float duration = 0.5f)
    {
        canvasGroup.DOFade(1f, duration).OnStart(() => gameObject.SetActive(true));
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide(float duration = 0.5f)
    {
        canvasGroup.DOFade(0f, duration).OnComplete(() => gameObject.SetActive(false));
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
