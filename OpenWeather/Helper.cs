using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWeather;

public static class Helper
{
    public static string GetInput(string input)
    {
        Console.Write(input);
        return Console.ReadLine();
    }
}
