using UnityEngine;

namespace Simonshouse.UI
{
    /// <summary>Entrada a la sala Lobby: capítulo y referencia opcional a Obj_FotoGrupo.</summary>
    public class LobbyController : MonoBehaviour
    {
        [SerializeField] private GameObject objFotoGrupo;

        private bool _fotoGrupoActivated;

        private void Start()
        {
            GameManager.Instance?.EnterRoom("Lobby");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCharacterDied -= OnCharacterDied;
                GameManager.Instance.OnCharacterDied += OnCharacterDied;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnCharacterDied -= OnCharacterDied;
        }

        private void OnCharacterDied(string _)
        {
            if (_fotoGrupoActivated || objFotoGrupo == null)
                return;
            _fotoGrupoActivated = true;
            objFotoGrupo.SetActive(true);
        }
    }
}
