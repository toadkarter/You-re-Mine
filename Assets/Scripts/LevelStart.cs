using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelStart : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI adventurerLocated;
    [SerializeField] private TextMeshProUGUI possessHim;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(LevelStartCutscene());

    }

    private IEnumerator LevelStartCutscene()
    {
        var isActive = false;
        for (var i = 0; i <= 8; i++)
        {
            adventurerLocated.gameObject.SetActive(!isActive);
            isActive = !isActive;

            if (i != 8)
            {
                yield return new WaitForSeconds(0.2f);
                
            }
        }

        possessHim.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }
}
