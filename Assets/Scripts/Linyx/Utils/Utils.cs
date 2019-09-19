using MaterialUI;
using UnityEngine;

namespace Linyx.Utils
{
    public static class Utils
    {
        public static void ToggleOpenButton(this MaterialButton button, bool isOpen)
        {
            button.iconVectorImageData = (isOpen) ?
                MaterialUIIconHelper.GetIcon("Material Design Icons", MaterialIconEnum.KEYBOARD_ARROW_DOWN).vectorImageData :
                MaterialUIIconHelper.GetIcon("Material Design Icons", MaterialIconEnum.KEYBOARD_ARROW_RIGHT).vectorImageData;
        }

        public static string FromSecondsToMinutesAndSeconds(float seconds)
        {
            int sec = (int)(seconds % 60f);
            int min = (int)((seconds / 60f) % 60f);

            string minSec = min.ToString("D2") + ":" + sec.ToString("D2");
            return minSec;
        }

        
    }
}
