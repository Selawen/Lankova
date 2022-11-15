using UnityEngine;



/// <summary>
/// Mouselook shamelessly borrowed from: https://answers.unity.com/questions/29741/mouse-look-script.html
/// </summary>
public class MouseLook : MonoBehaviour
{
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -60F;
    public float maximumY = 60F;
    float rotationX = 0F;
    float rotationY = 0F;
    Quaternion originalRotation;
    public float deadzoneSize = 100;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        if (Time.timeScale <= 0.0f) { return; }

        if (axes == RotationAxes.MouseXAndY)
        {

            float mouseX = Input.mousePosition.x - (Screen.width / 2);
            float mouseY = Input.mousePosition.y - (Screen.height / 2);

            if (mouseX < deadzoneSize && mouseX > -deadzoneSize) { mouseX = 0.0f; }
            if (mouseY < deadzoneSize && mouseY > -deadzoneSize) { mouseY = 0.0f; }

            rotationX = mouseX / Screen.width * (maximumX - minimumX) * 1.5f;
            rotationY = mouseY / Screen.height * (maximumY - minimumY) * 1.5f;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }
        else if (axes == RotationAxes.MouseX)
        {
            rotationX += (Input.mousePosition.x - Screen.width / 2) / Screen.width; rotationX = ClampAngle(rotationX, minimumX, maximumX);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        }
        else
        {
            rotationY += (Input.mousePosition.y - Screen.height / 2) / Screen.height;
            rotationY = ClampAngle(rotationY, minimumY, maximumY);
            Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
            transform.localRotation = originalRotation * yQuaternion;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
