using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbingScript : MonoBehaviour
{
    [SerializeField] private LayerMask PickupMask;
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private Transform PickupTarget;
    [Space]
    [SerializeField] private float PickupRange;
    private Rigidbody CurrentObject;
    public GameObject otherGameobject;
    public Transform otherGameobjectTransform;
    public float otherGameObjectFloat;
    public float objectSpeed = 12f;

    public Transform respawn;
    CharacterController pController;
    public bool isCarrying = false;


    // Start is called before the first frame update
    void Start()
    {
        pController = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        DistanceCalc();


        if (Input.GetMouseButtonDown(0))
        {


            if (CurrentObject)
            {
                isCarrying = false;
                otherGameobject = null;
                otherGameobjectTransform = null;
                CurrentObject.useGravity = true;
                CurrentObject.constraints = RigidbodyConstraints.None;
                CurrentObject.drag = 1;
                CurrentObject = null;
                isCarrying = false;
                return;
            }

            Ray CameraRay = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(CameraRay, out RaycastHit HitInfo, PickupRange, PickupMask) && isCarrying == false)
            {

                DistanceCalc();
                otherGameobject = HitInfo.transform.gameObject;
                otherGameobjectTransform = HitInfo.transform;
                isCarrying = true;
                CurrentObject = HitInfo.rigidbody;
                CurrentObject.useGravity = false;
                CurrentObject.constraints = RigidbodyConstraints.FreezeRotation;
                CurrentObject.drag = 10;
                isCarrying = true;
            }
        }

    }

    void DistanceCalc()
    {
        if (otherGameobjectTransform && CurrentObject)
        {
            float dist = Vector3.Distance(otherGameobjectTransform.position, transform.position);
            otherGameObjectFloat = dist;
            print("Distance: " + dist);
            if (dist > PickupRange)
            {
                CurrentObject.useGravity = true;
                CurrentObject.constraints = RigidbodyConstraints.None;
                isCarrying = false;
                otherGameobjectTransform = null;
                otherGameobject = null;
                CurrentObject = null;
            }
        }
    }



    private void FixedUpdate()
    {
        if (CurrentObject)
        {
            isCarrying = true;
            Vector3 DirectionToPoint = PickupTarget.position - CurrentObject.position;
            float DistanceToPoint = DirectionToPoint.magnitude;
            CurrentObject.velocity = DirectionToPoint * objectSpeed * DistanceToPoint;
        }

        if (CurrentObject == null)
        {
            otherGameobject = null;
            otherGameobjectTransform = null;
            isCarrying = false;
            return;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "lavaBlock")
        {
            pController.enabled = false;
            this.gameObject.transform.position = respawn.transform.position;
            pController.enabled = true;
        }
    }


}
