using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private GameObject mainCamera;

    public int player1FinalScore;
    public int player2FinalScore;

    private Vector3 victorPosition = new Vector3 (-0.099f, 1.4f, -0.847f);
    private Vector3 loserPosition = new Vector3 (-1.338f, 0.983f, -2.441f);

    [SerializeField] private GameObject player1Character;
    [SerializeField] private GameObject player2Character;

    [SerializeField] private GameObject[] endGameObjects;

    [SerializeField] private TextMeshProUGUI endGameFinalText;
    [SerializeField] private AudioSource soundEffects;
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioClip winSFX;
    [SerializeField] private AudioClip gunshotSFX;


    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        foreach (GameObject GO in endGameObjects)
        {
            GO.SetActive(true);
        }
        SetWhoHasWon();
        StartCoroutine(BeginEndGameAnim());
    }

    private void SetWhoHasWon()
    {
        if(player1FinalScore > player2FinalScore)
        {
            player1Character.gameObject.transform.position = victorPosition;
            player2Character.gameObject.transform.position = loserPosition;

            endGameFinalText.text = "Player 1 WINS! With a score of " + player1FinalScore;
        }
        else if(player1FinalScore < player2FinalScore)
        {
            player1Character.gameObject.transform.position = loserPosition;
            player2Character.gameObject.transform.position = victorPosition;

            endGameFinalText.text = "Player 2 WINS! With a score of " + player2FinalScore;
        }
    }

    private IEnumerator BeginEndGameAnim()
    {
        StartCoroutine(LerpMusicVolume(0.1f, 0f, 2f));
        yield return new WaitForSeconds(2f);
        mainCamera.GetComponent<Animator>().SetBool("EndGame", true);
        soundEffects.PlayOneShot(winSFX);

        yield return new WaitForSeconds(6f);
        soundEffects.PlayOneShot(gunshotSFX);
    }

    private IEnumerator LerpMusicVolume(float startVolume, float endVolume, float duration)
    {
        float elapsed = 0f;
        music.volume = startVolume;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            music.volume = Mathf.Lerp(startVolume, endVolume, elapsed / duration);
            yield return null;
        }

        music.volume = endVolume;
    }
}
