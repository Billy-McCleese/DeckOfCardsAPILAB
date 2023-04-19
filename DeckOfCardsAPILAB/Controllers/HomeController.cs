using DeckOfCardsAPILAB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace DeckOfCardsAPILAB.Controllers
{
    public class HomeController : Controller
    {
        public async Task<string> NewDeck()
        {
            string apiUrl = "https://deckofcardsapi.com/api/deck/new/shuffle/?deck_count=1";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JObject jsonObj = JObject.Parse(json);
                    return (string)jsonObj["deck_id"];
                }
            }
            return null;
        }


        public async Task<List<DrawResult>> DrawCards(string deckId)
        {
            string apiUrl = $"https://deckofcardsapi.com/api/deck/{deckId}/draw/?count=5";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JObject jsonObj = JObject.Parse(json);
                    JArray cards = (JArray)jsonObj["cards"];
                    List<DrawResult> results = new List<DrawResult>();
                    foreach (JToken card in cards)
                    {
                        results.Add(new DrawResult
                        {
                            Name = (string)card["value"] + " of " + (string)card["suit"],
                            ImageUrl = (string)card["image"]
                        });
                    }
                    return results;
                }
            }
            return null;
        }
        public async Task<ActionResult> Index()
        {
            string deckId = await NewDeck();
            List<DrawResult> results = await DrawCards(deckId);
            return View(results);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}