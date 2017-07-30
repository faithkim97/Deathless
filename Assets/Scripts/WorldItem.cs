﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interactable object that exists within the
/// game world (as opposed to the inventory).
/// </summary>
public class WorldItem : GameItem {
    /// <summary>
    /// The default speed at which objects should move.
    /// </summary>
    public const float DEFAULT_SPEED = 5f;

    /// <summary>
    /// Whether this item may be interacted with. Interactable
    /// items should, at the least, have a dialogue attached to
    /// them with examine text.
    /// </summary>
    [SerializeField] [HideInInspector]
    private bool interactable;

    /// <summary>
    /// The scale of the gameObject on start.
    /// </summary>
    private Vector3 startingScale;

    /// <summary>
    /// The z position of the gameObject on start.
    /// </summary>
    private float startingZPos;
    
    /// <summary>
    /// The sprite renderer attached to the gameObject.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// The Seeker attached to the gameObject. Part of the A* Pathfinding Project plugin.
    /// </summary>
    private Seeker seeker;

    /// <summary>
    /// The AIPath attached to the gameObject. Part of the A* Pathfinding Project plugin.
    /// </summary>
    private CustomAIPath aiPath;
    
    /// <summary>
    /// Initializes fields.
    /// </summary>
	void Start () {
        seeker = gameObject.GetComponent<Seeker>();
        aiPath = gameObject.GetComponent<CustomAIPath>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        if (aiPath != null) { aiPath.canMove = false; }
        startingScale = transform.localScale;
        startingZPos = GameManager.ZDepthMap.GetZDepthAtWorldPoint(transform.position);
    }

    void Update() {
        if (aiPath != null && aiPath.canMove) {
            UpdateScale();
        }
    }
    
    /// <summary>
    /// Trigger player movement and object interaction
    /// when the player clicks this object.
    /// </summary>
    void OnMouseUpAsButton() {
        //if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
        if (UIManager.WorldInputEnabled) {
            InteractionTarget = this;
            GameManager.Player.MoveToPoint(transform.position);
        }
    }

    public override void OnMouseEnter() {
        if (UIManager.WorldInputEnabled) {
            base.OnMouseEnter();
        }
    }

    public override void OnMouseExit() {
        if (UIManager.WorldInputEnabled) {
            base.OnMouseExit();
        }
    }

    public override void Interact() {
        if (interactable) { base.Interact(); }
    }
    
    /// <summary>
    /// Returns the current position of the instance of this object
    /// in the scene.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPosition() {
        if (Instance != null) {
            return Instance.transform.position;
        }
        else { return Vector3.zero; }
    }

    /// <summary>
    /// Initiate movement to a given point.
    /// </summary>
    public virtual void MoveToPoint(Vector2 point, float speed = DEFAULT_SPEED) {
        if (aiPath == null) {
            Debug.LogError("Tried to move " + gameObject.name + ", but it does not have pathfinding AI. " +
                "(Must have Seeker and CustomAIPath components attached.)");
        }
        else {
            aiPath.speed = speed;
            aiPath.targetReachedCallback += OnTargetReached;
            aiPath.canMove = true;

            Flip(true);
            if (Animator != null) { Animator.SetInteger("state", (int)AnimState.WALK); }

            seeker.StartPath(transform.position, point);
        }
    }

    /// <summary>
    /// Called when the item successfully arrives at its target.
    /// </summary>
    public void OnTargetReached(Transform target) {
        if (aiPath.canMove) {         //Helps to ensure this only gets called once.
            aiPath.canMove = false;

            Flip(false);
            if (Animator != null) { Animator.SetInteger("state", (int)AnimState.IDLE); }

            if (InteractionTarget != null) {
                InteractionTarget.Interact();
            }
        }
    }

    /// <summary>
    /// Updates the scale of the object based on its z position.
    /// </summary>
    protected void UpdateScale() {
        float currentZ = GameManager.ZDepthMap.GetZDepthAtWorldPoint(Instance.transform.position);

        float camZ = Camera.main.transform.position.z;
        float zDist = Mathf.Abs(camZ - currentZ);
        float startingZDist = Mathf.Abs(camZ - startingZPos);

        float flipModifier = Instance.transform.localScale.x < 0 ? -1 : 1;

        Vector3 scale = startingScale / (startingZDist / zDist);
        scale.x *= flipModifier;

        transform.localScale = scale;
    }

    /// <summary>
    /// Set this object's sprite.
    /// </summary>
    public override void ChangeSprite(Sprite sprite) {
        if (((WorldItem)Instance).spriteRenderer != null) {
            ((WorldItem)Instance).spriteRenderer.sprite = sprite;
        }
    }

    public void RemoveFromWorld() {
        Destroy(Instance.gameObject);
        World.UpdateNavGraph();
    }

    public void Enable() {
        Instance.gameObject.SetActive(true);
    }

    public void Flip(bool faceRight) {
        Vector3 scale = Instance.transform.localScale;
        bool flip = faceRight ? (scale.x > 0) : (scale.x < 0);

        if (flip) {
            scale.x *= -1;
            Instance.transform.localScale = scale;
        }
    }
}