#if UNITY_EDITOR
namespace Simonshouse.EditorTools
{
    /// <summary>Líneas C1 Lobby v2.8 (orden: narración / personaje alternados según spec).</summary>
    internal static class LobbySceneV28Dialogues
    {
        public static readonly (string text, bool isNarration)[] Robert =
        {
            ("Robert está de pie junto a la ventana. Su postura es rígida, como la de alguien que lleva mucho tiempo de pie en lugares donde no quiere estar.", true),
            ("Vine porque me lo pidieron. Un abogado —no sé quién lo contrató— me envió una carta diciéndome que había asuntos pendientes relacionados con la herencia de Simón que requerían mi presencia.", false),
            ("Hace una pausa. Su postura es más rígida de lo que debería ser para alguien que simplemente está descansando.", true),
            ("No lo conocía bien. Nos cruzamos en algunos círculos. Nada más.", false),
            ("Lo dice con demasiada calma. La clase de calma que se ensaya frente al espejo antes de salir de casa.", true)
        };

        public static readonly (string text, bool isNarration)[] Ana =
        {
            ("Ana examina cada objeto de la sala con una mirada que lo tasa todo, incluso sin querer.", true),
            ("Era mi cliente. Uno de los mejores que he tenido, honestamente. Cuando me dijeron que había muerto...", false),
            ("Deja la frase en el aire. Sus ojos siguen recorriendo la sala mientras habla.", true),
            ("Supongo que vine por respeto. Y porque alguien tiene que asegurarse de que su obra quede bien catalogada. No es una colección menor.", false),
            ("Sonríe brevemente. Es la sonrisa de alguien que sabe exactamente qué impresión está produciendo.", true)
        };

        public static readonly (string text, bool isNarration)[] Ben =
        {
            ("Ben está de pie pero parece incapaz de quedarse quieto del todo. Su peso se desplaza de un pie al otro, imperceptiblemente, como alguien que espera que suene una alarma.", true),
            ("Simón y yo éramos socios, de cierta forma. Él pintaba, yo me encargaba del lado financiero. Muy informal, ya sabe. Sin contratos de por medio. La gente creativa suele preferirlo así.", false),
            ("Saca un reloj de bolsillo, lo mira, lo guarda. No es para consultar la hora.", true),
            ("La verdad es que me enteré de su muerte y quise venir a... no sé, despedirme. Algo así.", false),
            ("Hay algo en su voz que no alcanza a esconder. Urgencia disfrazada de nostalgia.", true)
        };

        public static readonly (string text, bool isNarration)[] Lisa =
        {
            ("Lisa ha elegido el sillón más cercano a la puerta de entrada. No es un detalle accidental.", true),
            ("Lo conocí en una exposición. Hace tres años, creo. Éramos amigos. Buenos amigos.", false),
            ("Mira hacia la escalera mientras habla. Como si esperara que alguien bajara.", true),
            ("¿Alguien sabe qué pasó exactamente? La noticia fue muy vaga. Muerte repentina, dicen. Pero eso no significa nada.", false),
            ("Lo dice como quien ya tiene una teoría y solo busca que alguien la confirme sin darse cuenta de que lo está haciendo.", true)
        };

        public static readonly (string text, bool isNarration)[] Lucas =
        {
            ("Lucas lleva su maletín de cuero gastado como si contuviera algo frágil o algo comprometedor. Probablemente ambas cosas.", true),
            ("Yo trabajé para él. Ayudante de estudio, básicamente. Limpiaba pinceles, preparaba lienzos, a veces mezclaba pigmentos.", false),
            ("Una pausa. Mira al suelo.", true),
            ("Era muy exigente. Pero aprendí más con él que en tres años de facultad. No sé por qué vine. Supongo que quería ver el lugar una última vez.", false),
            ("Lo dice mirando al suelo. Algo en su postura sugiere que hay mucho más detrás de esas palabras. Mucho más que no tiene ninguna intención de decir.", true)
        };
    }
}
#endif
