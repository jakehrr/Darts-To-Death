using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Security.Cryptography;

public class GameManager : MonoBehaviour
{
    [Header("Animators and Visuals")]
    public Animator mainCamAnimator;
    public Animator doorAnimator;
    public GameObject mainDartsUI;
    public GameObject player1Text;
    public GameObject player2Text;
    public TextMeshProUGUI[] player1scores;
    public TextMeshProUGUI[] player2scores;
    public TextMeshProUGUI mainPlayer1score;
    public TextMeshProUGUI mainPlayer2score;

    [Header("Script Accessing")]
    public HorizontalPower horizontalPowerScript;
    public VerticalPower verticalPowerScript;

    [Header("User Variables")]
    public float selectedHorizontalPower;
    public float selectedVerticalPower;
    public bool isPlayer1sTurn = true;
    public int player1ScoreIndex;
    public int player2ScoreIndex;
    public int currentPlayer1Score;
    public int currentPlayer2Score;

    [Header("Dartboard Info")]
    public RectTransform dartboard;
    public RectTransform dart;
    public RectTransform centrePoint;

    public void BeginGame()
    {
        mainCamAnimator.SetBool("BeginGame", true);
        doorAnimator.SetBool("BeginGame", true);
        StartCoroutine(DisplayDartsUI());
    }

    public void BeginHorizontalPower()
    {
        horizontalPowerScript.GetComponent<HorizontalPower>().enabled = true;
    }

    public void BeginVerticalPower()
    {
        verticalPowerScript.GetComponent<VerticalPower>().enabled = true;
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    private IEnumerator DisplayDartsUI()
    {
        yield return new WaitForSeconds(2f);
        mainDartsUI.SetActive(true);

        yield return new WaitForSeconds(2f);
        BeginHorizontalPower();
    }

   /* private void Update()
    {
        Vector2 centrePointPosition = centrePoint.position;
        Vector2 dartPosition = dart.anchoredPosition;

        float distanceFromCentre = Vector2.Distance(centrePointPosition, dartPosition);

        Debug.Log(distanceFromCentre);
    }*/

    public void PlaceDart()
    {
        Vector2 dartboardSize = dartboard.rect.size;

        float dartPosX = (selectedHorizontalPower * dartboardSize.x) - (dartboardSize.x / 2f);
        float dartPosY = (selectedVerticalPower * dartboardSize.y) - (dartboardSize.y / 2f);

        dart.anchoredPosition = new Vector2(dartPosX, dartPosY);

        // Get Landing Score (Un-Multiplied)
        int sectionHit = GetComponent<DartBoardHitCalculator>().GetDartboardSection(dart.anchoredPosition);

        // Get the distance from the centre point for multiplier calculations
        Vector2 centrePointPosition = centrePoint.position;
        Vector2 dartPosition = dart.anchoredPosition;
        float distanceFromCentre = Vector2.Distance(centrePointPosition, dartPosition);
        
        // Base Multiplier
        int multiplier = 1;
        if(distanceFromCentre >= 249 && distanceFromCentre <= 287) // Triple Score Multiplier
        {
            multiplier = 3;
        }
        else if(distanceFromCentre >= 409f && distanceFromCentre <= 445f) // Double Score Multiplier
        {
            multiplier = 2;
        }
        else if(distanceFromCentre <= 38) // Bullseye Hit
        {
            sectionHit = 50;
        }
        else if(distanceFromCentre >= 38.01 && distanceFromCentre <= 72) // Secondary Bullseye Hit
        {
            sectionHit = 25;
        }

        if (isPlayer1sTurn)
        {
            if(player1ScoreIndex < player1scores.Length)
            {
                player1scores[player1ScoreIndex].gameObject.SetActive(true);
                sectionHit *= multiplier;
                player1scores[player1ScoreIndex].text = sectionHit.ToString();
                currentPlayer1Score += sectionHit;
                mainPlayer1score.text = currentPlayer1Score.ToString();
                player1ScoreIndex++;
            }
        }
        else if(!isPlayer1sTurn)
        {
            if (player2ScoreIndex < player2scores.Length)
            {
                player2scores[player2ScoreIndex].gameObject.SetActive(true);
                sectionHit *= multiplier;
                player2scores[player2ScoreIndex].text = sectionHit.ToString();
                currentPlayer2Score += sectionHit;;
                mainPlayer2score.text = currentPlayer2Score.ToString();
                player2ScoreIndex++;
            }
        }
        isPlayer1sTurn = !isPlayer1sTurn;

        StartCoroutine(BeginThrowReset());
    }

    private void DisplayTextUserChange()
    {
        if (isPlayer1sTurn)
        {
            player1Text.SetActive(true);
            player2Text.SetActive(false);
        }
        else if (!isPlayer1sTurn)
        {
            player1Text.SetActive(false);
            player2Text.SetActive(true);
        }
    }

    private void ResetThrowState()
    {
        // Reset GameManager variables
        selectedHorizontalPower = 0f;
        selectedVerticalPower = 0f;

        // Reset the dart position to center (or wherever you want it idle)
        dart.anchoredPosition = Vector2.zero;

        // Reset Horizontal Power script
        horizontalPowerScript.progress = 0f;
        horizontalPowerScript.movingRight = true;
        horizontalPowerScript.powerSelected = false;
        horizontalPowerScript.SelectedHorizontalPower = 0f;
        horizontalPowerScript.powerIndicator.position = horizontalPowerScript.leftPoint.position;
        horizontalPowerScript.enabled = false;

        // Reset Vertical Power script
        verticalPowerScript.progress = 0f;
        verticalPowerScript.movingRight = true;
        verticalPowerScript.powerSelected = false;
        verticalPowerScript.SelectedVerticalPower = 0f;
        verticalPowerScript.powerIndicator.position = verticalPowerScript.bottomPoint.position;
        verticalPowerScript.enabled = false;
    }

    private IEnumerator BeginThrowReset()
    {
        DisplayTextUserChange();
        yield return new WaitForSeconds(2f);
        ResetThrowState();
    }
}
