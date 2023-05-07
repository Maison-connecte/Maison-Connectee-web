using MaisonConnecteBlazor.Misc;

namespace MaisonConnecteBlazor.Database
{
    /// <summary>
    /// Classe héritant de StringEnum pour les évenements de statistiques
    /// </summary>
    public class EventEnum : StringEnum
    {
        public const string LightOn = "light_on";
        public const string LightOff = "light_off";
        public const string LEDColor = "led_color";
        public const string DoorStatusChanged = "door_status_changed";
    }
}
