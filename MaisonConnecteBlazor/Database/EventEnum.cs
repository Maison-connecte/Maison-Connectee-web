using MaisonConnecteBlazor.Misc;

namespace MaisonConnecteBlazor.Database
{
    /// <summary>
    /// Classe héritant de StringEnum pour les évenements de statistiques
    /// </summary>
    public class EventEnum : StringEnum
    {
        public const string LumiereAllume = "light_on";
        public const string LumiereFerme = "light_off";
        public const string CouleurLED = "led_color";
        public const string StatusPorteChange = "door_status_changed";
    }
}
