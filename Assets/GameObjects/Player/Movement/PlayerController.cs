using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

internal struct ANIMSTATES
{
    public const string IDLE = "Idle";
    public const string WALK = "Walk";
}

public class PlayerController : MonoBehaviour
{
    /*
     FIELDS
    */
    // Component needed for moving around and display it
    NavMeshAgent _agent;
    Animator _animator;
    LineRenderer _lineRenderer;
    LineRenderer _previewLineRenderer;

    // Fields needed to move around
    [SerializeField] LayerMask _clickableLayers;    // TODO: Implement that
    private float _lastCalculatedWalkTime;
    Vector3 _virtualDestination;

    // Fields needed for preview (of movement and/or trajectories ?)
    List<List<Vector3>> _paths;
    Vector3[] _previewPath;
    
    // Fields needed for the Camera
    [SerializeField] float _lookRotationSpeed = 8f;

    public float _moveMult;
    public float _baseSpeed;
    public bool _resetMoveMult;

    [SerializeField] GameObject _destinationParticlePrefab;
    List<GameObject> _movementParticleInstances;
    List<Vector3> _plannedDestinations;


    /*
     METHODS
    */
    void Awake()
    {
        // Loads in the fields useful data and references
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _lineRenderer = GetComponent<LineRenderer>();
        _previewLineRenderer = GameObject.Find("RoomAnchor").GetComponent<LineRenderer>();
        _paths = new List<List<Vector3>>();

        _plannedDestinations = new List<Vector3>();
        _movementParticleInstances = new List<GameObject>();

        // Loading in PlayerManager a new state and its Action to change what the controls will do
        GI._PManFetcher()._virtualPos = _agent.transform.position;
        PlayerManager.AddOrOverrideState("movement", EnterMovementState, ExitState);

        _moveMult = 1f;
        _resetMoveMult = false;
        _baseSpeed = _agent.speed;
    }
    private void Update()
    {
        // TODO: Make them not spam Console to de-comment them
        //FaceTarget();
        //SetAnimations();

        if (_resetMoveMult == false) return;
        if (GI._PlayerFetcher().GetComponent<QueueComponent>().GetActiveCard() is not null) return;
        Debug.LogWarning("Reset mult");
        _moveMult = 1f;
        _resetMoveMult = false;
    }

    // ------
    // STATE RELATED
    // ------

    private void EnterMovementState()
    {
        PlayerManager manager = GI._PManFetcher();
        manager.SetLeftClickTo(ApplyMovement);
        manager.SetRightClickTo(() => { });
        manager.SetHoverTo(Preview);
        GI.UpdateCursors("Move", (byte)(GI.CursorRestriction.TILES));
    }

    void ExitState()
    {
        ClearPath();
    }


    // ------
    // MANIP ON LINE RENDERER & PATH
    // ------
    
    //Draws what path the player would take if he decided to move where the mouse is
    void Preview()
    {
       PlayerManager manager = GI._PManFetcher();
       // Crop the destination to the center of the target tile
       Vector3 alteredPos = manager._lastHit.transform.position;
       alteredPos.y += 0.5f;

        _agent.speed = _baseSpeed * _moveMult;

        // Calculate the path to the clicked point
        NavMeshPath path = new NavMeshPath();

        // In the following snipet, the commented code are those that use not cropped positions
        //if (NavMesh.CalculatePath(_virtualPos, hit.point, NavMesh.AllAreas,  path))
        if (NavMesh.CalculatePath(manager._virtualPos, alteredPos, NavMesh.AllAreas,  path))
        {
            _previewPath = path.corners;
            TrajectoryToolbox.DrawPath(_previewPath, ref _previewLineRenderer);
            _lastCalculatedWalkTime = GetPathTime(path);
        }

        _virtualDestination = alteredPos;
    }

    // Clear the path from the line renderer
    public void ClearPath()
    {
        _lineRenderer.positionCount = 0;
        _previewLineRenderer.positionCount = 0;
    }


    // ------
    // MOVEMENT STUFF
    // ------

