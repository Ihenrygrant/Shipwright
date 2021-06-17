using UnityEngine;


[ExecuteInEditMode]
public class CameraController : MonoBehaviour {

    private Rigidbody rb;

    float buildDistance = 10f;
    float speed = 7f;
    float rollSpeed = 70f;

    float speedH = 2.0f;
    float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private float roll = 0.0f;

    void Start()
    {
        PlayerInfo.PlayerInit();
        rb = GetComponent<Rigidbody>();
    }

    void Update() {

        switch (PlayerInfo.PlayerState) {
            case PlayerStates.Flying:
                firstPerson();
                break;
        }
	}

    private void firstPerson()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, roll);
    }

    public void RollPlayer(Vector3 vector)
    {
        transform.Rotate(vector * rollSpeed * Time.deltaTime);
    }

    public void SetVelocity(Vector3 velocity)
    {
        rb.velocity = velocity;
    }

    public void RotatePlayer(float speed)
    {
        transform.RotateAround(transform.position, transform.up, Time.deltaTime * speed);

    }

    public void DemoCameraMove()
    {
        transform.position = new Vector3(138.0F, 129F, 94F);

        yaw = -117.0F;
        pitch = 40.6F;
        roll = 0.0F;
    }
}
