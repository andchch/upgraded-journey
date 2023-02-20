namespace kursovaya;

public class AppSettings
{
    public float[] weights = new float[10];

    public AppSettings(INIManager manager)
    {
        weights[0] = float.Parse(manager.GetPrivateString("weights", "hasHPS"));
        weights[1] = float.Parse(manager.GetPrivateString("weights", "numOfADCChannels"));
        weights[2] = float.Parse(manager.GetPrivateString("weights", "voltage"));
        weights[3] = float.Parse(manager.GetPrivateString("weights", "hasDDR"));
        weights[4] = float.Parse(manager.GetPrivateString("weights", "numOfButtons"));
        weights[5] = float.Parse(manager.GetPrivateString("weights", "numOfSwitches"));
        weights[6] = float.Parse(manager.GetPrivateString("weights", "numOfLED"));
        weights[7] = float.Parse(manager.GetPrivateString("weights", "numOfGPIO"));
        weights[8] = float.Parse(manager.GetPrivateString("weights", "hasVGA"));
        weights[9] = float.Parse(manager.GetPrivateString("weights", "hasEth"));
    }

    public void UpdateWeights(INIManager manager)
    {
        weights[0] = float.Parse(manager.GetPrivateString("weights", "hasHPS"));
        weights[1] = float.Parse(manager.GetPrivateString("weights", "numOfADCChannels"));
        weights[2] = float.Parse(manager.GetPrivateString("weights", "voltage"));
        weights[3] = float.Parse(manager.GetPrivateString("weights", "hasDDR"));
        weights[4] = float.Parse(manager.GetPrivateString("weights", "numOfButtons"));
        weights[5] = float.Parse(manager.GetPrivateString("weights", "numOfSwitches"));
        weights[6] = float.Parse(manager.GetPrivateString("weights", "numOfLED"));
        weights[7] = float.Parse(manager.GetPrivateString("weights", "numOfGPIO"));
        weights[8] = float.Parse(manager.GetPrivateString("weights", "hasVGA"));
        weights[9] = float.Parse(manager.GetPrivateString("weights", "hasEth"));
    }
}