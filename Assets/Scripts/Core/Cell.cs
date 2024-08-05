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

    public IntVector2 coordinates;
    
    public Defines.CellTypes thisCellType;

    [SerializeField]
    protected Color activeColor;

    [SerializeField]
    protected Color deactiveColor;

    public List<Slot> slots;

    private EventTrigger eventTrigger;

    int currentTargetZAngle = 0;

    bool isMoving = false;

    public System.Action OnCellRotated;

    public Slot EastSlot
    { 
        get 
        { 
            return GetSlotInDirection(Defines.Directions.East); 
        } 
    }

    public Slot WestSlot
    {
        get
        {
            return GetSlotInDirection(Defines.Directions.West);
        }
    }

    public Slot NorthSlot
    {
        get
        {
            return GetSlotInDirection(Defines.Directions.North);
        }
    }

    public Slot SouthSlot
    {
        get
        {
            return GetSlotInDirection(Defines.Directions.South);
        }
    }

    public int CurrentState {
        get => currentState;

        set
        {
            currentState = value;
            //OnCurrentStateChanged(value);
        }
    }

    private void Awake()
    {
        if(thisCellType == Defines.CellTypes.Solid) { return; }

        spriteRenderer = container.GetComponentInChildren<SpriteRenderer>();
        if (slots == null || slots.Count == 0)
        {
            slots = new List<Slot>();
            slots = container.GetComponentsInChildren<Slot>().ToList();
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
        if (thisCellType == Defines.CellTypes.Solid) { return; }

        for (int i=0;i<slots.Count; i++) 
        { 
            slots[i].enabled = true;
            UpdateSlotDirection(slots[i]);
            slots[i].initializeSlot(OnSlotStateChanged);
        }
        
        // Initialize(IntVector2.Zero);
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
        Vector3 currEuler = container.rotation.eulerAngles;
        currEuler.z = currentTargetZAngle;
        container.rotation = Quaternion.Euler(currEuler);
        OnCellRotated?.Invoke();

        foreach (Slot slot in slots)
        {
            UpdateSlotDirection(slot);
        }
    }

    public void OnSlotStateChanged(bool newState)
    {
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

    public void Initialize(IntVector2 coords, int startRotation ,int startState = 0)
    {
        coordinates = coords;
        currentState = startState;
        if(thisCellType == Defines.CellTypes.Source || thisCellType == Defines.CellTypes.Source_2Slots) 
        { 
            currentState = 1; 
        
        }
        SetNewColor(currentState, true);
        container.rotation = Quaternion.Euler(0f, 0f, startRotation);
        if (thisCellType == Defines.CellTypes.Solid) { return; }
    }

    public void OnGameStart(System.Action cellRotateCallback)
    {
        if (thisCellType == Defines.CellTypes.Solid) { return; }
        OnCellRotated += cellRotateCallback;
    }

    public void SetNewColor(int newState, bool snap = false)
    {
        if (thisCellType == Defines.CellTypes.Solid) { return; }
        Color targetColor = newState == 1 ? activeColor : deactiveColor;
        if (snap) 
        {
            if(spriteRenderer == null)
                spriteRenderer = container.GetComponentInChildren<SpriteRenderer>();

            if(TryGetComponent<SpriteRenderer>(out SpriteRenderer iconRenderer))
            {
                spriteRenderer.color = targetColor;
            }
            
            spriteRenderer.color = targetColor;
            return;
        }
        spriteRenderer.DOColor(targetColor, 0.125f);
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer IconRenderer))
        {
            IconRenderer.DOColor(targetColor, 0.125f);
        }
    }

    public Slot GetSlotInDirection(Defines.Directions direction)
    {
        Vector2 targetDirection = Vector2.zero;

        switch (direction)
        {
            case Defines.Directions.North:      targetDirection = Vector2.up;       break;
            case Defines.Directions.South:      targetDirection = Vector2.down;     break;
            case Defines.Directions.West:       targetDirection = Vector2.left;     break;
            case Defines.Directions.East:       targetDirection = Vector2.right;    break;
        }
        return slots.Find(x => Vector2.Dot((x.transform.position - transform.position).normalized, targetDirection) > 0);
    }

    public void UpdateSlotDirection(Slot slot)
    {

        Vector2 targetDirection = Vector2.zero;
        for (int i = 0; i <= (int)Defines.Directions.East; i++) 
        {
            switch ((Defines.Directions)i)
            {
                case Defines.Directions.North: targetDirection = Vector2.up; break;
                case Defines.Directions.South: targetDirection = Vector2.down; break;
                case Defines.Directions.West: targetDirection = Vector2.left; break;
                case Defines.Directions.East: targetDirection = Vector2.right; break;
            }
            if (Vector2.Dot((slot.transform.position - transform.position).normalized, targetDirection) > 0)
            {
                slot.CurrentDirection = (Defines.Directions)i;
                break;
            }
        }
    }

    private Vector3 GetDirectionVector(Defines.Directions direction)
    {
        switch (direction)
        {
            case Defines.Directions.East:
                return container.right;
            case Defines.Directions.West:
                return -container.right;
            case Defines.Directions.North:
                return container.up;
            case Defines.Directions.South:
                return -container.up;
            default:
                return Vector3.zero;
        }
    }
}
