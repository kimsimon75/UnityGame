using UnityEngine;

namespace Plugins.RaycastPro.Demo.Scripts
{
    public class FreeTurn : MonoBehaviour
    {
        [SerializeField] private Vector3 moveTurn;
        [SerializeField] private  Vector3 rotateTurn;
        [SerializeField] private Vector3 randomRotate;
        [SerializeField] private Vector3 randomMove;
        [SerializeField] private float periodTime;

        private float timer;
        private void Start()
        {
            randomMove += new Vector3(Random.value * randomMove.x, Random.value * randomMove.y,
                Random.value * randomMove.z);
            rotateTurn += new Vector3(Random.value * randomRotate.x, Random.value * randomRotate.y,
                Random.value * randomRotate.z);
        }

        
        private void Update()
        {
            if (periodTime > 0)
            {
                if (timer >= periodTime)
                {
                    Apply(1);
                    timer = 0;
                }
                else
                {
                    timer += Time.deltaTime;
                }
            }
            else
            {
                Apply(Time.deltaTime);
            }

        }

        public void Apply(float delta)
        {
            transform.Rotate(rotateTurn * delta);
            transform.Translate(moveTurn * delta);
        }
    }
}
