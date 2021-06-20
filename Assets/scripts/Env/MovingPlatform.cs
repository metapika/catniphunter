using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityCore {

    namespace Environment {

        public class MovingPlatform : MonoBehaviour {
            public bool allEnemiesDefeated;
            [SerializeField] Transform waypointsParent;
            [SerializeField] float travelTimeBetweenPoints = 5f;
            [SerializeField] float endPointStallTime = 3f;
            [SerializeField] float midPointStallTime = 1f;

            float dwellTimer = 0f;
            bool movingForward = true;
            int currentPointIndex = 0;
            [SerializeField] bool loopTrack = false;
            [SerializeField] bool activateOnStand = false;
            bool start = true;

            float stoppingDistanceThreshold = 0.05f;

            float t = 0f;

            Vector3 startingPoint;
            Vector3 targetPoint;

            List<Vector3> waypoints;

            public Rigidbody rb;

            private void Start() {
                if(activateOnStand == true)
                    start = false;
                else
                    start = true;
                
                waypoints = new List<Vector3>();

                for(int i = 0; i < waypointsParent.childCount; i++)
                {
                    waypoints.Add(waypointsParent.GetChild(i).position);
                }

                if(waypoints.Count < 1)
                {
                    Debug.LogError("You need more than 1 waypoint to travel along!");
                    enabled = false;
                }

                currentPointIndex = 0;

                startingPoint = GetWaypoint();
                GetNextWaypointIndex();
                targetPoint = GetWaypoint();

                transform.position = startingPoint;
            }

            private void FixedUpdate() {
                if(start)
                {
                    if(Vector3.Distance(transform.position, targetPoint) <= stoppingDistanceThreshold) {
                        dwellTimer += Time.deltaTime;

                        bool continueTrack = false;

                        if(IsAtEndpoint()) {
                            if(dwellTimer >= endPointStallTime)
                            {
                                continueTrack = true;
                            }
                        } else {
                            if(dwellTimer >= midPointStallTime)
                            {
                                continueTrack = true;
                            }
                        }

                        if(continueTrack) {
                            startingPoint = targetPoint;
                            GetNextWaypointIndex();
                            targetPoint = GetWaypoint();

                            t = 0f;
                            dwellTimer = 0f;
                        }

                        return;
                    }

                    t += (Time.deltaTime / travelTimeBetweenPoints);

                    
                    //Vector3 currentPos = Vector3.Lerp(startingPoint, targetPoint, t);
                    //rb.MovePosition(Vector3.Lerp(startingPoint, targetPoint, t));
                    transform.position = Vector3.Lerp(startingPoint, targetPoint, t);
                }
            }

            private Vector3 GetWaypoint() {
                return (waypoints[currentPointIndex]);
            }

            private void GetNextWaypointIndex() {
                if(!loopTrack)
                {
                    if(movingForward)
                    {
                        currentPointIndex++;
                        if(currentPointIndex >= waypoints.Count)
                        {
                            currentPointIndex = waypoints.Count - 2;
                            movingForward = false;
                        }
                    } else {
                        currentPointIndex--;
                        if(currentPointIndex < 0)
                        {
                            currentPointIndex = 1;
                            movingForward = true;
                        }
                    }
                } else {
                    currentPointIndex++;
                    if(currentPointIndex >= waypoints.Count)
                    {
                        currentPointIndex = 0;
                    }
                }
            }

            private bool IsAtEndpoint() {
                if(currentPointIndex == 0 || currentPointIndex == (waypoints.Count - 1)) {
                    return true;
                } else {
                    return false;
                }
            }

            private void OnTriggerEnter(Collider other) {
                if(other.CompareTag("Player"))
                {
                    start = true;
                    this.gameObject.GetComponent<BoxCollider>().enabled = false;
                }
            }

            private void OnDrawGizmos() {
                if(waypointsParent == null) return;

                Gizmos.color = Color.green;

                for(int i = 0; i < waypointsParent.childCount; i++)
                {
                    Gizmos.DrawSphere(waypointsParent.GetChild(i).position, 0.5f);

                    int nextWaypoint = (i + 1);
                    if(nextWaypoint >= waypointsParent.childCount) break;

                    Gizmos.DrawLine(waypointsParent.GetChild(i).position, waypointsParent.GetChild(nextWaypoint).position);
                }
            }
        }
    }
}
