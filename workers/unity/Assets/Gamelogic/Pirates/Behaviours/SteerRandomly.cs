
using UnityEngine;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Ship;

namespace Assets.Gamelogic.Pirates.Behaviours {
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class SteerRandomly : MonoBehaviour {

        [Require] private ShipControls.Writer ShipControlsWriter;

        private void RandomizeSteering() {
            ShipControlsWriter.Send(new ShipControls.Update()
                .SetTargetSpeed(Random.value)
                .SetTargetSteering((Random.value * 30.0f) - 15.0f));
        }

        private void OnEnable() {
            InvokeRepeating("RandomizeSteering", 0, 5.0f);
        }

        private void OnDisable() {
            CancelInvoke("RandomizeSteering");
        }

    }
}