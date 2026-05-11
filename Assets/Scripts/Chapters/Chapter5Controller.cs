using System.Collections.Generic;
using Simonshouse.UI;
using TMPro;
using UnityEngine;

namespace Simonshouse.Chapters
{
    public class Chapter5Controller : ChapterController
    {
        private readonly HashSet<string> exploredItems = new();
        private readonly HashSet<string> objectChoiceDone = new();

        private int _phase = 1;
        private bool _lightsApplied;
        private bool _doorResolved;

        [Header("Exploración — Sótano / noche")]
        [SerializeField] private TextMeshProUGUI textRoomName;
        [SerializeField] private TextMeshProUGUI textRoomDesc;

        [Header("Interacciones / decisión")]
        [SerializeField] private TextMeshProUGUI textInteractionsTitle;
        [SerializeField] private TextMeshProUGUI textDecisionPrompt;

        protected override int GetChapterNumber() => 5;

        protected override void Start()
        {
            base.Start();
            GameManager.Instance?.EnterRoom("Sotano");
            BindLabels();
        }

        private void BindLabels()
        {
            if (textRoomName != null) textRoomName.text = "LA NOCHE — SÓTANO";
            if (textRoomDesc != null)
            {
                textRoomDesc.text =
                    "Las diez en punto. El aire es más denso abajo; el zumbido de un transformador viejo marca un ritmo "
                    + "que no coincide con vuestros pulmones. Cada sombra parece demasiado deliberada.";
            }

            if (textInteractionsTitle != null)
                textInteractionsTitle.text = "¿Con quién hablas?";

            if (textDecisionPrompt != null)
            {
                textDecisionPrompt.text =
                    "Tras encontrar a Simón — o lo que queda de certeza sobre él— el grupo debe decidir cómo sale de la casa.";
            }
        }

        protected override string GetNarrativeText()
        {
            return "La mansión ha cambiado de registro. Lo que antes era tensión diplomática se ha vuelto supervivencia práctica.\n\n"
                + "El sótano espera. Quien baje primero dejará de ser un invitado y se convertirá en testigo… o en cebo.";
        }

        protected override void LoadExploration()
        {
            PrepareExplorationOptions();
            ClearOptions();

            switch (_phase)
            {
                case 1:
                    LoadPhase1Basement();
                    break;
                case 2:
                    LoadPhase2Objects();
                    break;
                case 3:
                    LoadPhase3Rescue();
                    break;
            }
        }

        private void LoadPhase1Basement()
        {
            if (GameManager.Instance == null) return;

            if (GameManager.Instance.CodeFound)
            {
                SpawnOptionButton("Entrar directamente en la sala de vigilancia (código conocido)",
                    EnterCamerasDirect);
            }
            else
            {
                SpawnOptionButton("Intentar abrir la cerradura electrónica al azar", Phase1RandomCode);
                SpawnOptionButton("Volver mentalmente a la galería y repasar el plano", Phase1ReturnGallery);
                SpawnOptionButton("Forzar la puerta del sótano con lo que hay a mano", Phase1ForceDoor);
            }
        }

