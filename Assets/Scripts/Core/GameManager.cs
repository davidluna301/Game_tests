using System.Collections.Generic;
using UnityEngine;

namespace Simonshouse.UI
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        // ── Capítulo / Sala ───────────────────────────────────────
        public int    CurrentChapter  { get; private set; } = 0;
        public string CurrentRoomName { get; private set; } = "";

        // ── Tabla sala → capítulo ─────────────────────────────────
        private static readonly Dictionary<string, int> RoomToChapter = new()
        {
            { "Lobby",     1 },
            { "Estudio",   2 },
            { "Habitacion",3 },
            { "Galeria",   4 },
            { "Sotano",    5 },
        };

        // ── Estado de juego ───────────────────────────────────────
        public Dictionary<string, CharacterData> Characters { get; private set; }
        public HashSet<string>   Clues     { get; private set; } = new();
        public List<string>      Decisions { get; private set; } = new();
        public List<ItemData>    Inventory { get; private set; } = new();
        public bool SimonFound  { get; set; } = false;
        public bool CodeFound   { get; set; } = false;

        // ── Eventos ───────────────────────────────────────────────
        public event System.Action<string>   OnCharacterDied;
        public event System.Action<ItemData> OnItemAdded;
        public event System.Action           OnRoomClosed;   // = fin de capítulo

        // ─────────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitCharacters();
        }

        private void InitCharacters()
        {
            Characters = new Dictionary<string, CharacterData>
            {
                { "Ben",    new CharacterData("Ben",    "El Dinero",    38,
                    "Corredor de bolsa en quiebra",
                    "Libro de cuentas comprometedor",       "Habitación de Simón") },
                { "Lisa",   new CharacterData("Lisa",   "La Evidencia", 31,
                    "Periodista suspendida",
                    "Carpeta con fotografías y documentos", "Galería") },
                { "Robert", new CharacterData("Robert", "La Carta",     54,
                    "Abogado notarial semi-retirado",
                    "Carta manuscrita del padre común",     "Lobby") },
                { "Ana",    new CharacterData("Ana",    "Las Joyas",    45,
                    "Galerista y tasadora de arte",
                    "Estuche de cuero con joyas familiares","Estudio") },
                { "Lucas",  new CharacterData("Lucas",  "El Relicario", 27,
                    "Estudiante, ex-ayudante de Simón",
                    "Relicario de plata con inscripción",   "Sala de vigilancia") },
            };
        }

        // ── API de sala ───────────────────────────────────────────
        /// Llamar al inicio de cada RoomSceneController (Start).
        public void EnterRoom(string roomName)
        {
            CurrentRoomName = roomName;
            if (RoomToChapter.TryGetValue(roomName, out int ch))
                CurrentChapter = ch;
            Debug.Log($"[Room] Entrando a '{roomName}' — Capítulo {CurrentChapter}");
        }

        // ── API de estado ─────────────────────────────────────────
        public void AddClue(string id)   { Clues.Add(id); Debug.Log($"[Clue+] {id}"); }
        public bool HasClue(string id)   => Clues.Contains(id);
        public void AddDecision(string id) => Decisions.Add(id);

        public void AddItem(ItemData item)
        {
            if (item == null) return;
            Inventory.Add(item);
            OnItemAdded?.Invoke(item);
            Debug.Log($"[Item+] {item.itemName}");
        }

        public bool HasItem(string itemId)
            => Inventory.Exists(i => i.itemId == itemId);

        public List<string> GetAliveCharacters()
        {
            var list = new List<string>();
            foreach (var kv in Characters)
                if (kv.Value.IsAlive) list.Add(kv.Key);
            return list;
        }

        // ── Cierre de sala / capítulo ─────────────────────────────
        /// Aplica la mecánica de aislamiento. Llamar antes de salir de cada sala.
        public void CloseCurrentRoom()
        {
            // 1. +25 a personajes no interactuados en esta sala
            foreach (var c in Characters.Values)
                if (c.IsAlive && !c.InteractedThisChapter)
                    c.AddIsolation(25);

            // 2. Bonus de grupo si todos siguen vivos: −5 a todos
            bool allAlive = true;
            foreach (var c in Characters.Values)
                if (!c.IsAlive) { allAlive = false; break; }

            if (allAlive)
                foreach (var c in Characters.Values)
                    if (c.IsAlive) c.AddIsolation(-5);

            // 3. Reset de flags para la siguiente sala
            foreach (var c in Characters.Values)
                c.InteractedThisChapter = false;

            // 4. Muere solo el de mayor aislamiento ≥ 75 (uno por sala)
            CharacterData toKill = null;
            int maxLevel         = 74;
            foreach (var c in Characters.Values)
            {
                if (!c.IsAlive) continue;
                if (c.Isolation > maxLevel) { maxLevel = c.Isolation; toKill = c; }
            }

            if (toKill != null) KillCharacter(toKill.Name);

            OnRoomClosed?.Invoke();
        }

        public void KillCharacter(string name)
        {
            if (!Characters.TryGetValue(name, out var c) || !c.IsAlive) return;
            c.IsAlive = false;
            OnCharacterDied?.Invoke(name);
            Debug.Log($"[Death] {name} — Aislamiento: {c.Isolation}");
        }

        // ── Final ─────────────────────────────────────────────────
        /// "A" = nadie cumple condición · "B" = todos · "C" = mixto
        public string DetermineEnding()
        {
            int withItem = 0, withoutItem = 0;
            foreach (var c in Characters.Values)
            {
                if (!c.IsAlive) continue;
                if (c.ItemFound) withItem++;
                else             withoutItem++;
            }
            if (withItem    == 0) return "A";
            if (withoutItem == 0) return "B";
            return "C";
        }
    }
}
