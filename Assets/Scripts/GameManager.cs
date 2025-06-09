using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Security.Cryptography;

public class GameManager : MonoBehaviour
{
    [Header("Animators and Visuals")]
    [SerializeField] private GameObject mainMenuUI;
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
    public float xValuePreviewVal;
    public float yValuePreviewVal;
    public bool isPlayer1sTurn = true;
    public int player1ScoreIndex;
    public int player2ScoreIndex;
    public int currentPlayer1Score;
    public int currentPlayer2Score;
    private bool isGameOver = false;

    [Header("Dartboard Info")]
    public RectTransform dartboard;
    public RectTransform dart;
    public RectTransform centrePoint;
    public GameObject player1PhysicalDart;
    public GameObject player2PhysicalDart;

    [Header("Audio & Sound Effects")]
    [SerializeField] private AudioSource soundEffects;
    [SerializeField] private AudioClip throwSFX;
    [SerializeField] private AudioClip dartboardHitSFX;
    [SerializeField] private AudioClip doorOpen;

   
    public void StartGameTrigger()
    {
        StartCoroutine(BeginGame());
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

    private IEnumerator BeginGame()
    {
        mainMenuUI.GetComponent<Animator>().SetBool("ShrinkUI", true);
        yield return new WaitForSeconds(1.5f);
        mainCamAnimator.SetBool("BeginGame", true);
        doorAnimator.SetBool("BeginGame", true);

        yield return new WaitForSeconds(1f);
        soundEffects.PlayOneShot(doorOpen);

        yield return new WaitForSeconds(1f);
        mainDartsUI.SetActive(true);

        yield return new WaitForSeconds(2f);
        BeginHorizontalPower();
    }

    private void Update()
    {
        /* THIS SECTION OF CODE IS FOR DEBUGGING THE DISTANCE OF THE AIM POINT FROM THE CENTRE OF THE BOARD
         * 
           Vector2 centrePointPosition = centrePoint.position;
           Vector2 dartPosition = dart.anchoredPosition;
           float distanceFromCentre = Vector2.Distance(centrePointPosition, dartPosition);
           Debug.Log(distanceFromCentre); 
        */

        UpdateDotPreview();
        EndTheGame();
    }

    public void UpdateDotPreview()
    {
        Vector2 dartboardSize = dartboard.rect.size;

        float xPos = (xValuePreviewVal * dartboardSize.x) - (dartboardSize.x / 2f);
        float yPos = (yValuePreviewVal * dartboardSize.y) - (dartboardSize.y / 2f);

        dart.anchoredPosition = new Vector2(xPos, yPos);
    }

    public void PlaceDart()
    {
        soundEffects.PlayOneShot(throwSFX);
        StartCoroutine(DelayHitSoundEffect());

        // Get the size of the dartboard. 
        Vector2 dartboardSize = dartboard.rect.size;

        // Calculate the position of the dart on the X and Y axis.
        float dartPosX = (selectedHorizontalPower * dartboardSize.x) - (dartboardSize.x / 2f);
        float dartPosY = (selectedVerticalPower * dartboardSize.y) - (dartboardSize.y / 2f);

        // Set the darts target location to these calculated dart positions.
        dart.anchoredPosition = new Vector2(dartPosX, dartPosY);

        // Change what the active dart is.
        if (isPlayer1sTurn)
        {
            player1PhysicalDart.GetComponent<DartHandler>().SlerpDartToPos();
        }
        else
        {
            player2PhysicalDart.GetComponent<DartHandler>().SlerpDartToPos();
        }

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
        else if(distanceFromCentre > 445f)
        {
            sectionHit = 0;
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
        xValuePreviewVal = 0.5f; 
        yValuePreviewVal = 0.5f; 

        // Reset the dart position to center (or wherever you want it idle)
        dart.anchoredPosition = Vector2.zero;

        // Reset Horizontal Power script
        horizontalPowerScript.progress = 0.5f;
        horizontalPowerScript.movingRight = true;
        horizontalPowerScript.powerSelected = false;
        horizontalPowerScript.SelectedHorizontalPower = 0f;
        horizontalPowerScript.powerIndicator.position = horizontalPowerScript.centrePoint.position;
        horizontalPowerScript.enabled = false;

        // Reset Vertical Power script
        verticalPowerScript.progress = 0.5f;
        verticalPowerScript.movingRight = true;
        verticalPowerScript.powerSelected = false;
        verticalPowerScript.SelectedVerticalPower = 0f;
        verticalPowerScript.powerIndicator.position = verticalPowerScript.centrePoint.position;
        verticalPowerScript.enabled = false;

        // Reset Physical Darts
        player1PhysicalDart.transform.position = new Vector3(-0.042f, 1.659f, 4.994f);
        player2PhysicalDart.transform.position = new Vector3(-0.042f, 1.659f, 4.994f);

        if (isPlayer1sTurn)
        {
            player1PhysicalDart.SetActive(true);
            player2PhysicalDart.SetActive(false);
        }
        else
        {
            player1PhysicalDart.SetActive(false);
            player2PhysicalDart.SetActive(true);
        }
    }

    private IEnumerator BeginThrowReset()
    {
        DisplayTextUserChange();

        yield return new WaitForSeconds(2f);
        ResetThrowState();
    }

    private void EndTheGame()
    {
        if(player2ScoreIndex == 10)
        {
            // GAME OVER!
            mainMenuUI.SetActive(false);
            Destroy(GetComponent<HorizontalPower>());
            Destroy(GetComponent<VerticalPower>());
            Destroy(mainDartsUI);
            GetComponent<EndGame>().enabled = true;
            GetComponent<EndGame>().player1FinalScore = currentPlayer1Score;
            GetComponent<EndGame>().player2FinalScore = currentPlayer2Score;
            Destroy(this.gameObject.GetComponent<GameManager>());   
        }
    }

    private IEnumerator DelayHitSoundEffect()
    {
        yield return new WaitForSeconds(0.35f);
        soundEffects.PlayOneShot(dartboardHitSFX);
    }
}