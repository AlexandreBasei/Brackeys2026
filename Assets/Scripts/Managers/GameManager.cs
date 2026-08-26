using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>
{
    public bool triggeredFrenzy;
    public float timerReduction;
    public List<BystanderScript> bystanders;
    public List<BystanderScript> eligiblePossession;
    public int dayCount = 0;
    public float timeLeft;
    private bool dayEnded;
    [HideInInspector] public int innocentKilled;
    public bool isPaused;
    public bool agressive;
    private IEnumerator timerHandler;
    private IEnumerator possessionCoroutine;



    void Start()
    {
        bystanders = new List<BystanderScript>();
        eligiblePossession = new List<BystanderScript>();
        isPaused = true;
        agressive = false;
    }

    private void Update()
    {
        if (isPaused || dayEnded || dayCount == 4)
            return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndDay();
        }

        print("timer reduction" + timerReduction);
    }


    private IEnumerator Possession()
    {
        while (triggeredFrenzy == false && bystanders.Count != 0)
        {
            yield return new WaitForSeconds(Random.Range(7f - timerReduction, 10f - timerReduction));
            if (isPaused)
                yield break;
            if (eligiblePossession.Count != 0)
            {
                BystanderScript possessed = eligiblePossession[Random.Range(0, eligiblePossession.Count)];
                if (!possessed.isPossessed && !possessed.isFaking)
                {
                    IEnumerator chosenCoroutine;
                    chosenCoroutine = possessed.Possession();
                    possessed.PlaySpawnSound();
                    if (dayCount >= 2)
                    {
                        if (Random.Range(0f, 3f) < 1)
                        {
                            chosenCoroutine = possessed.Fakeout();
                        }
                        else if (dayCount >= 3 && Random.Range(0f, 4f) < 1)
                        {
                            chosenCoroutine = possessed.Feral();
                        }
                    }
                    if (dayCount == 4)
                    {
                        chosenCoroutine = possessed.Smart();
                    }
                    possessed.StartBehavior(chosenCoroutine);
                }
            }
        }

        if (bystanders.Count == 0)
        {
            ChooseDay();
        }
    }

    private IEnumerator TimerHandler()
    {
        while (!dayEnded)
        {
            yield return new WaitForSeconds(10f);
            timerReduction += 1f;
        }
    }

    public void RemoveBystander(BystanderScript bystander, bool killed)
    {
        if (!bystander.isPossessed && killed)
        {
            innocentKilled++;
        }

        if (!bystander.isExcluded)
        {
            eligiblePossession.Remove(bystander);
        }

        if (killed)
        {
            agressive = true;
            bystanders.Remove(bystander);
        }

        if (innocentKilled >= 2)
        {
            Frenzy();
        }
    }

    public void AddBystander(BystanderScript bystander)
    {
        eligiblePossession.Add(bystander);
    }

    public void Frenzy()
    {
        foreach (BystanderScript bystander in bystanders)
        {
            if (!bystander.isDead && !triggeredFrenzy)
            {
                bystander.Frenzy();
            }
        }
        triggeredFrenzy = true;
    }

    private void FindBystanders()
    {
        bystanders.Clear();
        foreach (BystanderScript bystander in FindObjectsOfType<BystanderScript>())
        {
            bystanders.Add(bystander);
        }
        eligiblePossession.Clear();
        eligiblePossession.AddRange(bystanders);
    }

    public void NextDay()
    {
        FindBystanders();

        triggeredFrenzy = false;
        timerReduction = 0f;
        timeLeft = 60f;
        dayEnded = false;

        dayCount++;
        isPaused = false;
        timerHandler = TimerHandler();
        PlayerController.Instance.dying = false;
        possessionCoroutine = Possession();

        StartCoroutine(possessionCoroutine);
        StartCoroutine(timerHandler);
    }

    private void EndDay()
    {
        if (dayEnded)
            return;

        if (!agressive)
        {
            dayEnded = true;
            Cleanup();
            StartCoroutine(PacificEndSequence());
        }
        else
        {
            ChooseDay();
        }
    }

    public void Cleanup()
    {
        foreach (var bystander in bystanders)
        {
            bystander.StopBehavior();
        }
        StopCoroutine(timerHandler);
        StopCoroutine(possessionCoroutine);
        isPaused = true;
        triggeredFrenzy = false;
        innocentKilled = 0;
        bystanders.Clear();
    }


    public void ChooseDay()
    {
        if (dayEnded)
            return;
        dayEnded = true;
        Cleanup();
        switch (dayCount)
        {
            case 1:
                TutoManager.Instance.NextTuto();
                SceneManager.LoadScene("Day2");
                AudioManager.Instance.PlaySFX(AudioManager.Instance.nextDay);
                break;
            case 2:
                TutoManager.Instance.NextTuto();
                SceneManager.LoadScene("Day3");
                AudioManager.Instance.PlaySFX(AudioManager.Instance.nextDay);
                break;
            case 3:
                TutoManager.Instance.NextTuto();
                SceneManager.LoadScene("Day4");
                AudioManager.Instance.PlaySFX(AudioManager.Instance.nextDay);
                break;
            case 4:
                StartCoroutine(killerEndSequence());
                break;
        }

    }

    private IEnumerator PacificEndSequence()
    {
        PlayerController.Instance.showPacificEnd();
        yield return new WaitForSeconds(5f);
        loadMainMenu();
    }

    private IEnumerator killerEndSequence()
    {
        PlayerController.Instance.showKillerEnd();
        yield return new WaitForSeconds(5f);
        loadMainMenu();
    }

    public void ResetGame()
    {
        dayCount = 0;
        agressive = false;
        Cleanup();
    }

    private void showCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void loadMainMenu()
    {
        ResetGame();
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayMusic(AudioManager.Instance.MusicMainMenu);
        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