        private void EnterCamerasDirect()
        {
            if (GameManager.Instance == null) return;
            exploredItems.Add("cameras");
            _phase = 2;
            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "El código encaja y la puerta cede sin teatro. Dentro, monitores apagados y cables ordenados con obsesión "
                            + "militar: alguien vigilaba a los vigilados. La sala de cámaras confirma lo peor: la casa siempre miraba."
                    }
                }
            };
            RunDialog(chain, LoadExploration);
        }

        private void Phase1RandomCode()
        {
            if (GameManager.Instance?.Characters == null) return;
            foreach (var n in GameManager.Instance.GetAliveCharacters())
                GameManager.Instance.Characters[n].AddIsolation(5);

            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "La cerradura zumba y se niega. Un led rojo parpadea burlón. El intento aleatorio solo confirma "
                            + "que alguien diseñó esto para no dejar cabos sueltos… salvo vosotros."
                    }
                }
            };
            RunDialog(chain, LoadExploration);
        }

        private void Phase1ReturnGallery()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.AddClue("gallery_recheck_stalled");

            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "Volvéis sobre vuestros pasos mentalmente: lienzos, cifras, la carpeta. Nada encaja del todo, "
                            + "pero el mapa del ala norte os recuerda que hay rutas que preferisteis no tomar… todavía."
                    }
                }
            };
            RunDialog(chain, LoadExploration);
        }

        private void Phase1ForceDoor()
        {
            if (GameManager.Instance?.Characters == null) return;
            foreach (var n in GameManager.Instance.GetAliveCharacters())
                GameManager.Instance.Characters[n].AddIsolation(10);
            GameManager.Instance.AddClue("forced_basement_door");
            exploredItems.Add("cameras");
            _phase = 2;

            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "Metal contra metal. La alarma no suena — peor: no hay alarma. Solo el chasquido de un mecanismo "
                            + "forzado y el silencio de quien esperaba que llegarais así. Entráis en la sala de vigilancia entre "
                            + "polvo y cables calientes."
                    }
                }
            };
            RunDialog(chain, LoadExploration);
        }

        private void LoadPhase2Objects()
        {
            if (GameManager.Instance?.Characters == null) return;

            var any = false;
            foreach (var kv in GameManager.Instance.Characters)
            {
                if (!kv.Value.IsAlive || kv.Value.ItemFound)
                    continue;
                var name = kv.Key;
                if (objectChoiceDone.Contains(name))
                    continue;

                any = true;
                var captured = name;
                SpawnOptionButton($"Ayudar a {name} a recuperar su objeto ({kv.Value.ItemName})",
                    () => ResolveObject(captured, true));
                SpawnOptionButton($"Ignorar la petición de {name}",
                    () => ResolveObject(captured, false));
            }

            if (!any)
            {
                _phase = 3;
                LoadExploration();
            }
        }

        private void ResolveObject(string name, bool help)
        {
            if (GameManager.Instance?.Characters == null) return;
            if (objectChoiceDone.Contains(name)) return;
            if (!GameManager.Instance.Characters.TryGetValue(name, out var c) || c.ItemFound)
                return;

            objectChoiceDone.Add(name);
            if (help)
            {
                c.Connect(20);
                c.ItemFound = true;
            }
            else
            {
                c.AddIsolation(15);
            }

            LoadExploration();
        }

        private void LoadPhase3Rescue()
        {
            if (GameManager.Instance == null) return;

            if (!_lightsApplied)
            {
                SpawnOptionButton("Encender las luces del pasillo hacia el ala norte", () =>
                {
                    _lightsApplied = true;
                    if (!GameManager.Instance.HasClue("north_warning"))
                    {
                        foreach (var n in GameManager.Instance.GetAliveCharacters())
                            GameManager.Instance.Characters[n].AddIsolation(10);
                    }

                    RunDialog(new DialogChain
                    {
                        characterName = "",
                        lines = new List<DialogLine>
                        {
                            new()
                            {
                                isNarration = true,
                                content =
                                    "La luz blanca devora las sombras de golpe. Durante un segundo, todos parecéis extraños "
                                    + "unos para otros: demasiado visibles, demasiado reales."
                            }
                        }
                    }, LoadExploration);
                });
                return;
            }

            if (!_doorResolved)
            {
                SpawnOptionButton("Probar la primera puerta del pasillo", () => WrongDoor());
                SpawnOptionButton("Probar la tercera puerta del pasillo", () => CorrectDoor());
                SpawnOptionButton("Probar la quinta puerta del pasillo", () => WrongDoor());
            }
        }

        private void WrongDoor()
        {
            if (GameManager.Instance?.Characters == null) return;
            foreach (var n in GameManager.Instance.GetAliveCharacters())
                GameManager.Instance.Characters[n].AddIsolation(10);

            RunDialog(new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "Un mecanismo falla con un golpe seco. Del otro lado solo hay habitación vacía y el olor a ozono. "
                            + "Habéis perdido tiempo… y piel social."
                    }
                }
            }, LoadExploration);
        }

        private void CorrectDoor()
        {
            if (GameManager.Instance == null) return;
            _doorResolved = true;
            GameManager.Instance.SimonFound = true;

            var lines = new List<DialogLine>
            {
                new()
                {
                    isNarration = true,
                    content =
                        "La tercera puerta cede sin resistencia teatral. Tras ella, el aire es más cálido, casi doméstico. "
                        + "Una figura encorvada levanta la vista: demasiado viva para ser un recuerdo, demasiado cansada para ser un fantasma."
                },
                new()
                {
                    isNarration = true,
                    content =
                        "Simón —o lo que queda de su certeza— os mira como quien reconoce una derrota aplazada. "
                        + "No hay triunfo en su voz. Solo alivio contaminado de miedo."
                }
            };

            foreach (var name in GameManager.Instance.GetAliveCharacters())
            {
                lines.Add(new DialogLine
                {
                    isNarration = true,
                    content = $"{name} contiene la respiración. Lo que ve redefine lo que cada uno creía estar dispuesto a pagar."
                });
            }

            RunDialog(new DialogChain { characterName = "", lines = lines }, GoToInteractions);
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

            SpawnOptionButton("No hablar con nadie más → Decisión final", GoToDecision);
        }

        private void TalkTo(string name) => RunDialog(BuildDialogChainC5(name), LoadInteractions);

        private static DialogChain BuildDialogChainC5(string name) => name switch
        {
            "Robert" => new DialogChain
            {
                chainId = "C5_Robert",
                characterName = "Robert",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "ROBERT",
                        content =
                            "Si él está aquí, entonces los testamentos, las firmas, las coartadas… todo se reabre. "
                            + "Esto no es una noche. Es un juzgado improvisado."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Sus manos tiemblan por primera vez."
                    },
                    new()
                    {
                        speakerName = "ROBERT",
                        content = "Salvadle si podéis. Pero salvad también lo que queda de la verdad, aunque os corte."
                    }
                }
            },
            "Ana" => new DialogChain
            {
                chainId = "C5_Ana",
                characterName = "Ana",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "ANA",
                        content =
                            "El arte fue su excusa. Esta casa fue su laboratorio. Y nosotros… éramos el público, ¿no?"
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Se seca los ojos con rabia contenida."
                    },
                    new()
                    {
                        speakerName = "ANA",
                        content = "Si salimos, salimos con él o sin él. Pero no con mentiras bonitas."
                    }
                }
            },
            "Ben" => new DialogChain
            {
                chainId = "C5_Ben",
                characterName = "Ben",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "BEN",
                        content =
                            "Esto cambia números, deudas, responsabilidades. Cambia quién puede chantajear a quién. "
                            + "Y cambia el precio de callar."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Mira a Simón un segundo y luego a vosotros, como calculando una liquidación."
                    },
                    new()
                    {
                        speakerName = "BEN",
                        content = "Si salimos juntos, será porque conviene. Si no… también."
                    }
                }
            },
            "Lisa" => new DialogChain
            {
                chainId = "C5_Lisa",
                characterName = "Lisa",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "LISA",
                        content =
                            "La historia que contéis mañana no será la de hoy. Escribidla con cuidado. O alguien la reescribirá por vosotros."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Sostiene el grabador apagado como si fuera un arma descargada."
                    },
                    new()
                    {
                        speakerName = "LISA",
                        content = "No quiero héroes. Quiero testigos vivos."
                    }
                }
            },
            "Lucas" => new DialogChain
            {
                chainId = "C5_Lucas",
                characterName = "Lucas",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "LUCAS",
                        content =
                            "Yo solo quería aprender a pintar. Y aprendí que hay cosas que no deberían enseñarse en voz alta."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Mira a Simón como quien mira un espejo roto."
                    },
                    new()
                    {
                        speakerName = "LUCAS",
                        content = "Si esto termina mal, que sea por la verdad. No por cobardía."
                    }
                }
            },
            _ => new DialogChain { lines = new List<DialogLine>() }
        };

        protected override void LoadDecision()
        {
            PrepareDecisionOptions();
            ClearOptions();

            SpawnOptionButton("[1] Salir juntos de la mansión, arrastrando la verdad como se pueda",
                () => FinalDecision(1));
            SpawnOptionButton("[2] Despedirse sin promesas: cada uno por su lado al amanecer",
                () => FinalDecision(2));
            SpawnOptionButton("[3] Buscar al asesino dentro de la casa antes de ir a la policia",
                () => FinalDecision(3));
        }

        private void FinalDecision(int option)
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
                    foreach (var n in alive)
                        GameManager.Instance.Characters[n].AddIsolation(-10);
                    RunDialog(DecisionNarration(
                            "Salís arrastrando pasos y silencios. La verdad pesa, pero al menos pesa repartida."), EndChapter);
                    break;
                case 2:
                    foreach (var n in alive)
                        GameManager.Instance.Characters[n].AddIsolation(-15);
                    RunDialog(DecisionNarration(
                            "Las despedidas son breves, casi verbales. Nadie promete llamar. Eso, entre vosotros, es una forma de misericordia."), EndChapter);
                    break;
                case 3:
                    foreach (var n in alive)
                        GameManager.Instance.Characters[n].AddIsolation(5);
                    RunDialog(DecisionNarration(
                            "La casa vuelve a cerrarse sobre vosotros como una mandíbula. La caza continúa… y el aislamiento también."), EndChapter);
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
