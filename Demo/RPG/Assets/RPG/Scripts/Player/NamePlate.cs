using SlimNet;
using SlimNet.Unity;
using UnityEngine;

public class NamePlate : MonoBehaviour
{
    TextMesh textMesh;
    SynchronizedValue<string> playerName;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        playerName = gameObject.GetActorRecursive().GetValue<string>("Name");
    }

    void LateUpdate()
    {
        if (textMesh.text != playerName.Value)
        {
            textMesh.text = playerName.Value;
        }

        Camera cam = Camera.mainCamera;

        if (cam != null)
        {
            // Direction from us to look at position
            Vector3 d = cam.transform.position - transform.position;

            // Use the inverse and create a look rotation
            transform.rotation = Quaternion.LookRotation(-d.normalized);
        }
    }
}