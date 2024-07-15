using UnityEngine;

public class BetterDebug : Debug
{
    // Better version of Debug.Log()
    // Acts like a print function in Lua or Python and so can take as many arguments as possible
    // I believe in print superiority
    public static void Log(params object[] args)
    {
        string str = "";
        foreach (var arg in args)
        {
            str += arg.ToString() + "\t";
        }
        Debug.Log(str);
    }
}
