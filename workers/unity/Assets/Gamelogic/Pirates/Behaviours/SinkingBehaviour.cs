using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable.Ship;

namespace Assets.Gamelogic.Pirates.Behaviours
{
    // Add this MonoBehaviour on client workers only
    [WorkerType(WorkerPlatform.UnityClient)]
    public class SinkingBehaviour : MonoBehaviour
    {
        [Require] Health.Reader HealthReader;

        private bool alreadySunk = false;

        private void OnCurrentHealthUpdate(int currentHealth) {
            if(!alreadySunk && currentHealth <= 0) {
                VisualiseSinking();
                alreadySunk = true;
            }
        }

        private void InitializeSinkingAnimation() {
            /*
             * SinkingAnimation is triggered when the ship is first killed. But a worker which checks out
             * the entity after this time (for example, a client connecting to the game later) 
             * must not visualize the ship as still alive.
             * 
             * Therefore, on checkout, any sunk ships jump to the end of the sinking animation.
             */
            if (HealthReader.Data.currentHealth <= 0) {
                foreach (AnimationState state in SinkingAnimation) {
                    // Jump to end of the animation
                    state.normalizedTime = 1;
                }
                VisualiseSinking();
                alreadySunk = true;
            }
        }


        [SerializeField]
        private Animation SinkingAnimation;

        private void OnEnable()
        {
            alreadySunk = false;
            InitializeSinkingAnimation();
            HealthReader.CurrentHealthUpdated.Add(OnCurrentHealthUpdate);
        }

        private void OnDisable()
        {
            HealthReader.CurrentHealthUpdated.Remove(OnCurrentHealthUpdate);
        }

        private void VisualiseSinking()
        {
            SinkingAnimation.Play();
        }
    }
}
