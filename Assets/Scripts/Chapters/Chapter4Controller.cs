using System.Collections.Generic;
using Simonshouse.UI;
using TMPro;
using UnityEngine;

namespace Simonshouse.Chapters
{
    public class Chapter4Controller : ChapterController
    {
        private readonly HashSet<string> exploredItems = new();

        [Header("Exploración — Galería")]
        [SerializeField] private TextMeshProUGUI textRoomName;
        [SerializeField] private TextMeshProUGUI textRoomDesc;

        [Header("Interacciones / decisión")]
        [SerializeField] private TextMeshProUGUI textInteractionsTitle;
        [SerializeField] private TextMeshProUGUI textDecisionPrompt;

        protected override int GetChapterNumber() => 4;

        protected override void Start()
        {
            base.Start();
            GameManager.Instance?.EnterRoom("Galeria");
            BindLabels();
        }

        private void BindLabels()
        {
            if (textRoomName != null) textRoomName.text = "LA GALERÍA";
            if (textRoomDesc != null)
            {
                textRoomDesc.text =
                    "Lienzos apoyados contra la pared como acusaciones silenciosas. La luz artificial tiñe de amarillo "
                    + "las caras y hace que cada sombra parezca más larga de lo que es. Las siete de la tarde suenan "
                    + "lejos, como si el tiempo fuera otra cosa aquí dentro.";
            }

            if (textInteractionsTitle != null)
                textInteractionsTitle.text = "¿Con quién hablas?";

            if (textDecisionPrompt != null)
            {
                textDecisionPrompt.text =
                    "El código del cuadro y la carpeta de Lisa tensan la sala. Decidís el siguiente movimiento antes de que la noche cierre el cerco.";
            }
        }

        protected override string GetNarrativeText()
        {
            return "La galería huele a trementina y a decisiones aplazadas. Los pasos resuenan distinto aquí: más claros, "
                + "más expuestos.\n\n"
                + "Entre dos bastidores, alguien ha dejado una carpeta con el membrete de un periódico. No está cerrada con llave: "
                + "casi invita a mirar.";
        }

        protected override void LoadExploration()
        {
            PrepareExplorationOptions();
            ClearOptions();

            if (!exploredItems.Contains("abstract"))
                SpawnOptionButton("Estudiar el cuadro abstracto con números incrustados", ExploreAbstract);

            if (!exploredItems.Contains("lisa_folder"))
                SpawnOptionButton("Revisar la carpeta entre los lienzos (evidencias)", ExploreLisaFolder);

            SpawnOptionButton("Terminar de explorar → Hablar con el grupo", GoToInteractions);
        }

