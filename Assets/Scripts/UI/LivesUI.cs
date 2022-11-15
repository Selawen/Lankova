using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{
    public float offset;
    public Image livesImage;

    Dictionary<int, Image> images = new Dictionary<int, Image>();

    int lives;
    public int Lives
    {
        get { return lives; }
        set
        {
            for (int i = images.Count; i < value; i++)
            {
                Image img = Instantiate(livesImage, transform);
                img.rectTransform.position += Vector3.right * offset * i;
                images.Add(i, img);
                img.gameObject.SetActive(true);
            }

            for(int i = value; i < images.Count && i>=0; i++)
            {
                Image img = images[i];
                images.Remove(i);
                Destroy(img.gameObject);
            }

            lives = Mathf.Max(value, 0);
        }
    }
}
