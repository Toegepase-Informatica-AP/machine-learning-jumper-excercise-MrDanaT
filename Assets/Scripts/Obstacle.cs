using UnityEngine;

namespace Assets.Scripts
{
    public class Obstacle : MonoBehaviour
    {
        public float givenSpeed = 5f;
        public bool constantGivenSpeed = true;
        public float minSpeed = 1f;
        public float maxSpeed = 10;

        private float randomizedSpeed = 0f;
        private Environment environment;

        public void Start()
        {
            if (!constantGivenSpeed)
            {
                randomizedSpeed = Random.Range(minSpeed, maxSpeed);
            }
            else
            {
                randomizedSpeed = givenSpeed;
            }
        }

        private void FixedUpdate()
        {
            if (randomizedSpeed > 0f)
            {
                Move();
            }
        }

        private void Move()
        {
            if (environment == null)
            {
                environment = GetComponentInParent<Environment>();
            }

            transform.Translate(Vector3.forward * Time.deltaTime * randomizedSpeed);
        }

        private void OnTriggerEnter(Collider other)
        {
            bool isDeadLineCollission = other.CompareTag("DeadLine");

            if (isDeadLineCollission)
            {
                Destroy(gameObject);
                Player player = environment.GetComponentInChildren<Player>();
                if (player.gameObject != null)
                {
                    player.AddReward(1f);
                }
            }
        }
    }
}
