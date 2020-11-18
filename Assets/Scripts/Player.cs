using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : Agent
    {
        public float jumpForce = 5.35f;

        private Rigidbody body;
        private Environment environment;
        private bool jumpIsReady = true;

        public override void CollectObservations(VectorSensor sensor)
        {
        }

        public override void Heuristic(float[] actionsOut)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }
        private void Jump()
        {
            if (jumpIsReady)
            {
                body.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
                jumpIsReady = false;
                AddReward(-0.15f);
            }
        }

        private void FixedUpdate()
        {
            if (jumpIsReady)
            {
                RequestDecision();
            }

            if(transform.position.y <= 1)
            {
                AddReward(0.001f);
            }
        }
        public override void Initialize()
        {
            base.Initialize();
            body = GetComponent<Rigidbody>();
            environment = GetComponentInParent<Environment>();
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            if (Mathf.FloorToInt(vectorAction[0]) == 1)
            {
                Jump();
            }
        }

        public override void OnEpisodeBegin()
        {
            body.angularVelocity = Vector3.zero;
            body.velocity = Vector3.zero;
            body.useGravity = true;

            environment.ClearEnvironment();
            if (environment.GetComponentInChildren<Player>() == null)
            {
                environment.SpawnPlayer();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Collider collider = collision.collider;
            if (collider.CompareTag("Obstacle"))
            {
                AddReward(-1f);
                EndEpisode();
            }
            else if (collider.CompareTag("Floor"))
            {
                jumpIsReady = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Coin"))
            {
                Destroy(other.gameObject);
                AddReward(0.6f);
            }
        }
    }
}
