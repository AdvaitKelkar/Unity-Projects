using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100f;

    private void Rotate()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime); 
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }
}
