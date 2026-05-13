using UnityEngine;

namespace Simonshouse.UI
{
    /// <summary>Entrada a la sala Lobby. Activa <c>Obj_FotoGrupo</c> tras la primera muerte (por nombre en jerarquía).</summary>
    public class LobbyController : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance?.EnterRoom("Lobby");
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnCharacterDied += ActivateGroupPhoto;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnCharacterDied -= ActivateGroupPhoto;
        }

        private static void ActivateGroupPhoto(string _)
        {
            var fotoGrupo = GameObject.Find("Obj_FotoGrupo");
            if (fotoGrupo != null)
                fotoGrupo.SetActive(true);
        }
    }
}
