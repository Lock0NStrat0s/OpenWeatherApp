using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;

namespace OpenWeather;

public class Program
{
    public static string apiKey = "";
    static void Main(string[] args)
    {
        // configure appsettings.json 
        var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

        // obtain api key from appsettings.json
        apiKey = config["api_key"];

        bool isRunning = true;
        do
        {
            isRunning = UserInputWhichCountry();
        } while (isRunning);

        Console.Write("\nPress any key to continue: ");
        Console.ReadKey();
    }

    private static bool UserInputWhichCountry()
    {
        Console.Clear();
        // read all data from city_list.json
        string cityData = File.ReadAllText(@"../../../city_list.json");

        // deserialize data into models
        List<CityModel> cityModelList = JsonConvert.DeserializeObject<List<CityModel>>(cityData);

        // main menu select option
        string input = Helper.GetInput($"Choose from the following:\n\n1: US\n2: Canada\n3: All Countries\n\nYour selection: ");

        switch (input)
        {
            case "1":
                PickUS_State(cityModelList);
                break;
            case "2":
                var countryList = cityModelList.Where(x => x.Country == "CA").ToList();
                SelectCity(countryList);
                break;
            case "3":
                AllCountries(cityModelList);
                break;
            default:
                return false;
        }

        return true;
    }

    private static void PickUS_State(List<CityModel>? cityModelList)
    {
        // distinct states in list
        List<string> distinctStates = cityModelList.Select(x => x.State).Distinct().ToList();
        distinctStates.RemoveAll(x => x == "");
        distinctStates.RemoveAll(x => x == "00");

        // select state to display data from
        Console.WriteLine("Select from the following states:");
        string stateName = GetNameInList(distinctStates);

        // create list of CityModel containing only specified country
        List<CityModel> cityList = cityModelList.Where(x => x.State == stateName).ToList();

        // get desired city
        SelectCity(cityList);
    }

    private static void AllCountries(List<CityModel>? cityModelList)
    {
        // distinct countries in list
        List<string> distinctCountries = cityModelList.Select(x => x.Country).Distinct().ToList();

        // select country to display data from
        Console.WriteLine("Which country do you want current weather data from?");
        string countryCode = GetNameInList(distinctCountries);

        // create list of CityModel containing only specified country
        List<CityModel> cityList = cityModelList.Where(x => x.Country == countryCode).ToList();

        // get desired city
        SelectCity(cityList);
    }

    private static void SelectCity(List<CityModel>? cityList)
    {
        Console.Clear();
        // select city to display data from
        Console.WriteLine($"Which city in {cityList[0].Country} do you want current weather data from?");
        CityModel cityModel = GetCityName(cityList);

        CallApi(cityModel);
    }

    private static void CallApi(CityModel cityModel)
    {
        HttpClient client = new HttpClient();

        string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={cityModel.Name},{cityModel.Country}&appid={apiKey}&units=metric";

        var response = client.GetStringAsync(apiUrl).Result;
        WeatherModel weather = JsonConvert.DeserializeObject<WeatherModel>(response);

        Console.ForegroundColor = ConsoleColor.Red;

        Console.WriteLine($"\n\n\nCity: {weather.Name}\nCountry: {weather.Sys.Country}\nCurrent Temperature: {weather.Main.Temp}\nMax Temperature: {weather.Main.TempMax}, Min temparature: {weather.Main.TempMin}\n\nWeather:\nMain: {weather.Weather[0].Main}\nDescription: {weather.Weather[0].Description}\n\n");

        Console.ForegroundColor = ConsoleColor.White;

        Console.Write("\nPress any key to continue: ");
        Console.ReadKey();
    }

    private static string GetNameInList(List<string> distinctList)
    {
        int input = 0;
        do
        {
            for (int i = 1; i <= distinctList.Count(); i++)
            {
                Console.Write($"{i}: {distinctList[i - 1]}, ");
                if (i % 3 == 0) Console.WriteLine();
            }

            int.TryParse(Helper.GetInput("\nYour selection: "), out input);

        } while (input < 1 || input > distinctList.Count());

        return distinctList[input - 1];
    }

    public static CityModel GetCityName(List<CityModel> distinctCityList)
    {
        int input = 0;
        do
        {
            for (int i = 1; i <= distinctCityList.Count(); i++)
            {
                Console.Write($"{i}: {distinctCityList[i - 1].Name}, ");
                if (i % 3 == 0) Console.WriteLine();
            }

            int.TryParse(Helper.GetInput("\nYour selection: "), out input);

        } while (input < 1 || input > distinctCityList.Count());

        return distinctCityList[input - 1];
    }
}
