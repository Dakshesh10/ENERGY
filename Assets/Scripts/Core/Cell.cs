using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour
{
    private int currentState;             

    private SpriteRenderer spriteRenderer;

    [SerializeField]
    [Range(15f, 90f)]
    protected int rotateStep = 90;

    [SerializeField]
    protected Transform container;

    [HideInInspector]
    public IntVector2 coordinates;
    
    public Defines.CellTypes thisCellType;

    [SerializeField]
    protected Color activeColor;

    [SerializeField]
    protected Color deactiveColor;

    protected List<Slot> slots;

    private EventTrigger eventTrigger;

    int currentTargetZAngle = 0;

    bool isMoving = false;

    public int CurrentState {
        get => currentState;

        set
        {
            currentState = value;
            OnCurrentStateChanged(value);
        }
    }

    private void Awake()
    {
        spriteRenderer = container.GetComponentInChildren<SpriteRenderer>();
        if (slots == null)
        {
            slots = new List<Slot>();
            slots = GetComponentsInChildren<Slot>().ToList();
        }

        #region ADDING_EVENT_TRIGGERS
        if (eventTrigger == null)
        {
            eventTrigger = GetComponent<EventTrigger>();

            EventTrigger.Entry onPointerClick = new EventTrigger.Entry();
            onPointerClick.eventID = EventTriggerType.PointerClick;
            UnityAction<BaseEventData> onPointerClickCallback = new UnityAction<BaseEventData>(OnCellClicked);
            onPointerClick.callback.AddListener(onPointerClickCallback);
            eventTrigger.triggers.Add(onPointerClick);
        }
        #endregion
    }

    private void OnEnable()
    {

        for (int i=0;i<slots.Count; i++) 
        { 
            slots[i].enabled = true;
            slots[i].initializeSlot(OnSlotStateChanged);
        }

        Initialize(IntVector2.Zero);
    }

    public void OnCellClicked(BaseEventData baseEventData)
    {
        
        if(isMoving) { return; }

        Vector3 currEuler = container.rotation.eulerAngles;
        currEuler.z = ((currEuler.z += rotateStep) % 360);
        currentTargetZAngle = Mathf.CeilToInt(currEuler.z);
        container.DORotate(currEuler, 0.25f, RotateMode.Fast).OnStart(OnCellRotateStart).OnComplete(OnCellRotateEnd).SetEase(Ease.OutSine);
    }

    protected void OnCellRotateStart()
    {
        isMoving = true;
    }

    protected void OnCellRotateEnd() 
    {
        isMoving = false;
        Vector3 currEuler = transform.rotation.eulerAngles;
        currEuler.z = currentTargetZAngle;
        container.rotation = Quaternion.Euler(currEuler);   
        // TODO: Trigger the Grid validation from here.
    }

    public void OnSlotStateChanged(bool newState)
    {
        Debug.Log("OnSlotStateChanged() " + newState);
        bool result = true;
        for(int i=0;i<slots.Count;i++)
        {
            result = result & slots[i].IsActive; 
            if(!result)
            {
                break;
            }
        }

        CurrentState = result ? 1 : 0;
    }

    public void Initialize(IntVector2 coords, int startState = 0)
    {
        coordinates = coords;
        currentState = startState;
        SetNewColor(currentState, true);
    }

    public void OnCurrentStateChanged(int newState)
    {
        SetNewColor(newState);
    }

    public void SetNewColor(int newState, bool snap = false)
    {
        Color targetColor = newState == 1 ? activeColor : deactiveColor;
        if (snap) 
        {
            if(spriteRenderer == null)
                spriteRenderer = container.GetComponentInChildren<SpriteRenderer>();
            
            spriteRenderer.color = targetColor;
            return;
        }
        spriteRenderer.DOColor(targetColor, 0.125f);
    }
}
