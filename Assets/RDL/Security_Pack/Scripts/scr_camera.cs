using UnityEngine;

public class scr_camera : MonoBehaviour
{
    public float rotate_amount;

    void Update()
    {
        transform.rotation = Quaternion.Euler(
            transform.eulerAngles.x,
            (Mathf.Sin(Time.realtimeSinceStartup) * rotate_amount) + transform.eulerAngles.y,
            transform.eulerAngles.z);
    }
}
