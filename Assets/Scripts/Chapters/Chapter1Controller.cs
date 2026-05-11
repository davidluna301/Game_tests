using System.Collections.Generic;
using Simonshouse.UI;
using TMPro;
using UnityEngine;

namespace Simonshouse.Chapters
{
    public class Chapter1Controller : ChapterController
    {
        private readonly HashSet<string> exploredItems = new();

        [Header("Exploración — Lobby")]
        [SerializeField] private TextMeshProUGUI textRoomName;
        [SerializeField] private TextMeshProUGUI textRoomDesc;

        [Header("Interacciones / decisión")]
        [SerializeField] private TextMeshProUGUI textInteractionsTitle;
        [SerializeField] private TextMeshProUGUI textDecisionPrompt;

        protected override int GetChapterNumber() => 1;

        protected override void Start()
        {
            base.Start();
            GameManager.Instance?.EnterRoom("Lobby");
            BindLobbyLabels();
        }

        private void BindLobbyLabels()
        {
            if (textRoomName != null)
                textRoomName.text = "EL LOBBY";
            if (textRoomDesc != null)
            {
                textRoomDesc.text =
                    "Un vestíbulo alto, demasiado silencioso. El reloj de pared marca las tres en punto. "
                    + "La luz entra tamizada por cortinas polvorientas; el mármol refleja sombras alargadas. "
                    + "Cinco maletas, cinco miradas que se evitan y cinco razones distintas para estar aquí.";
            }

            if (textInteractionsTitle != null)
                textInteractionsTitle.text = "¿Con quién hablas?";

            if (textDecisionPrompt != null)
            {
                textDecisionPrompt.text =
                    "El grupo aguarda una propuesta. Cómo se organice la primera incursión en la mansión "
                    + "marcará el tono de la noche.";
            }
        }

        protected override void LoadExploration()
        {
            PrepareExplorationOptions();
            ClearOptions();

            if (!exploredItems.Contains("libro"))
                SpawnOptionButton("Examinar el libro de visitas sobre la mesita",
                    () => ExploreItem_Lobby_Libro());
            if (!exploredItems.Contains("abrigo"))
                SpawnOptionButton("Revisar el abrigo en el perchero",
                    () => ExploreItem_Lobby_Abrigo());
            if (!exploredItems.Contains("foto"))
                SpawnOptionButton("Mirar la fotografía sobre la chimenea",
                    () => ExploreItem_Lobby_Foto());
            if (!exploredItems.Contains("periodico"))
                SpawnOptionButton("Leer el periódico doblado en el sillón",
                    () => ExploreItem_Lobby_Periodico());

            SpawnOptionButton("Terminar de explorar → Hablar con el grupo",
                GoToInteractions);
        }

