using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{

    IEvent[] randomEvents;
    IEvent foodEvent;
    IEvent thirstEvent;
    IEvent hygieneEvent;
    IEvent dayPassEvent;

    IEvent currentEvent;
    private enum EventStates { FOOD, THIRST, HYGIENE, RANDOM, DAYPASS };
    private enum RandomEventStates { SURVIVORS, SEARCH, INJURY, ENERGY };
    private EventStates state;
    private RandomEventStates randomState;

    public int rounds;
    //public int eventsPerRound;
    private int currentRound = 0;
    //Start the number of events on 0.
    private int nEvent = -1;
    //private int eventIndex;

    private int life = 3;
    private int hunger = 3;
    private int hydration = 3;
    private int hygiene = 3;

    [SerializeField]
    private CollectibleItemScriptableObject[] collectibleObjects;
    [SerializeField]
    private ItensManagerScriptableObject itensManager;

    [TextArea(3, 10)]
    public string FoodEventChallengeText;
    [TextArea(3, 10)]
    public string SurvivorsEventChallengeText;
    [TextArea(3, 10)]
    public string SearchEventChallengeText;
    [TextArea(3, 10)]
    public string InjuryEventChallengeText;
    [TextArea(3, 10)]
    public string ThirstEventChallengeText;
    [TextArea(3, 10)]
    public string HygieneEventChallengeText;
    [TextArea(3, 10)]
    public string EnergyEventChallengeText;
    [TextArea(3, 10)]
    public string DayPassEventChallengeText;

    [TextArea(3, 10)]
    public string FoodEventIgnoreText;
    [TextArea(3, 10)]
    public string SurvivorsEventIgnoreText;
    [TextArea(3, 10)]
    public string SearchEventIgnoreText;
    [TextArea(3, 10)]
    public string InjuryEventIgnoreText;
    [TextArea(3, 10)]
    public string ThirstEventIgnoreText;
    [TextArea(3, 10)]
    public string HygieneEventIgnoreText;
    [TextArea(3, 10)]
    public string EnergyEventIgnoreText;
    [TextArea(3, 10)]
    public string DayPassEventIgnoreText;

    public event EventHandler OnGameEnd;
    public event EventHandler OnDeath;
    public event EventHandler OnLoseLife;
    public event EventHandler OnLoseHunger;
    public event EventHandler OnLoseThirst;
    public event EventHandler OnLoseHygiene;

    public Animator animator;

    // animation IDs
    private int animIDSuccess;
    private int animIDFailure;
    private int animIDDefault;
    private int animIDEatOrDrink;
    private int animIDGive;

    public int CurrentRound
    {
        get { return currentRound; }
    }

    public int NEvent
    {
        get { return nEvent; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        randomEvents = new IEvent[4]
        {
            new SurvivorsEvent(SurvivorsEventChallengeText, SurvivorsEventIgnoreText),
            new SearchEvent(SearchEventChallengeText, SearchEventIgnoreText),
            new InjuryEvent(InjuryEventChallengeText, InjuryEventIgnoreText),
            new EnergyEvent(EnergyEventChallengeText, EnergyEventIgnoreText)
        };

        foodEvent = new FoodEvent(FoodEventChallengeText, FoodEventIgnoreText);
        thirstEvent = new ThirstEvent(ThirstEventChallengeText, ThirstEventIgnoreText);
        hygieneEvent = new HygieneEvent(HygieneEventChallengeText, HygieneEventIgnoreText);
        dayPassEvent = new DayPassEvent(DayPassEventChallengeText, DayPassEventIgnoreText);

        currentEvent = foodEvent;
        state = EventStates.FOOD;
        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        animIDSuccess = Animator.StringToHash("isSuccess");
        animIDFailure = Animator.StringToHash("isFailure");
        animIDDefault = Animator.StringToHash("isDefault");
        animIDEatOrDrink = Animator.StringToHash("EatOrDrink");
        animIDGive = Animator.StringToHash("Give");
    }

    public void NextEvent()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        ResetTriggers();
        switch (state)
        {
            case EventStates.FOOD:
                currentRound += 1;
                if (currentRound == (rounds + 1))
                    OnGameEnd?.Invoke(this, EventArgs.Empty);
                else
                {
                    currentEvent = foodEvent;
                    animator.Play("Hunger");

                    nEvent += 1;
                    state = EventStates.THIRST;
                }
                break;

            case EventStates.THIRST:
                currentEvent = thirstEvent;

                animator.SetLayerWeight(1, 1);

                
                animator.Play("Idle");
                animator.Play("Thirst", 1);

                nEvent += 1;
                state = EventStates.HYGIENE;
                break;

            case EventStates.HYGIENE:
                currentEvent = hygieneEvent;

                animator.SetLayerWeight(1, 0);
                animator.Play("Hygiene");

                nEvent += 1;
                state = EventStates.RANDOM;
                break;

            case EventStates.RANDOM:
                int idx = UnityEngine.Random.Range(0,
                    itensManager.GetCollectibleItens().Exists(item => item.Type == CollectibleItemScriptableObject.ItemType.ELETRONIC)
                    ? randomEvents.Length : randomEvents.Length - 1);

                currentEvent = randomEvents[idx];
                randomState = (RandomEventStates)idx;

                if (randomState == RandomEventStates.INJURY)
                {
                    animator.SetLayerWeight(1, 1);
                    animator.Play("Idle");
                    animator.Play("Injury");
                }
                else
                    animator.Play("Thinking");

                nEvent += 1;
                state = EventStates.DAYPASS;
                break;

            case EventStates.DAYPASS:
                currentEvent = dayPassEvent;
                state = EventStates.FOOD;
                break;
        }
    }

    public string GetCurrentEventChallenge()
    {
        return currentEvent.Challenge();
    }

    public List<CollectibleItemScriptableObject> GetAllowedItens(List<CollectibleItemScriptableObject> itens)
    {
        return currentEvent.AllowedItens(itens);
    }

    public string GetResultMessage(CollectibleItemScriptableObject item)
    {
        return currentEvent.GetResultMessage(item);
    }

    public void IsSuccess(CollectibleItemScriptableObject item)
    {
        if (!currentEvent.Result(item))
        {
            OnFailEvent();
            animator.SetTrigger(animIDFailure);
        }

        else
        {
            switch (state)
            {
                case EventStates.THIRST:
                    animator.SetLayerWeight(2, 1);
                    animator.SetTrigger(animIDEatOrDrink);
                    break;

                case EventStates.HYGIENE:
                    animator.SetLayerWeight(2, 1);
                    animator.SetTrigger(animIDEatOrDrink);
                    break;

                case EventStates.RANDOM:
                    animator.SetTrigger(animIDSuccess);
                    break;

                case EventStates.DAYPASS:
                    switch (randomState)
                    {
                        case RandomEventStates.SEARCH:
                            animator.SetTrigger(animIDSuccess);
                            CollectibleItemScriptableObject itemToAdd = collectibleObjects[UnityEngine.Random.Range(0, collectibleObjects.Length)];
                            currentEvent.SetItemInMessage(itemToAdd);
                            itensManager.AddItem(itemToAdd);
                            break;

                        case RandomEventStates.SURVIVORS:
                            animator.SetTrigger(animIDGive);
                            itensManager.RemoveItem(item);
                            break;

                        default:
                            animator.SetTrigger(animIDSuccess);
                            break;
                    }
                    break;
            }
        }
    }

    public string GetIgnoreEventText()
    {
        if (currentRound == rounds && currentEvent == dayPassEvent)
            return currentEvent.GetResultMessage();
        return currentEvent.IgnoreEventText();
    }

    public void IsIgnoreEventSuccess()
    {
        animator.SetLayerWeight(1, 0);
        if (!currentEvent.IgnoreEventResult())
        {
            OnFailEvent();
            
            animator.SetTrigger(animIDFailure);
        }
        else
            animator.SetTrigger(animIDDefault);
    }

    private void OnFailEvent()
    {
        switch (state)
        {
            case EventStates.THIRST:
                hunger -= 1;
                OnLoseHunger?.Invoke(this, EventArgs.Empty);
                break;

            case EventStates.HYGIENE:
                hydration -= 1;
                OnLoseThirst?.Invoke(this, EventArgs.Empty);
                break;

            case EventStates.RANDOM:
                hygiene -= 1;
                OnLoseHygiene?.Invoke(this, EventArgs.Empty);
                break;

            case EventStates.DAYPASS:
                if (randomState == RandomEventStates.INJURY)
                {
                    life -= 1;
                    OnLoseLife?.Invoke(this, EventArgs.Empty);
                }
                    
                else if (randomState == RandomEventStates.ENERGY)
                {
                    CollectibleItemScriptableObject itemToRemove = itensManager.GetCollectibleItens().Find(item => item.Type == CollectibleItemScriptableObject.ItemType.ELETRONIC);
                    currentEvent.SetItemInMessage(itemToRemove);
                    itensManager.RemoveItem(itemToRemove);
                }
                    

                break;
        }
        if (hunger <= 0 || hydration <= 0 || hygiene <= 0 || life <= 0)
            OnDeath?.Invoke(this, EventArgs.Empty);
    }

    private void ResetTriggers()
    {
        animator.ResetTrigger("isSuccess");
        animator.ResetTrigger("isFailure");
        animator.ResetTrigger("isDefault");
    }
}
