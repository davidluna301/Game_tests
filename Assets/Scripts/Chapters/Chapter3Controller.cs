using System.Collections.Generic;
using Simonshouse.UI;
using TMPro;
using UnityEngine;

namespace Simonshouse.Chapters
{
    public class Chapter3Controller : ChapterController
    {
        private readonly HashSet<string> exploredItems = new();

        [Header("Exploración — Habitación de Simón")]
        [SerializeField] private TextMeshProUGUI textRoomName;
        [SerializeField] private TextMeshProUGUI textRoomDesc;

        [Header("Interacciones / decisión")]
        [SerializeField] private TextMeshProUGUI textInteractionsTitle;
        [SerializeField] private TextMeshProUGUI textDecisionPrompt;

        protected override int GetChapterNumber() => 3;

        protected override void Start()
        {
            base.Start();
            GameManager.Instance?.EnterRoom("Habitacion");
            BindLabels();
        }

        private void BindLabels()
        {
            if (textRoomName != null) textRoomName.text = "LA HABITACIÓN DE SIMÓN";
            if (textRoomDesc != null)
            {
                textRoomDesc.text =
                    "La penumbra huele a tabaco frío y a perfume barato. La cama está hecha con precisión militar; "
                    + "los cajones, no tanto. Cada objeto insiste en contar una versión distinta de la misma noche.";
            }

            if (textInteractionsTitle != null)
                textInteractionsTitle.text = "¿Con quién hablas?";

            if (textDecisionPrompt != null)
            {
                textDecisionPrompt.text =
                    "La carta en el buró cambia las reglas. Decidir qué hacéis con ella arrastrará al grupo hacia la verdad o hacia el secreto.";
            }
        }

        protected override string GetNarrativeText()
        {
            return "Las cinco en punto. La habitación de Simón parece haber esperado demasiado tiempo a que alguien "
                + "se atreviera a entrar del todo.\n\n"
                + "En el buró, bajo un pisapapeles de bronce, hay una carta inconclusa. Las primeras líneas bastan para "
                + "saber que no está dirigida a un desconocido: está dirigida a vosotros.";
        }

        protected override void LoadExploration()
        {
            PrepareExplorationOptions();
            ClearOptions();

            if (!exploredItems.Contains("letter"))
                SpawnOptionButton("Leer la carta inconclusa del buró", ExploreLetter);

            if (!exploredItems.Contains("drawer"))
                SpawnOptionButton("Revisar el cajón de la mesita de noche", ExploreDrawer);

            SpawnOptionButton("Terminar de explorar → Hablar con el grupo", GoToInteractions);
        }

