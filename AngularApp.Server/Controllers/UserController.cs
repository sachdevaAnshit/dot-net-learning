using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AngularApp.Server.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
            record City(string Name, long Population);
            record Country(string Name, double Area, long Population, List<City> Cities);

            // A traditional enumeration of some root vegetables.
            public enum SomeRootVegetables
            {
                HorseRadish,
                Radish,
                Turnip
            }

            // A bit field or flag enumeration of harvesting seasons.
            [Flags]
            public enum Seasons
            {
                None = 0,
                Summer = 1,
                Autumn = 2,
                Winter = 4,
                Spring = 8,
                All = Summer | Autumn | Winter | Spring
            }

            [HttpGet]
            [ProducesResponseType(StatusCodes.Status200OK)]
            public string[] GetUserList()
            {
            /*
            System.Diagnostics.Debug.WriteLine("Anshit sachdeva is here!!!");
            System.Diagnostics.Debug.WriteLine("Anshit sachdeva is here!!! ==> ", Constants.CountryId);
            System.Diagnostics.Debug.WriteLine("Anshit sachdeva is here!!! ==> ", Constants.StateId);

            // Hash table of when vegetables are available.
            Dictionary<SomeRootVegetables, Seasons> AvailableIn = new Dictionary<SomeRootVegetables, Seasons>();

            AvailableIn[SomeRootVegetables.HorseRadish] = Seasons.All;
            AvailableIn[SomeRootVegetables.Radish] = Seasons.Spring;
            AvailableIn[SomeRootVegetables.Turnip] = Seasons.Spring |
                    Seasons.Autumn;

            // Array of the seasons, using the enumeration.
            Seasons[] theSeasons = new Seasons[] { Seasons.Summer, Seasons.Autumn,
            Seasons.Winter, Seasons.Spring };

            System.Diagnostics.Debug.WriteLine("SomeRootVegetables.HorseRadish ==> ", SomeRootVegetables.HorseRadish);
            System.Diagnostics.Debug.WriteLine("AvailableIn ==> ", JsonSerializer.Serialize(AvailableIn));
            System.Diagnostics.Debug.WriteLine("theSeasons ==> ", JsonSerializer.Serialize(theSeasons));

            // Print information of what vegetables are available each season.
            foreach (Seasons season in theSeasons)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format(
                        "The following root vegetables are harvested in {0}:\n",
                        season.ToString("G")));
                    foreach (KeyValuePair<SomeRootVegetables, Seasons> item in AvailableIn)
                    {
                        // A bitwise comparison.
                        if (((Seasons)item.Value & season) > 0)
                            System.Diagnostics.Debug.WriteLine(String.Format("  {0:G}\n",
                                    (SomeRootVegetables)item.Key));
                    }
                }
            */


                List<int> numbers = [1, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20];

                IEnumerable<int> queryFactorsOfFour =
                    from num in numbers
                    where num % 4 == 0
                    select num;

                System.Diagnostics.Debug.WriteLine("queryFactorsOfFour");
                System.Diagnostics.Debug.WriteLine(queryFactorsOfFour);

                // Store the results in a new variable
                // without executing a foreach loop.
                var factorsofFourList = queryFactorsOfFour.ToList();
                
                // Read and write from the newly created list to demonstrate that it holds data.
                System.Diagnostics.Debug.WriteLine(factorsofFourList[2]);
                factorsofFourList[2] = 0;
                System.Diagnostics.Debug.WriteLine(factorsofFourList[2]);

                City[] cities = [
                        new City("Tokyo", 37_833_000),
                        new City("Delhi", 30_290_000),
                        new City("Shanghai", 27_110_000),
                        new City("São Paulo", 22_043_000),
                        new City("Mumbai", 20_412_000),
                        new City("Beijing", 20_384_000),
                        new City("Cairo", 18_772_000),
                        new City("Dhaka", 17_598_000),
                        new City("Osaka", 19_281_000),
                        new City("New York-Newark", 18_604_000),
                        new City("Karachi", 16_094_000),
                        new City("Chongqing", 15_872_000),
                        new City("Istanbul", 15_029_000),
                        new City("Buenos Aires", 15_024_000),
                        new City("Kolkata", 14_850_000),
                        new City("Lagos", 14_368_000),
                        new City("Kinshasa", 14_342_000),
                        new City("Manila", 13_923_000),
                        new City("Rio de Janeiro", 13_374_000),
                        new City("Tianjin", 13_215_000)
                ];
                Country[] countries = [
                    new Country ("Vatican City", 0.44, 526, [new City("Vatican City", 826)]),
                    new Country ("Monaco", 2.02, 38_000, [new City("Monte Carlo", 38_000)]),
                    new Country ("Nauru", 21, 10_900, [new City("Yaren", 1_100)]),
                    new Country ("Tuvalu", 26, 11_600, [new City("Funafuti", 6_200)]),
                    new Country ("San Marino", 61, 33_900, [new City("San Marino", 4_500)]),
                    new Country ("Liechtenstein", 160, 38_000, [new City("Vaduz", 5_200)]),
                    new Country ("Marshall Islands", 181, 58_000_000, [new City("Majuro", 28_000)]),
                    new Country ("Saint Kitts & Nevis", 261, 53_000, [new City("Basseterre", 13_000)])
                ];

                // where countryGroup.Key >= 20
                // percentileQuery is an IEnumerable<IGrouping<int, Country>>
                var percentileQuery =
                        from country in countries
                        let percentile = (int)country.Population / 10_000_000
                        group country by percentile into countryGroup
                        
                        orderby countryGroup.Key
                        select countryGroup;

                System.Diagnostics.Debug.WriteLine("percentileQuery");
                System.Diagnostics.Debug.WriteLine(percentileQuery);
                System.Diagnostics.Debug.WriteLine(percentileQuery.Count());

                // grouping is an IGrouping<int, Country>
                foreach (var grouping in percentileQuery)
                {
                    System.Diagnostics.Debug.WriteLine(grouping.Key);
                    foreach (var country in grouping)
                    {
                        System.Diagnostics.Debug.WriteLine(country.Name + ":" + country.Population);
                    }
                }


                System.Diagnostics.Debug.WriteLine("Anshit is here");
                return ["Anshit", "Sachdeva"];

            }

            [HttpGet]
            [Route("{id}")]
            // Route can be added at function level in order to append it to relative URL of the API
            public string GetUserById(int id, string? typeOfUser)
            {
                System.Diagnostics.Debug.WriteLine("id == ", JsonSerializer.Serialize(id));
                System.Diagnostics.Debug.WriteLine("typeOfUser== ", JsonSerializer.Serialize(typeOfUser));
                return "Get User by ID method with query param is running";
            }

            [HttpPost]
            // [Route("create")]
            // Route can be added at function level in order to append it to relative URL of the API
            public string CreateUser([FromBody] User user)
            {
                System.Diagnostics.Debug.WriteLine("user == ", JsonSerializer.Serialize(user));

                // var pathBase = HttpContext.Request.PathBase;
                var contextItems = HttpContext.Items;

                System.Diagnostics.Debug.WriteLine("contextItems == ", JsonSerializer.Serialize(contextItems));

                return "Post method is running";
            }

            [HttpPut]
            [Route("{id}")]
            // Route can be added at function level in order to append it to relative URL of the API
            public string EditUser(User user, int id)
            {
                System.Diagnostics.Debug.WriteLine("user == ", JsonSerializer.Serialize(user));
                System.Diagnostics.Debug.WriteLine("id == ", JsonSerializer.Serialize(id));
                return "Put method is running";
            }

    }
}
