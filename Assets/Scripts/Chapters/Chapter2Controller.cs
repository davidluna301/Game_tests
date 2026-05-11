using System.Collections.Generic;
using Simonshouse.UI;
using TMPro;
using UnityEngine;

namespace Simonshouse.Chapters
{
    public class Chapter2Controller : ChapterController
    {
        private readonly HashSet<string> exploredItems = new();

        [Header("Exploración — Estudio")]
        [SerializeField] private TextMeshProUGUI textRoomName;
        [SerializeField] private TextMeshProUGUI textRoomDesc;

        [Header("Interacciones / decisión")]
        [SerializeField] private TextMeshProUGUI textInteractionsTitle;
        [SerializeField] private TextMeshProUGUI textDecisionPrompt;

        protected override int GetChapterNumber() => 2;

        protected override void Start()
        {
            base.Start();
            GameManager.Instance?.EnterRoom("Estudio");
            BindLabels();
        }

        protected override string GetNarrativeText()
        {
            return "La luz había cambiado. El sol de la tarde se había movido y ahora entraba por el lado opuesto, "
                + "proyectando sombras más largas sobre el suelo de madera del lobby.\n\n"
                + "Fue Ana quien descubrió que la puerta del estudio no estaba cerrada con llave. La empujó sin pensar demasiado y la encontró abierta.";
        }

        private void BindLabels()
        {
            if (textRoomName != null) textRoomName.text = "EL ESTUDIO";
            if (textRoomDesc != null)
            {
                textRoomDesc.text =
                    "Madera oscura, olor a tinta seca y al polvo de los lienzos apilados. "
                    + "El reloj de escritorio marca las cuatro de la tarde. Cada objeto parece haber sido colocado "
                    + "para ser encontrado en el orden equivocado.";
            }

            if (textInteractionsTitle != null)
                textInteractionsTitle.text = "¿Con quién hablas?";

            if (textDecisionPrompt != null)
            {
                textDecisionPrompt.text =
                    "Las tensiones del estudio piden una dirección: confrontar, preguntar o intentar unir al grupo.";
            }
        }

        protected override void LoadExploration()
        {
            PrepareExplorationOptions();
            ClearOptions();

            if (!exploredItems.Contains("ledger"))
                SpawnOptionButton("Revisar el libro de contabilidad en el escritorio",
                    ExploreLedger);

            if (!exploredItems.Contains("tape"))
                SpawnOptionButton("Examinar la fotografía con cinta negra en la pared",
                    ExplorePhotoTape);

            SpawnOptionButton("Intentar abrir el archivador metálico", TryArchiver);

            SpawnOptionButton("Terminar de explorar → Hablar con el grupo", GoToInteractions);
        }

