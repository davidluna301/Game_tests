using Simonshouse.UI;
using UnityEngine;

namespace Simonshouse.Chapters
{
    public class Chapter1Controller : ChapterController
    {
        protected override int GetChapterNumber() => 1;

        protected override void LoadExploration()
        {
            ClearOptions();
            SpawnOptionButton("Simular Fin de Capítulo (Ben aislado)", OnSimulateEndWithBenDeath);
            SpawnOptionButton("Simular Fin de Capítulo (sin muerte)", OnSimulateEndNoDeath);
            SpawnOptionButton("Ir a interacciones", GoToInteractions);
        }

        protected override void LoadInteractions()
        {
            ClearOptions();
            SpawnOptionButton("Ir a decisión", GoToDecision);
        }

        protected override void LoadDecision()
        {
            ClearOptions();
            SpawnOptionButton("Volver a exploración", () =>
            {
                SetPanelState(false, true, false, false);
                LoadExploration();
            });
        }

        private void OnSimulateEndWithBenDeath()
        {
            var gm = GameManager.Instance;
            if (gm?.Characters == null) return;

            foreach (var c in gm.Characters.Values)
                c.InteractedThisChapter = true;

            if (gm.Characters.TryGetValue("Ben", out var ben))
                ben.Isolation = 80;

            foreach (var kv in gm.Characters)
            {
                if (kv.Key != "Ben" && kv.Value.IsAlive)
                    kv.Value.Isolation = 0;
            }

            EndChapter();
        }

        private void OnSimulateEndNoDeath()
        {
            var gm = GameManager.Instance;
            if (gm?.Characters == null) return;

            foreach (var c in gm.Characters.Values)
            {
                c.InteractedThisChapter = true;
                if (c.IsAlive)
                    c.Isolation = 0;
            }

            EndChapter();
        }
    }
}