        private void ExploreItem_Lobby_Libro()
        {
            if (GameManager.Instance == null) return;
            exploredItems.Add("libro");
            GameManager.Instance.AddClue("entry_crossed");

            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "Sobre la mesita de entrada hay un libro de visitas. Las últimas tres entradas son de los últimos seis meses. "
                            + "Una de ellas está tachada con tinta roja. El nombre debajo de la tachadura es ilegible, "
                            + "pero la fecha es de hace exactamente tres semanas."
                    }
                }
            };
            RunDialog(chain, LoadExploration);
        }

        private void ExploreItem_Lobby_Abrigo()
        {
            if (GameManager.Instance == null) return;
            exploredItems.Add("abrigo");
            GameManager.Instance.AddClue("note_coat");

            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "Un abrigo de hombre de talla grande. En el bolsillo interior hay una nota manuscrita, sin firma:"
                    },
                    new() { speakerName = "NOTA", content = "No confíes en nadie que llegue antes que tú." },
                    new()
                    {
                        isNarration = true,
                        content =
                            "La letra es firme, decidida. No es la letra de alguien que escribe con miedo. Es la de alguien que advierte."
                    }
                }
            };
            RunDialog(chain, LoadExploration);
        }

        private void ExploreItem_Lobby_Foto()
        {
            if (GameManager.Instance?.Characters == null) return;
            exploredItems.Add("foto");
            GameManager.Instance.AddClue("photo_father_son");
            GameManager.Instance.Characters["Robert"].AddIsolation(10);

            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "Una fotografía en blanco y negro de Simón joven, junto a un hombre mayor de rasgos severos. Al dorso, escrito a lápiz:"
                    },
                    new() { speakerName = "DORSO", content = "Padre e hijo. 1987." },
                    new()
                    {
                        isNarration = true,
                        content =
                            "Robert, al otro lado de la sala, desvía la mirada cuando notas que examinas la fotografía."
                    }
                }
            };
            RunDialog(chain, LoadExploration);
        }

        private void ExploreItem_Lobby_Periodico()
        {
            if (GameManager.Instance?.Characters == null) return;
            exploredItems.Add("periodico");
            GameManager.Instance.AddClue("fire_newspaper");
            GameManager.Instance.Characters["Lisa"].Connect(10);

            var chain = new DialogChain
            {
                characterName = "",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        isNarration = true,
                        content =
                            "Un periódico local de hace tres días. La esquina de la página de sucesos está doblada. El titular visible dice:"
                    },
                    new()
                    {
                        speakerName = "TITULAR",
                        content = "Incendio en almacén del puerto, investigación reabierta."
                    },
                    new()
                    {
                        isNarration = true,
                        content =
                            "Lisa, desde su sillón, mira el periódico con una expresión que intenta ser indiferente y no lo consigue."
                    }
                }
            };
            RunDialog(chain, LoadExploration);
        }

        protected override void LoadInteractions()
        {
            PrepareInteractionsOptions();
            ClearOptions();
            if (GameManager.Instance?.Characters == null)
                return;

            var alive = GameManager.Instance.GetAliveCharacters();

            foreach (var name in alive)
            {
                var captured = name;
                var charData = GameManager.Instance.Characters[name];
                SpawnOptionButton($"Hablar con {name}  —  {charData.Role}",
                    () => TalkTo(captured));
            }

            SpawnOptionButton("No hablar con nadie más → Tomar decisión",
                GoToDecision);
        }

        private void TalkTo(string name)
        {
            var chain = BuildDialogChainC1(name);
            RunDialog(chain, LoadInteractions);
        }

        private static DialogChain BuildDialogChainC1(string name) => name switch
        {
            "Robert" => new DialogChain
            {
                chainId = "C1_Robert",
                characterName = "Robert",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "ROBERT",
                        content =
                            "Vine porque me lo pidieron. Un abogado, no sé quién lo contrató, me envió una carta diciéndome que había asuntos pendientes relacionados con la herencia de Simón que requerían mi presencia."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Hace una pausa. Su postura es rígida, ensayada."
                    },
                    new()
                    {
                        speakerName = "ROBERT",
                        content = "No lo conocía bien. Nos cruzamos en algunos círculos. Nada más."
                    },
                    new()
                    {
                        isNarration = true,
                        content =
                            "Lo dice con demasiada calma. La clase de calma que se ensaya frente al espejo antes de salir de casa."
                    }
                }
            },

            "Ana" => new DialogChain
            {
                chainId = "C1_Ana",
                characterName = "Ana",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "ANA",
                        content =
                            "Era mi cliente. Uno de los mejores que he tenido, honestamente. Cuando me dijeron que había muerto..."
                    },
                    new() { isNarration = true, content = "Deja la frase en el aire." },
                    new()
                    {
                        speakerName = "ANA",
                        content =
                            "Supongo que vine por respeto. Y porque alguien tiene que asegurarse de que su obra quede bien catalogada. No es una colección menor."
                    },
                    new()
                    {
                        isNarration = true,
                        content =
                            "Sonríe brevemente. Sus ojos recorren la sala como tasando cada objeto visible."
                    }
                }
            },

            "Ben" => new DialogChain
            {
                chainId = "C1_Ben",
                characterName = "Ben",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "BEN",
                        content =
                            "Simón y yo éramos socios, de cierta forma. Él pintaba, yo me encargaba del lado financiero. Muy informal, ya sabe. Sin contratos de por medio."
                    },
                    new()
                    {
                        isNarration = true,
                        content = "Saca un reloj de bolsillo, lo mira, lo guarda."
                    },
                    new()
                    {
                        speakerName = "BEN",
                        content =
                            "La verdad es que me enteré de su muerte y quise venir a... no sé, despedirme. Algo así."
                    },
                    new()
                    {
                        isNarration = true,
                        content =
                            "Hay algo en su voz que no alcanza a esconder. Urgencia disfrazada de nostalgia."
                    }
                }
            },

            "Lisa" => new DialogChain
            {
                chainId = "C1_Lisa",
                characterName = "Lisa",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "LISA",
                        content =
                            "Lo conocí en una exposición. Hace tres años, creo. Éramos amigos. Buenos amigos."
                    },
                    new() { isNarration = true, content = "Mira hacia la escalera." },
                    new()
                    {
                        speakerName = "LISA",
                        content =
                            "¿Alguien sabe qué pasó exactamente? La noticia fue muy vaga. Muerte repentina, dicen. Pero eso no significa nada."
                    },
                    new()
                    {
                        isNarration = true,
                        content =
                            "Lo dice como quien ya tiene una teoría y busca que alguien la confirme sin saberlo."
                    }
                }
            },

            "Lucas" => new DialogChain
            {
                chainId = "C1_Lucas",
                characterName = "Lucas",
                lines = new List<DialogLine>
                {
                    new()
                    {
                        speakerName = "LUCAS",
                        content =
                            "Yo trabajé para él. Ayudante de estudio, básicamente. Limpiaba pinceles, preparaba lienzos, a veces mezclaba pigmentos."
                    },
                    new() { isNarration = true, content = "Una pausa." },
                    new()
                    {
                        speakerName = "LUCAS",
                        content =
                            "Era muy exigente. Pero aprendí más con él que en tres años de facultad. No sé por qué vine. Supongo que quería ver el lugar una última vez."
                    },
                    new()
                    {
                        isNarration = true,
                        content =
                            "Lo dice mirando al suelo. Algo en su postura sugiere que hay mucho más detrás de esas palabras."
                    }
                }
            },

            _ => new DialogChain { lines = new List<DialogLine>() }
        };

        protected override void LoadDecision()
        {
            PrepareDecisionOptions();
            ClearOptions();

            SpawnOptionButton("[1] Proponer explorar la mansión juntos, como grupo",
                () => ApplyDecision(1));
            SpawnOptionButton("[2] Sugerir que cada uno explore por su cuenta",
                () => ApplyDecision(2));
            SpawnOptionButton("[3] Intentar que el grupo hable abiertamente sobre Simón",
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
                    GameManager.Instance.AddDecision("grupo_unido_c1");
                    foreach (var n in alive)
                        GameManager.Instance.Characters[n].Connect(10);
                    var chainD1 = new DialogChain
                    {
                        characterName = "",
                        lines = new List<DialogLine>
                        {
                            new()
                            {
                                isNarration = true,
                                content =
                                    "El grupo acepta, aunque con reservas. Hay algo reconfortante en moverse juntos por una casa que ninguno conoce del todo. Los pasos de cinco personas suenan distintos a los de una sola."
                            }
                        }
                    };
                    RunDialog(chainD1, EndChapter);
                    break;

                case 2:
                    GameManager.Instance.AddDecision("separados_c1");
                    foreach (var n in alive)
                        GameManager.Instance.Characters[n].AddIsolation(15);
                    var chainD2 = new DialogChain
                    {
                        characterName = "",
                        lines = new List<DialogLine>
                        {
                            new()
                            {
                                isNarration = true,
                                content =
                                    "Cada uno toma una dirección distinta. La mansión los absorbe en silencio. La distancia entre ellos crece con cada paso. Desde algún lugar de la casa, alguien observa cómo se separan."
                            }
                        }
                    };
                    RunDialog(chainD2, EndChapter);
                    break;

                case 3:
                    GameManager.Instance.AddDecision("hablar_c1");
                    foreach (var n in alive)
                        GameManager.Instance.Characters[n].Connect(5);
                    var chainD3 = new DialogChain
                    {
                        characterName = "",
                        lines = new List<DialogLine>
                        {
                            new()
                            {
                                isNarration = true,
                                content =
                                    "Las respuestas son vagas, calculadas. Pero en los silencios entre las palabras hay más información que en las palabras mismas. Algo se mueve debajo de la superficie de cada frase."
                            }
                        }
                    };
                    RunDialog(chainD3, EndChapter);
                    break;
            }
        }
    }
}