    // Method to add a "movement Card" to the Queue
    // If it succeeds, prepares everything to render the path whilst keeping the preview
     void ApplyMovement()
     {
        //if (Time.timeScale == 0) return;
        if (_virtualDestination == GI._PManFetcher()._virtualPos) return;
        
        // Data duplication so that the closure takes the right data
        Vector3 vect = _virtualDestination;
        vect.y += 0.51f;

        _plannedDestinations.Add(vect);

        // We need to dynamically create a card in order to subscribe it to the Queue
        List<Vector3> slicedPath = new List<Vector3>();
        GameObject moveCardObj = new GameObject();
        Card moveCard = moveCardObj.AddComponent<Card>();
        //Card moveCard = new Card();
        moveCard._trigger += () =>
        {
            _agent.destination = vect;
            StartCoroutine(UpdatePath(slicedPath));

            GameObject movementParticleInstance = Instantiate(_destinationParticlePrefab, vect, Quaternion.identity);
            _movementParticleInstances.Add(movementParticleInstance);
        };
        moveCard._duration = _lastCalculatedWalkTime;

        // Check if the card's duration fit in the Queue
        if (GameObject.Find("Player").GetComponent<QueueComponent>().AddToQueue(moveCard))
        {
            // The movment fits in and so it's validated. Let's make sure the preview gets updated accordingly
            // We keep the previous preview so we can draw it
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos, _virtualDestination, NavMesh.AllAreas, path);
            _paths.Add(path.corners.ToList());
            TrajectoryToolbox.DrawPath(_paths, ref _lineRenderer);

            // Updating Player's virtual pos for the next preview
            GameObject.Find("Player").GetComponent<PlayerManager>()._virtualPos = _virtualDestination;

            GlobalStats.UpdateStat("mouvements", 1);

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Vector3 start = path.corners[i];
                Vector3 end = path.corners[i + 1];
                int segments = Mathf.CeilToInt(Vector3.Distance(start, end) / 0.1f); // Adjust segment length as needed

                for (int j = 0; j <= segments; j++)
                {
                    // Calculate the point along the segment
                    float t = (float)j / segments;

                    // Add the point to the path points
                    Vector3 point = Vector3.Lerp(start, end, t);

                    // Project the point onto the NavMesh surface
                    slicedPath.Add(point);
                }
            }
            slicedPath.Reverse();
        }
    }
    

    // Coroutine to erase parts of the path the agent moved past of
    IEnumerator UpdatePath(List<Vector3> path)
    {
        // Wait for the agent to reach the destination
        while (path.Count > 1)
        {
            if (Vector3.Magnitude(path[path.Count - 1] - GI._PlayerFetcher().transform.position) > 0.1)
            {
                path.RemoveAt(path.Count - 1);
                _paths[0] = path;
                TrajectoryToolbox.DrawPath(_paths, ref _lineRenderer);
            }
            yield return null;
        }
        _paths.RemoveAt(0);
        ClearRipples();
    }

    // ------
    // HANDLES HOW THE PLAYER LOOKS (ANIMS N SHIT)
    // ------

    // Rotate the player to face the target destination
    private void FaceTarget()
    {
        // Calculate the direction to the target destination
        Vector3 direction = (_agent.destination - transform.position).normalized;

        // Rotate the player towards the target destination
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Smoothly rotate the player
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _lookRotationSpeed);
    }

    // Set animations based on agent velocity
    private void SetAnimations()
    {
        if (_agent.velocity == Vector3.zero)
        {
            _animator.Play(ANIMSTATES.IDLE);
        }
        else
        {
            _animator.Play(ANIMSTATES.WALK);
        }
    }

    // ------
    // UTILITIES & TOOLS
    // ------

    float GetPathTime(NavMeshPath path)
    {
        // Calculate the time to traverse the path
        float time = 0;
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            // Get the start and end points of the segment
            Vector3 start = path.corners[i];
            Vector3 end = path.corners[i + 1];

            // Add the time to traverse the segment
            time += Vector3.Distance(start, end) / _agent.speed;
        }
        return time;
    }

    void ClearRipples()
    {
        int i = 0;
        foreach (GameObject movementParticleInstance in _movementParticleInstances)
        {
            if (i < 1)
            {
                i++;
                Destroy(movementParticleInstance);
            }
        }      
        _movementParticleInstances.Clear();
    }

    void CreateRipples(List<Vector3> positions)
    {
        foreach (Vector3 position in positions)
        {
            Vector3 ripplePosition = position;
            ripplePosition.y += 0.51f;
            GameObject movementParticleInstance = Instantiate(_destinationParticlePrefab, ripplePosition, Quaternion.identity);
            _movementParticleInstances.Add(movementParticleInstance);
        }
    }
}