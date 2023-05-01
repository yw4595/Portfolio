using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testLerp : MonoBehaviour
{
    public Image img1;
    public Image img2;
    public float t;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        img1.rectTransform.position = new Vector3(Mathf.Lerp(img1.rectTransform.position.x, img2.rectTransform.position.x, t), Mathf.Lerp(img1.rectTransform.position.y, img2.rectTransform.position.y, t), 0);
    }
}
