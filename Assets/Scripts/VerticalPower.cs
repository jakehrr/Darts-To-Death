using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPower : MonoBehaviour
{
    public RectTransform powerIndicator;
    public RectTransform bottomPoint;  
    public RectTransform topPoint;    
    public RectTransform centrePoint;    
    public float speed = 1f;             

    public bool movingRight = true;
    public float progress = 0.5f;

    public bool powerSelected = false;
    public float SelectedVerticalPower = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!powerSelected)
            {
                powerSelected = true;
                SelectedVerticalPower = progress;

                GetComponent<GameManager>().selectedVerticalPower = SelectedVerticalPower;
                GetComponent<GameManager>().PlaceDart();
            }
        }

        if (!powerSelected)
        {
            HorizontalPowerSlide();
        }

        GetComponent<GameManager>().yValuePreviewVal = progress;
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
