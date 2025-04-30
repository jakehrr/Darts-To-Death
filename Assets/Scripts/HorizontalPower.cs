using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalPower : MonoBehaviour
{
    public RectTransform powerIndicator;
    public RectTransform leftPoint;  
    public RectTransform rightPoint;    
    public float speed = 1f;             

    public bool movingRight = true;
    public float progress = 0f;

    public bool powerSelected = false;
    public float SelectedHorizontalPower = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            powerSelected = true;
        }

        if (!powerSelected)
        {
            HorizontalPowerSlide();
        }
        else if (powerSelected)
        {
            SelectedHorizontalPower = progress;
            GetComponent<GameManager>().selectedHorizontalPower = SelectedHorizontalPower;
            GetComponent<GameManager>().BeginVerticalPower();
        }

        GetComponent<GameManager>().xValuePreviewVal = progress;
    }

    private void HorizontalPowerSlide()
    {
        if (movingRight)
        {
            progress += speed * Time.deltaTime;
            if (progress >= 1f)
            {
                progress = 1f;
                movingRight = false;
            }
        }
        else
        {
            progress -= speed * Time.deltaTime;
            if (progress <= 0f)
            {
                progress = 0f;
                movingRight = true;
            }
        }

        // Move the indicator between the left and right points
        powerIndicator.position = Vector3.Lerp(leftPoint.position, rightPoint.position, progress);
    }
}
