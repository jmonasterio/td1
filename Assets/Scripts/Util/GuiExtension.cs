using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    static public class GuiExtension
    {
        // Runtime draw text
        public static void GuiLabel( Vector3 vector, string s, Color color)
        {
            var orig = GUI.color;
            GUI.color = color;

            var screenVector = MapPositionToScreen(vector);
            var rc = new Rect(screenVector.x, screenVector.y, 100, 100);
            GUI.Label(rc, s);
            GUI.color = orig;
        }

        public static Vector3 MapPositionToScreen(Vector3 pos)
        {
            pos.y = -pos.y + 2.0f ;
            var pos2 = Camera.main.WorldToScreenPoint(pos);
            pos2.z = 0;
            return pos2;
        }

        // Design time draw text
        public static void HandleLabel(Vector3 vector, string s, Color color)
        {
            var orig = Handles.color;
            Handles.color = color;

            //var screenVector = MapPositionToScreen(vector);
            Handles.Label( vector, s);
            Handles.color = orig;
        }
    }
}