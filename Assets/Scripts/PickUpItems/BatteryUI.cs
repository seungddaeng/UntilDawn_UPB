using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [Header("Barras de batería")]
    public Image[] batteryBars;

    [Header("Colores")]
    public Color fullColor = Color.green;
    public Color mediumColor = new Color(1f, 0.5f, 0f);
    public Color lowColor = Color.red;
    public Color emptyColor = Color.gray;

    public void UpdateBatteryUI(int currentBatteries)
    {
        Color activeColor = GetBatteryColor(currentBatteries);

        for (int i = 0; i < batteryBars.Length; i++)
        {
            if (batteryBars[i] == null)
            {
                continue;
            }

            if (i < currentBatteries)
            {
                batteryBars[i].color = activeColor;
            }
            else
            {
                batteryBars[i].color = emptyColor;
            }
        }
    }

    private Color GetBatteryColor(int currentBatteries)
    {
        if (currentBatteries >= 3)
        {
            return fullColor;
        }

        if (currentBatteries == 2)
        {
            return mediumColor;
        }

        if (currentBatteries == 1)
        {
            return lowColor;
        }

        return emptyColor;
    }
}