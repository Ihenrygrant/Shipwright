using UnityEngine;

public class RotateIcon : MonoBehaviour {
    public Vector3 rotation = new Vector3();
    public float Speed = 1.0f;
    void Update()
    {
        transform.Rotate(rotation * Time.deltaTime * Speed);
    }
}

