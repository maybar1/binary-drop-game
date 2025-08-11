using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using UnityEngine.EventSystems;

public class BinaryTile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public int Row;
    public int Col;

    public int BitValue = 0;
    public TextMeshProUGUI bitText;
    public Image backgroundImage; 

    private Color originalColor;

    void Start()
    {
        originalColor = backgroundImage.color; 
        UpdateVisual();
    }

    public void SetValue(string value)
    {
        BitValue = int.Parse(value);
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        bitText.text = BitValue.ToString();
    }

    public void Toggle()
    {
        BitValue = 1 - BitValue;
        UpdateVisual();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.Instance.StartSelection(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.ContinueSelection(this);

    }
    public void OnPointerUp(PointerEventData eventData)
    {
        GameManager.Instance.EndSelection(); 
    }

    public void Highlight()
    {
        if (backgroundImage != null)
            backgroundImage.color = new Color(1f, 0.9f, 0.3f); 
        transform.localScale = Vector3.one * 1.1f;
    }


    public void ResetHighlight()
    {
        if (backgroundImage != null)
            backgroundImage.color = originalColor;

        transform.localScale = Vector3.one;
    }
    

}
