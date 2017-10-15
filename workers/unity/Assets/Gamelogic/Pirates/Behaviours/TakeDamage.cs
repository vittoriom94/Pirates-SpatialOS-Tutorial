using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Pirates.Behaviours {
    // Add this MonoBehaviour on UnityWorker (server-side) workers only
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class TakeDamage : MonoBehaviour
    {
        [Require] private Health.Writer HealthWriter;

        private void OnTriggerEnter(Collider other) {
            if (HealthWriter == null) {
                return;
            }
            if(HealthWriter.Data.currentHealth <= 0) {
                return;
            }
            if (other != null && other.gameObject.tag == SimulationSettings.CannonballTag)
            {
                int newHealth = HealthWriter.Data.currentHealth - 250;
                HealthWriter.Send(new Health.Update().SetCurrentHealth(newHealth));
                if (newHealth <= 0) {
                    AwardPointsForKill(new EntityId(other.GetComponent<Cannons.DestroyCannonball>().firerEntityId.Value.Id));
                }
            }
        }

        private void AwardPointsForKill(EntityId firerEntityId) {
            uint pointsToAward = 1;
            // Use Commands API to issue an AwardPoints request to the entity who fired the cannonball
            SpatialOS.Commands.SendCommand(HealthWriter, Score.Commands.AwardPoints.Descriptor, new AwardPoints(pointsToAward), firerEntityId)
                .OnSuccess(OnAwardPointsSuccess)
                .OnFailure(OnAwardPointsFailure);
        }

        private void OnAwardPointsSuccess(AwardResponse response) {
            Debug.Log("AwardPoints command succeeded. Points awarded: " + response.amount);
        }

        private void OnAwardPointsFailure(ICommandErrorDetails response) {
            Debug.LogError("Failed to send AwardPoints command with error: " + response.ErrorMessage);
        }
    }
}