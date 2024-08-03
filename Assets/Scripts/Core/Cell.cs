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

    public System.Action OnCellRotated;

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
        if (thisCellType == Defines.CellTypes.Solid) { return; }

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
        OnCellRotated?.Invoke();
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

    public void Initialize(IntVector2 coords, int startState = 0)
    {
        coordinates = coords;
        currentState = startState;
        SetNewColor(currentState, true);
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
            
            spriteRenderer.color = targetColor;
            return;
        }
        spriteRenderer.DOColor(targetColor, 0.125f);
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

    public Slot GetSlotInDirectionGPT(Defines.Directions direction)
    {
        // Get the direction vector in world space based on the container's rotation
        Vector3 directionVector = GetDirectionVector(direction);

        Slot closestSlot = null;
        float closestDot = -1f;

        foreach (Slot slot in slots)
        {
            Vector3 slotDirection = (slot.transform.position - container.position).normalized;
            float dot = Vector3.Dot(directionVector, slotDirection);

            if (dot > closestDot)
            {
                closestDot = dot;
                closestSlot = slot;
            }
        }

        return closestSlot;
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