        private void ExploreLetter()
        {
            if (GameManager.Instance == null) return;
            exploredItems.Add("letter");
            GameManager.Instance.AddClue("simon_alive");

            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "La letra tiembla en la mitad de una frase. Habla de un encuentro que no llegó a producirse, "
                            + "de una voz en el teléfono que prometió pruebas, de un hombre que «no puede estar muerto "
                            + "porque acaba de escribirme». El papel está fechado hace dos días."
                    }
                }
            };
            RunDialog(chain, LoadExploration);
        }

        private void ExploreDrawer()
        {
            if (GameManager.Instance == null) return;
            exploredItems.Add("drawer");
            GameManager.Instance.GrantItem("small_key", "Llave pequeña");

            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "Bajo un pañuelo doblado hay una llave pequeña, de esas que abren archivadores o cajas fuertes domésticas. "
                            + "Alguien la dejó donde la encontraríais sin esfuerzo… o donde delataría a quien la ignore."
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

        private void TalkTo(string name) => RunDialog(BuildDialogChainC3(name), LoadInteractions);

        private static DialogChain BuildDialogChainC3(string name)
        {
            if (name == "Robert")
            {
                return new DialogChain
                {
                    chainId = "C3_Robert",
                    characterName = "Robert",
                    lines = new List<DialogLine>
                    {
                        new()
                        {
                            isNarration = true,
                            content =
                                "Robert no cruza el umbral. Se queda en el marco de la puerta, como si la habitación fuera "
                                + "sagrada o como si le fuera a morder."
                        },
                        new()
                        {
                            speakerName = "ROBERT",
                            content =
                                "Desde aquí basta. He visto habitaciones como esta en testamentos y en juicios. Lo que importa "
                                + "no es el color de las paredes, sino lo que firmó o dejó de firmar quien durmió aquí."
                        },
                        new()
                        {
                            isNarration = true,
                            content =
                                "Su voz suena más lejos de lo que está. La distancia es voluntaria: si no entra, no puede "
                                + "ser culpable de lo que toquéis."
                        },
                        new()
                        {
                            speakerName = "ROBERT",
                            content =
                                "Si encontráis algo que lo comprometa… recordad que hay testigos y hay coartadas. Y yo, por ahora, "
                                + "tengo las dos."
                        }
                    }
                };
            }

            return name switch
            {
                "Ana" => new DialogChain
                {
                    chainId = "C3_Ana",
                    characterName = "Ana",
                    lines = new List<DialogLine>
                    {
                        new()
                        {
                            speakerName = "ANA",
                            content =
                                "Esta habitación es un museo de ego. Cada detalle grita control. Si Simón fingió su muerte, "
                                + "lo hizo con el mismo pulcro sadismo con el que colgaba sus cuadros."
                        },
                        new()
                        {
                            isNarration = true,
                            content = "Se acerca al armario y cuenta los percheros, como tasando un inventario."
                        },
                        new()
                        {
                            speakerName = "ANA",
                            content = "No me sorprendería que hubiera una salida oculta. Los narcisistas siempre dejan una puerta trasera."
                        }
                    }
                },
                "Ben" => new DialogChain
                {
                    chainId = "C3_Ben",
                    characterName = "Ben",
                    lines = new List<DialogLine>
                    {
                        new()
                        {
                            speakerName = "BEN",
                            content =
                                "La carta huele a trampa. Si Simón está vivo, alguien nos ha traído aquí para limpiar cabos sueltos… "
                                + "o para cobrar deudas viejas."
                        },
                        new()
                        {
                            isNarration = true,
                            content = "Mira la cama como quien calcula riesgos, no como quien manda."
                        },
                        new()
                        {
                            speakerName = "BEN",
                            content = "No me fío de las buenas noticias. Las malas, al menos, son honestas."
                        }
                    }
                },
                "Lisa" => new DialogChain
                {
                    chainId = "C3_Lisa",
                    characterName = "Lisa",
                    lines = new List<DialogLine>
                    {
                        new()
                        {
                            speakerName = "LISA",
                            content =
                                "Si esto sale a la luz, no es una noticia: es una sentencia. Hay nombres en esa carta que no deberían "
                                + "aparecer juntos en la misma frase."
                        },
                        new()
                        {
                            isNarration = true,
                            content = "Sus dedos rozan el sobre sin abrirlo del todo."
                        },
                        new()
                        {
                            speakerName = "LISA",
                            content = "Publicar no siempre es justicia. A veces es un disparo en un cuarto cerrado."
                        }
                    }
                },
                "Lucas" => new DialogChain
                {
                    chainId = "C3_Lucas",
                    characterName = "Lucas",
                    lines = new List<DialogLine>
                    {
                        new()
                        {
                            speakerName = "LUCAS",
                            content =
                                "Dormía en la buhardilla cuando él… cuando creíamos que él estaba aquí abajo. Oía pasos que no cuadraban."
                        },
                        new()
                        {
                            isNarration = true,
                            content = "Se sienta en el borde de la cama, demasiado joven para el cansancio que arrastra."
                        },
                        new()
                        {
                            speakerName = "LUCAS",
                            content =
                                "Si está vivo, entonces lo que enterramos no era un cuerpo. Era una mentira compartida. Y eso es peor."
                        }
                    }
                },
                _ => new DialogChain { lines = new List<DialogLine>() }
            };
        }

        protected override void LoadDecision()
        {
            PrepareDecisionOptions();
            ClearOptions();

            if (GameManager.Instance != null && GameManager.Instance.HasClue("simon_alive"))
            {
                SpawnOptionButton("[1] Revelar al grupo lo que implica la carta (Simón podría estar vivo)",
                    () => ApplyDecision(1));
            }

            SpawnOptionButton("[2] Guardar la carta y no mostrarla todavía",
                () => ApplyDecision(2));
            SpawnOptionButton("[3] Confrontar a Robert por su reacción en el umbral",
                () => ApplyDecision(3));
        }

        private void ApplyDecision(int option)
        {
            if (GameManager.Instance?.Characters == null)
            {
                EndChapter();
                return;
            }

            var alive = GameManager.Instance.GetAliveCharacters();

            switch (option)
            {
                case 1:
                    GameManager.Instance.AddClue("group_knows_simon_alive");
                    foreach (var n in alive)
                        GameManager.Instance.Characters[n].AddIsolation(-15);
                    RunDialog(DecisionNarration(
                            "Las palabras caen en la habitación como un objeto contundente. Si Simón puede estar vivo, "
                            + "todo lo que creísteis saber sobre esta casa acaba de partirse por la mitad."), EndChapter);
                    break;
                case 2:
                    GameManager.Instance.AddClue("letter_kept");
                    RunDialog(DecisionNarration(
                            "Dobláis la carta con cuidado casi ritual. El silencio que sigue no es alivio: es deuda."), EndChapter);
                    break;
                case 3:
                    GameManager.Instance.AddClue("robert_complicated");
                    if (GameManager.Instance.Characters.TryGetValue("Robert", out var robert))
                        robert.AddIsolation(-20);
                    RunDialog(DecisionNarration(
                            "Robert aprieta el marco de la puerta hasta que los nudillos se le ponen blancos. "
                            + "No grita. Eso, en él, es peor que cualquier insulto."), EndChapter);
                    break;
            }
        }

        private static DialogChain DecisionNarration(string body) => new()
        {
            characterName = "",
            lines = new List<DialogLine> { new() { isNarration = true, content = body } }
        };
    }
}
