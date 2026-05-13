#if UNITY_EDITOR
namespace Simonshouse.EditorTools
{
    /// <summary>Textos e IDs de objetos Lobby v2.7 (usado por LobbySceneBuilder).</summary>
    internal static class LobbySceneV27Content
    {
        public const string FathersLetterAssetPath = "Assets/Resources/Items/item_fathers_letter.asset";

        public const string IdLibroVisitas = "libro_visitas";
        public const string IdFotoChimenea = "foto_chimenea";
        public const string IdPeriodico = "periodico";
        public const string IdCajonCarta = "cajon_carta";
        public const string IdAbrigo = "abrigo_perchero";
        public const string IdFotoGrupo = "foto_grupo_sangre";

        public static readonly string DescLibroVisitas =
            "Un libro de visitas encuadernado en cuero. Las páginas huelen a humedad y tinta vieja.\n\n"
            + "Las últimas tres entradas corresponden a los últimos seis meses. La penúltima entrada está tachada con tinta roja de forma agresiva, "
            + "como si la pluma se hubiera apoyado con demasiada fuerza. El nombre debajo de la tachadura es completamente ilegible.\n\n"
            + "La fecha de esa entrada: hace exactamente tres semanas.";

        public static readonly string DescFotografia =
            "Una fotografía en blanco y negro enmarcada en madera oscura.\n\n"
            + "Simón aparece joven, quizás dieciséis o diecisiete años, junto a un hombre mayor de rasgos severos y postura rígida. "
            + "El hombre tiene una mano apoyada en el hombro de Simón, pero el gesto no parece cariñoso; parece más bien el de alguien que marca una posesión.\n\n"
            + "Al dorso, escrito a lápiz con una letra ordenada y sin afecto:\n«Padre e hijo. 1987.»\n\n"
            + "Cuando examinas la fotografía, Robert, al otro lado de la sala, desvía la mirada.";

        public static readonly string DescPeriodico =
            "Un periódico local de hace tres días. Doblado con descuido, como si alguien lo hubiera dejado a medias.\n\n"
            + "La esquina de la sección de sucesos está marcada con una doblez intencional. El titular visible dice:\n\n"
            + "«INCENDIO EN ALMACÉN DEL PUERTO — INVESTIGACIÓN REABIERTA»\n\n"
            + "La nota debajo del titular menciona que el incendio original ocurrió hace dos años y que se habían archivado las diligencias por falta de evidencia. "
            + "La reapertura se produce «a raíz de nueva información proporcionada por un testigo anónimo».\n\n"
            + "Lisa, desde su sillón al otro lado de la sala, mira el periódico. Su expresión intenta ser indiferente y no lo consigue.";

        public static readonly string DescCajonCarta =
            "Un cajón de madera con el pomo de latón oxidado. No tiene llave.\n\n"
            + "Dentro: pañuelos de tela doblados con cuidado, un botón suelto, y debajo de todo, un sobre blanco sin dirección ni remitente. "
            + "El lacre en el cierre tiene una inicial grabada: «R».\n\n"
            + "El sobre pesa más de lo que debería para su tamaño.\n\n"
            + "[El sobre se guarda en el inventario.]";

        public static readonly string DescAbrigo =
            "Un abrigo de hombre de talla grande. Lana oscura, corte formal, ligeramente húmedo como si hubiera entrado con lluvia hace pocas horas.\n\n"
            + "Pero no ha llovido hoy.\n\n"
            + "En el bolsillo interior hay una nota doblada en cuatro. Sin sobre, sin firma. La letra es firme y decidida: no es la letra de alguien que escribe con miedo; es la de alguien que advierte.\n\n"
            + "«No confíes en nadie que llegue antes que tú.»\n\n"
            + "El abrigo no pertenece a ninguno de los cinco presentes. Ninguno llegó con lluvia. Ninguno lo reconoce.";

        public static readonly string DescFotoGrupo =
            "Una fotografía sobre la repisa de la chimenea. No estaba aquí antes.\n\n"
            + "Son todos ustedes. Cinco personas en el lobby de la mansión. La fotografía fue tomada hoy, desde adentro: el ángulo no pertenece a ninguno de los presentes.\n\n"
            + "Sobre el rostro de uno de los cinco, dibujada con algo que podría ser pintura pero que huele a cobre, hay una X.\n\n"
            + "Alguien sabía que iban a estar aquí. Alguien los fotografió sin que lo notaran. Y alguien ya eligió quién va primero.";
    }
}
#endif
