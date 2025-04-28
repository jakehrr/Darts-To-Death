using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPower : MonoBehaviour
{
    public RectTransform powerIndicator;
    public RectTransform bottomPoint;  
    public RectTransform topPoint;    
    public float speed = 1f;             

    public bool movingRight = true;
    public float progress = 0f;

    public bool powerSelected = false;
    public float SelectedVerticalPower = 0;

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
            SelectedVerticalPower = progress;
            GetComponent<GameManager>().selectedVerticalPower = SelectedVerticalPower;
            GetComponent<GameManager>().PlaceDart();
        }
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
        powerIndicator.position = Vector3.Lerp(bottomPoint.position, topPoint.position, progress);
    }
}
