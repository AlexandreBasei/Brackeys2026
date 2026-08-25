using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    public bool triggeredFrenzy;
    public float timerReduction;
    public List<BystanderScript> bystanders;
    public List<BystanderScript> eligiblePossession;
    public int dayCount = 0;
    public float timeLeft;
    private IEnumerator runningTimer;
    private bool dayEnded;
    private int innocentKilled;
    public bool isPaused;
    

    
    void Start()
    {
        bystanders = new List<BystanderScript>();
        eligiblePossession = new List<BystanderScript>();
        isPaused = true;
        NextDay();
    }


    private IEnumerator Possession()
    {
        while (triggeredFrenzy == false && bystanders.Count != 0)
        {
            yield return new WaitForSeconds(Random.Range(7f-timerReduction, 10f-timerReduction));
            if(isPaused)
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
                        else if (dayCount >= 3 && Random.Range(0f, 4f) <1)
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
            StopCoroutine(runningTimer);
            ChooseDay();
        }
    }

    private IEnumerator TimerHandler()
    {
        while (!dayEnded)
        {
            yield return new WaitForSeconds(10f);
            timerReduction += 0.5f;
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
            if (!bystander.isDead)
            {
                bystander.Frenzy();
            }
        }
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
        timerReduction = 0;
        timeLeft = 120f;
        runningTimer = GameDuration(timeLeft);
        dayCount++;
        isPaused = false;
        PlayerController.Instance.dying = false;
        StartCoroutine(runningTimer);
        StartCoroutine(Possession());
        StartCoroutine(TimerHandler());
        dayEnded = false;
    }

    public IEnumerator GameDuration(float time)
    {
        yield return new WaitForSeconds(time);
        ChooseDay();
    }

    public void Cleanup()
    {
        foreach (var bystander in bystanders)
        {
            bystander.StopBehavior();
        }

        isPaused = true;
        bystanders.Clear();
    }


    public void ChooseDay()
    {
        if(dayEnded)
            return;
        dayEnded = true;
        Cleanup();
        switch (dayCount)
        {
            case 1:
                //Go to day 2
                NextDay();
                break;
            case 2:
                //Go to day 3
                NextDay();
                break;
            case 3:
                //Go to day 4
                NextDay();
                break;
            case 4:
                //End game
                break;
        }
        
    }

    
}
