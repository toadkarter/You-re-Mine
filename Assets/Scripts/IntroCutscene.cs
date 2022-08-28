using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] cutSceneLines;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject skipText;
    [SerializeField] private AudioClip soundtrack;
    [SerializeField] private AudioClip stinger;

    private AudioSource _audioSource;
    private bool _soundtrackIsPlaying;
    private bool _cutSceneIsFinished = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        StartCoroutine(PlayCutScene(0.01f, 2.0f, cutSceneLines));

        if (Input.GetButtonDown("Jump"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if (_cutSceneIsFinished)
        {
            LoadNextScene();
        }
    }

    private IEnumerator PlayCutScene(float timeIncrement, float delay, TextMeshProUGUI[] lines)
    {
        if (!_soundtrackIsPlaying)
        {
            _audioSource.PlayOneShot(soundtrack);
            _soundtrackIsPlaying = true;
        }

        foreach (var line in lines)
        {
            while (line.color.a < 1.0f)
            {
                IncrementTextVisibility(timeIncrement, line);
                yield return null;
            }

            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(delay);

        DisableLineVisibility(lines);

        skipText.SetActive(false);
        title.SetActive(true);
        FadeOut(2.0f);

        yield return new WaitForSeconds(delay + 1);

        _cutSceneIsFinished = true;

        yield return null;
    }

    private static void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private static void IncrementTextVisibility(float timeIncrement, Graphic line)
    {
        line.color = new Color(line.color.r, line.color.g, line.color.b,
            line.color.a + (Time.deltaTime * timeIncrement));
    }

    private static void DisableLineVisibility(IEnumerable<TextMeshProUGUI> lines)
    {
        foreach (var line in lines)
        {
            line.enabled = false;
        }
    }

    private IEnumerator FadeOut(float fadeTime)
    {
        float startVolume = _audioSource.volume;

        while (_audioSource.volume > 0)
        {
            _audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
        
        _audioSource.Stop();
    }
}
