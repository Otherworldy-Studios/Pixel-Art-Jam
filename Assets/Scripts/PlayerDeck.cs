using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerDeck : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] PlayerStats playerStats;
    [SerializeField] Vector3 targetViewRot;
    [SerializeField] Vector3 targetViewScale;


    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(targetViewScale, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOLocalRotate(Vector3.zero, 0.2f);
        transform.DOScale(Vector3.one, 0.2f);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
       playerStats.DrawCards(1);
    }

    
}
