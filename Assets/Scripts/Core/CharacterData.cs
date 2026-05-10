using UnityEngine;

namespace Simonshouse.UI
{
    [System.Serializable]
    public class CharacterData
    {
        // ── Datos fijos ──────────────────────────────────────
        public string Name;
        public string Role;
        public int    Age;
        public string Occupation;
        public string ItemName;
        public string ItemLocation;

        // ── Estado dinámico ──────────────────────────────────
        public bool IsAlive               = true;
        public bool InteractedThisChapter = false;
        public bool ItemFound             = false;
        public int  Isolation             = 0;   // 0–100
        public int  Trust                 = 0;   // 0–100

        public CharacterData(string name, string role, int age, string occupation,
                             string itemName, string itemLocation)
        {
            Name = name; Role = role; Age = age; Occupation = occupation;
            ItemName = itemName; ItemLocation = itemLocation;
        }

        public void AddIsolation(int amount)
            => Isolation = Mathf.Clamp(Isolation + amount, 0, 100);

        /// Reduce aislamiento, sube confianza, marca interacción del capítulo.
        public void Connect(int amount)
        {
            AddIsolation(-amount);
            Trust = Mathf.Clamp(Trust + amount, 0, 100);
            InteractedThisChapter = true;
        }

        public IsolationLevel GetIsolationLevel()
        {
            if (Isolation >= 75) return IsolationLevel.Critical;
            if (Isolation >= 50) return IsolationLevel.High;
            if (Isolation >= 25) return IsolationLevel.Medium;
            return IsolationLevel.Stable;
        }
    }

    public enum IsolationLevel { Stable, Medium, High, Critical }
}
