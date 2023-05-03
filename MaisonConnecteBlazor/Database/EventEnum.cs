using MaisonConnecteBlazor.Misc;

namespace MaisonConnecteBlazor.Database
{
    public class EventEnum : StringEnum
    {
        public const string LightOn = "light_on";
        public const string LightOff = "light_off";
        public const string LEDColor = "led_color";
        public const string DoorStatusChanged = "door_status_changed";
    }
}
