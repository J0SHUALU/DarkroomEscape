using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 4f;
    public TMP_Text promptText;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            Interactable obj = hit.collider.GetComponentInParent<Interactable>();
            if (obj != null)
            {
                if (promptText != null) promptText.text = obj.interactionPrompt;
                if (Input.GetMouseButtonDown(0)) obj.Interact();
                return;
            }
        }
        if (promptText != null) promptText.text = "";
    }
}