        private void ExploreLedger()
        {
            if (GameManager.Instance == null) return;
            exploredItems.Add("ledger");
            GameManager.Instance.AddClue("accounting_book");
            if (GameManager.Instance.Characters.TryGetValue("Ben", out var ben))
                ben.AddIsolation(10);

            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "Las cifras no mienten, pero tampoco confiesan todo. Entre anotaciones de ventas y gastos "
                            + "hay subrayados agresivos alrededor de transferencias a cuentas que ya no existen. "
                            + "Ben, de refilón, deja de respirar cuando pasas esa página."
                    }
                }
            };
            RunDialog(chain, LoadExploration);
        }

        private void ExplorePhotoTape()
        {
            if (GameManager.Instance == null) return;
            exploredItems.Add("tape");
            GameManager.Instance.AddClue("photo_black_tape");

            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "Una sexta fotografía ha sido arrancada del álbum y pegada aquí con cinta negra, "
                            + "como quien esconde una prueba a plena vista. El reverso está limpio: quien la colgó "
                            + "no quiso dejar nombre, pero sí certeza de que hay un testigo más de lo que creéis."
                    }
                }
            };
            RunDialog(chain, LoadExploration);
        }

        private void TryArchiver()
        {
            if (GameManager.Instance == null) return;

            if (!GameManager.Instance.HasItem("small_key"))
            {
                var locked = new DialogChain
                {
                    characterName = "",
                    lines = new List<DialogLine>
                    {
                        new()
                        {
                            isNarration = true,
                            content =
                                "El archivador está cerrado con un candado pequeño. Los cajones metálicos no ceden. "
                                + "Necesitas una llave diminuta para abrirlo."
                        }
                    }
                };
                RunDialog(locked, LoadExploration);
                return;
            }

            if (exploredItems.Contains("archiver")) return;
            exploredItems.Add("archiver");

            var open = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "La llave gira con un clic seco. Dentro, carpetas ordenadas por fecha y un sobre sin remitente "
                            + "con el membrete del estudio. Lo que había aquí confirma que alguien preparó este despacho "
                            + "para ser revisado… pero no por vosotros."
                    }
                }
            };
            RunDialog(open, LoadExploration);
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

        private void TalkTo(string name) => RunDialog(BuildDialogChainC2(name), LoadInteractions);

        private static DialogChain BuildDialogChainC2(string name) => name switch
        {
            "Robert" => new DialogChain
            {
                chainId = "C2_Robert",
                characterName = "Robert",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "ROBERT",
                        content =
                            "El estudio no es neutral. Cada trazo en la pared es un recordatorio de que Simón controlaba lo que veíamos."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Se acerca al escritorio sin tocar nada, como si el contacto le quemara."
                    },
                    new()
                    {
                        speakerName = "ROBERT",
                        content =
                            "Si hay documentos comprometedores, deberían estar bajo custodia legal. No en un cajón sin cerrar."
                    }
                }
            },
            "Ana" => new DialogChain
            {
                chainId = "C2_Ana",
                characterName = "Ana",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "ANA",
                        content =
                            "Los marcos de las obras son auténticos. Los precios en las facturas, no tanto. Alguien infló el valor de piezas vendidas a coleccionistas privados."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Sus dedos rozan un lienzo cubierto sin miraros a los ojos."
                    },
                    new()
                    {
                        speakerName = "ANA",
                        content = "No me gusta lo que implica. Pero me gusta menos fingir que no lo veo."
                    }
                }
            },
            "Ben" => new DialogChain
            {
                chainId = "C2_Ben",
                characterName = "Ben",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "BEN",
                        content =
                            "Los números del libro de contabilidad no son un accidente. Simón sabía quién le debía favores y quién le debía silencio."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Cierra el libro de un golpe seco, demasiado fuerte para ser casual."
                    },
                    new()
                    {
                        speakerName = "BEN",
                        content = "No estamos aquí por arte. Estamos aquí por deudas. Las dos cosas se parecen más de lo que duele admitir."
                    }
                }
            },
            "Lisa" => new DialogChain
            {
                chainId = "C2_Lisa",
                characterName = "Lisa",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "LISA",
                        content =
                            "Este despacho es un escenario. Las sombras en las fotos, el ángulo de la cinta… alguien nos está leyendo la partitura."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Abre la carpeta que lleva bajo el brazo sin ofrecer su contenido."
                    },
                    new()
                    {
                        speakerName = "LISA",
                        content = "Si encontráis algo que importe, que sea verificable. No quiero otra historia que no se pueda imprimir."
                    }
                }
            },
            "Lucas" => new DialogChain
            {
                chainId = "C2_Lucas",
                characterName = "Lucas",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "LUCAS",
                        content =
                            "Mezclaba pigmentos aquí mismo. A veces Simón dejaba notas en el borde de los lienzos, como si el cuadro fuera un diario."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Pasa el dedo por un marco y retira polvo con la manga."
                    },
                    new()
                    {
                        speakerName = "LUCAS",
                        content =
                            "Si la sexta foto es real… entonces hay alguien que estuvo en esta habitación después de que yo me fui. Y no era él."
                    }
                }
            },
            _ => new DialogChain { lines = new List<DialogLine>() }
        };

        protected override void LoadDecision()
        {
            PrepareDecisionOptions();
            ClearOptions();

            SpawnOptionButton("[1] Confrontar a Ben con lo del libro de contabilidad",
                () => ApplyDecision(1));
            SpawnOptionButton("[2] Preguntar a Lisa, en privado, qué sabe del incendio del puerto",
                () => ApplyDecision(2));
            SpawnOptionButton("[3] Proponer subir juntos al piso superior antes de que anochezca",
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
                    GameManager.Instance.AddClue("ben_confronted");
                    if (GameManager.Instance.Characters.TryGetValue("Ben", out var ben))
                        ben.AddIsolation(15);
                    RunDialog(DecisionNarration(
                            "Ben aprieta la mandíbula. No niega las cifras; las convierte en silencio cargado, "
                            + "y ese silencio pesa sobre todos."), EndChapter);
                    break;
                case 2:
                    GameManager.Instance.AddClue("lisa_trusts");
                    if (GameManager.Instance.Characters.TryGetValue("Lisa", out var lisa))
                        lisa.AddIsolation(-20);
                    RunDialog(DecisionNarration(
                            "Lisa te mira un segundo más de la cuenta. No promete nada. Pero deja de mirar el suelo: "
                            + "eso, en ella, es casi una confesión."), EndChapter);
                    break;
                case 3:
                    foreach (var n in alive)
                        GameManager.Instance.Characters[n].AddIsolation(-5);
                    RunDialog(DecisionNarration(
                            "Subís la escalera en grupo, demasiado cerca unos de otros para fingir indiferencia. "
                            + "La casa parece contener el aliento con vosotros."), EndChapter);
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
