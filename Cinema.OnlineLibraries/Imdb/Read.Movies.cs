using Cinema.OnlineLibraries.Helper;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.OnlineLibraries.Data;

namespace Cinema.OnlineLibraries.Imdb
{
  public partial class Read
  {
    public static async Task<List<Movie>> Movies(string country, string postalCode, string day = null)
    {
      var list = new List<Movie>();

      var url = "https://www.imdb.com/showtimes/location/" + country + "/" + postalCode + "?ref_=shlc_dt";
      if (day != null)
      {
        url = "https://www.imdb.com/showtimes/location/" + country + "/" + postalCode + "/" + day + "?ref_=shlc_dt";
      }

      var doc = Http.GetHtmlDocument(url);
      var lister = doc.DocumentNode.SelectSingleNode("//*[@class=\"lister list detail sub-list\"]");

      IEnumerable<HtmlNode> items = lister.SelectNodes(".//*[@class=\"lister-item mode-grid\"]");
      foreach (var item in items)
      {
        var movie = new Movie();

        //var p1 = item.SelectSingleNode(".//*[@class=\"hidden inline-sort-params\"]");
        //foreach (var childNode in p1.ChildNodes)
        //  if (childNode.Name == "span")
        //  {
        //    if (childNode.GetAttributeValue("name", "") == "alpha")
        //      movie.Title = childNode.GetAttributeValue("data-value", "");

        //    if (childNode.GetAttributeValue("name", "") == "release_date")
        //      movie.Runtime = childNode.GetAttributeValue("data-value", "");

        //    if (childNode.GetAttributeValue("name", "") == "runtime")
        //      movie.Release = childNode.GetAttributeValue("data-value", "");
        //  }

        var p2 = item.SelectSingleNode(".//*[@class=\"lister-item-image ribbonize\"]");
        movie.ImdbId = p2.GetAttributeValue("data-tconst", "");

        //var p3 = p2.SelectSingleNode(".//img[@class=\"loadlate\"]");
        //movie.CoverUrl = p3.GetAttributeValue("loadlate", "");

        var p4 = item.SelectSingleNode(".//*[@class=\"lister-item-content\"]");
        //var p5 = p4.SelectSingleNode(".//*[@class=\"genre\"]");
        //movie.Genres = p5.InnerText.Replace("\n", "").Trim();

        var p6 = p4.SelectSingleNode(".//*[@class=\"certificate\"]");
        if (p6 == null)
        {
          movie.Age = "0";
        }
        else
        {
          movie.Age = p6.InnerText.Replace("\n", "").Trim();
        }

        list.Add(movie);
      }

      return list;
    }
  }
}
