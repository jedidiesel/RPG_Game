using UnityEngine;

public static class InputUtility
{
    private const string leftTriggerName = "LTrigger";
    private const string rightTriggerName = "RTrigger";
    private static bool used;

    public static bool LTriggerPulled
    {
        get
        {
            if (!used && Input.GetAxis(leftTriggerName) > 0)
            {
                used = true;
                return true;
            }
            else if (used && Input.GetAxis(leftTriggerName) == 0)
            {
                used = false;
            }
            return false;
        }
    }

    public static bool RTriggerPulled
    {
        get
        {
            if (!used && Input.GetAxis(rightTriggerName) > 0)
            {
                used = true;
                return true;
            }
            else if (used && Input.GetAxis(rightTriggerName) == 0)
            {
                used = false;
            }
            return false;
        }
    }
}