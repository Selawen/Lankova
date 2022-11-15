using UnityEngine;
public class MechTurning : MonoBehaviour
{
    public float rotationSpeed = 5.0f, deadzoneSize;
    public AudioSource servoStart, servo, servoEnd;

    private void Update()
    {
        if (Time.timeScale <= 0.0f) { return; }

        float rotAngle = 0.0f;

        float mouseX = Input.mousePosition.x - (Screen.width / 2);
        
        if(mouseX > deadzoneSize) { rotAngle = rotationSpeed; }
        if(mouseX < -deadzoneSize) { rotAngle = -rotationSpeed; }

        if(rotAngle == 0 && (servo.isPlaying || servoStart.isPlaying))
        {
            servo.Stop(); servoStart.Stop(); servoEnd.Play();
        }

        if(rotAngle !=0 && !servoStart.isPlaying && !servo.isPlaying)
        {
            servoStart.Play();
            servo.PlayDelayed(servoStart.clip.length / servo.pitch);
        }

        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y + rotAngle * Time.deltaTime,
        transform.localRotation.eulerAngles.z);     
    }

}
