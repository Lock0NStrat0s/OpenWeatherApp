using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;

namespace OpenWeather;

public class Program
{
    static void Main(string[] args)
    {
        // configure appsettings.json 
        var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

        // obtain api key from appsettings.json
        string apiKey = config["api_key"];

        // read all data from city_list.json
        string cityData = File.ReadAllText(@"../../../city_list.json");

        // deserialize data into models
        List<CityModel> cityModelList = JsonConvert.DeserializeObject<List<CityModel>>(cityData);



        bool isRunning = true;
        do
        {
            UserInputWhichCountry(cityModelList);
        } while (isRunning);

        //HttpClient client = new HttpClient();

        //string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?id={city id}&appid={apiKey}";
    }

    private static void UserInputWhichCountry(List<CityModel>? cityModelList)
    {
        string input = Helper.GetInput($"Choose from the following:\n\n1: US\n2: Canada\n3: All Countries\n\nYour selection: ");

        switch (input)
        {
            case "1":
                PickUS_State(cityModelList);
                break;
            case "2":
                PickCanadianCity(cityModelList);
                break;
            default:
                AllCountries(cityModelList);
                break;
        }
    }

    private static void PickUS_State(List<CityModel>? cityModelList)
    {
        List<string> distinctStates = cityModelList.Select(x => x.State).Distinct().ToList();
        Console.WriteLine("Select from the following states:");

        int input = SelectIndexInList(distinctStates);
    }

    private static void PickCanadianCity(List<CityModel>? cityModelList)
    {
        throw new NotImplementedException();
    }

    private static void AllCountries(List<CityModel>? cityModelList)
    {
        // number of distinct countries in list
        List<string> distinctCountries = cityModelList.Select(x => x.Country).Distinct().ToList();

        // select country to display data from
        Console.WriteLine("Which country do you want current weather data from?");
        int input = SelectIndexInList(distinctCountries);
    }

    private static int SelectIndexInList(List<string> distinctList)
    {
        int input = 0;
        do
        {
            for (int i = 1; i <= distinctList.Count(); i++)
            {
                Console.Write($"{i}: {distinctList[i - 1]}, ");
                if (i % 3 == 0) Console.WriteLine();
            }

            int.TryParse(Helper.GetInput("Your selection: "), out input);

        } while (input < 1 || input > distinctList.Count());

        return input;
    }
}
