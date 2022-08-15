namespace MenuSpace;

/// <summary>
/// Более не используется
/// </summary>
public class Collection
{
    public void ReSetRunner(ref List<Dictionary<string, Menu.Runner>[]> dict, int entry, int subEntry, string[] menuNames, Menu.Runner[] runner)
    {
        Dictionary<string, Menu.Runner>[] newDict = new Dictionary<string, Menu.Runner>[runner.Length];
        int i = 0;
        dict[entry - 1][subEntry].Clear();
        for (i = 0; i < runner.Length; i++)
        {
            dict[entry - 1][subEntry].Add(menuNames[i], runner[i]);
        }
    }
}