        private void ExploreAbstract()
        {
            if (GameManager.Instance == null) return;
            exploredItems.Add("abstract");
            GameManager.Instance.AddClue("code_4729");
            GameManager.Instance.CodeFound = true;

            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "Los números no son decoración: 4-7-2-9, grabados con la misma mano que firmó facturas en el estudio. "
                            + "El código late en la retina como un segundo pulso."
                    }
                }
            };
            RunDialog(chain, LoadExploration);
        }

        private void ExploreLisaFolder()
        {
            if (GameManager.Instance == null) return;
            exploredItems.Add("lisa_folder");
            GameManager.Instance.AddClue("lisa_folder");

            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "Fotografías fechadas, recortes de puerto, anotaciones sobre testigos que «cambian de opinión». "
                            + "Lisa no discute cuando veis el contenido: solo cierra los ojos un segundo, como si os hubierais llevado "
                            + "su última coartada."
                    }
                }
            };
            RunDialog(chain, LoadExploration);
        }

        protected override void LoadInteractions()
        {
            PrepareInteractionsOptions();
            ClearOptions();
            if (GameManager.Instance?.Characters == null) return;

            foreach (var name in GameManager.Instance.GetAliveCharacters())
            {
                var captured = name;
                var charData = GameManager.Instance.Characters[name];
                SpawnOptionButton($"Hablar con {name}  —  {charData.Role}",
                    () => TalkTo(captured));
            }

            SpawnOptionButton("No hablar con nadie más → Tomar decisión", GoToDecision);
        }

        private void TalkTo(string name) => RunDialog(BuildDialogChainC4(name), LoadInteractions);

        private static DialogChain BuildDialogChainC4(string name) => name switch
        {
            "Robert" => new DialogChain
            {
                chainId = "C4_Robert",
                characterName = "Robert",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "ROBERT",
                        content =
                            "Una galería es teatro. Aquí se vende lo que la gente quiere creer sobre sí misma. Y pagáis caro por ello."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Se detiene frente al cuadro abstracto sin decir el número en voz alta."
                    },
                    new()
                    {
                        speakerName = "ROBERT",
                        content = "Si eso es una clave, no la murmuréis. Las paredes de esta casa tienen memoria."
                    }
                }
            },
            "Ana" => new DialogChain
            {
                chainId = "C4_Ana",
                characterName = "Ana",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "ANA",
                        content =
                            "La autenticidad aquí es negociable. Lo que no lo es es la cadena de custodia: si esas fotos salen de esta carpeta, "
                            + "hay responsables."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Sus ojos saltan del lienzo a la carpeta como buscando una coherencia imposible."
                    },
                    new()
                    {
                        speakerName = "ANA",
                        content = "Si vais al sótano, id con las manos visibles. Metafóricamente… y si hace falta, literalmente."
                    }
                }
            },
            "Ben" => new DialogChain
            {
                chainId = "C4_Ben",
                characterName = "Ben",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "BEN",
                        content =
                            "El puerto vuelve como un fantasma. Incendios, seguros, testigos sobornados… y ahora números en un cuadro. "
                            + "Alguien quiere que conectemos los puntos."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Ríe sin humor, mirando el techo como si calculara intereses."
                    },
                    new()
                    {
                        speakerName = "BEN",
                        content = "Si ese código abre algo, abrirá también deudas. Preparaos."
                    }
                }
            },
            "Lisa" => new DialogChain
            {
                chainId = "C4_Lisa",
                characterName = "Lisa",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "LISA",
                        content =
                            "Esa carpeta no es «periodismo». Es supervivencia. Si la habéis visto, ya sois parte de la historia… "
                            + "y de la diana."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Cierra la carpeta con fuerza, como quien intenta contener un incendio con las manos."
                    },
                    new()
                    {
                        speakerName = "LISA",
                        content = "Si bajáis, bajad sabiendo que abajo no hay arte. Hay consecuencias."
                    }
                }
            },
            "Lucas" => new DialogChain
            {
                chainId = "C4_Lucas",
                characterName = "Lucas",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "LUCAS",
                        content =
                            "Mezclaba barnices en un rincón como este. Simón decía que la galería era «el escaparate del miedo». "
                            + "No entendí el chiste hasta ahora."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Pasa el dedo por el marco del lienzo, casi reverente."
                    },
                    new()
                    {
                        speakerName = "LUCAS",
                        content = "Si el código es real… entonces él nos dejó una salida. O una trampa con forma de salida."
                    }
                }
            },
            _ => new DialogChain { lines = new List<DialogLine>() }
        };

        protected override void LoadDecision()
        {
            PrepareDecisionOptions();
            ClearOptions();

            if (GameManager.Instance != null && GameManager.Instance.CodeFound)
                SpawnOptionButton("[1] Bajar al sótano usando el código del cuadro", () => ApplyPrimaryPath(true));
            else
                SpawnOptionButton("[1] Volver a la habitación de Simón a buscar más pistas", () => ApplyPrimaryPath(false));

            SpawnOptionButton("[2] Compartir con el grupo lo que habéis encontrado (carpeta y galería)",
                ApplyShare);
        }

        private void ApplyPrimaryPath(bool codeFoundPath)
        {
            if (GameManager.Instance == null)
            {
                EndChapter();
                return;
            }

            if (codeFoundPath)
            {
                GameManager.Instance.AddClue("camera_room_open");
                RunDialog(DecisionNarration(
                        "El pasillo hacia el sótano huele a ozono y a metal frío. El código encaja en la mente como una llave: "
                        + "4729. Lo que hay detrás ya no es una exposición: es vigilancia."), EndChapter);
            }
            else
            {
                GameManager.Instance.AddClue("map_north_wing");
                GameManager.Instance.AddClue("north_warning");
                RunDialog(DecisionNarration(
                        "Regresáis a la habitación con la sensación de haber dejado atrás una pista demasiado grande. "
                        + "El mapa del ala norte queda grabado en la memoria del grupo como una advertencia silenciosa."), EndChapter);
            }
        }

        private void ApplyShare()
        {
            if (GameManager.Instance?.Characters == null)
            {
                EndChapter();
                return;
            }

            GameManager.Instance.AddClue("group_shares");
            foreach (var n in GameManager.Instance.GetAliveCharacters())
                GameManager.Instance.Characters[n].AddIsolation(-15);

            RunDialog(DecisionNarration(
                    "Las palabras fluyen con tanta franqueza que duele. Por un instante, la galería deja de ser un escenario "
                    + "y se convierte en un refugio frágil."), EndChapter);
        }

        private static DialogChain DecisionNarration(string body) => new()
        {
            characterName = "",
            lines = new List<DialogLine> { new() { isNarration = true, content = body } }
        };
    }
}
