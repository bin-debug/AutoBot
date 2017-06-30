using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;
using Api.Models;
using Api.Repositories;
using Api.Utils;

namespace Api.Repositories
{
    public class AutoTraderRepository : ICarService
    {
        /// <summary>
        /// AutoTrader does not seem to have a public api so I had to construct my own api to query the site programmatically.
        /// </summary>
        /// <param name="query">QueryRequest object sent from the controller.</param>
        /// <returns>A List of cars</returns>
        public List<Car> GetCars(QueryRequest query)
        {
            if (query == null)
                return null;

            string url = "";
            var web = new HtmlWeb();
            
            var cars = new List<Car>();

            if (!string.IsNullOrEmpty(query.Model))
                url = Helper.BuildURL(query);
            else
                url = "http://www.autotrader.co.za/makemodel/make/BMW/model/1%20SERIES/search";

            var doc = web.Load(url);

            bool rows = doc.DocumentNode.SelectSingleNode("//div[@class='result-list row']//div[@class='col-xs-12']") != null;
            if (rows == false)
                return null;

            var html = doc.DocumentNode.SelectNodes("//div[@class='result-list row']//div[@class='col-xs-12']").ToList();

            cars = ProcessHtml(html);

            return cars;
        }

        private List<Car> ProcessHtml(List<HtmlNode> html)
        {
            if (html.Count == 0)
                return null;

            var cars = new List<Car>();

            foreach (var item in html)
            {
                var car = new Car();
                var imageList = new List<string>();

                string description = item.SelectSingleNode(".//a[@class='listing-title xs-2-lines']").InnerText;
                string year = description.Substring(0, 4);
                string modelName = description.Replace("BMW", "").Replace(year, "").Trim();

                var link = item
                  .Descendants("a")
                  .First(x => x.Attributes["class"] != null
                           && x.Attributes["class"].Value == "mobile-cav-link visible-xs-block");
                string hrefValue = link.Attributes["href"].Value;

                var imageNodes = item.SelectNodes(".//a[@class='cav-link']").ToList();
                if (imageNodes.Count > 1)
                {
                    imageNodes[1].ChildNodes.ToList().ForEach(c => imageList.Add(c.OuterHtml));
                    if (imageList.Count > 0)
                        imageList.ForEach(i => car.Images.Add(new CarImage { Image = Helper.GetImageURL(i) }));
                }

                bool priceExist = item.SelectSingleNode(".//div[@class='price price col-xs-4 col-sm-3 col-middle text-right no-padding']") != null;
                if (priceExist)
                    car.Price = item.SelectSingleNode(".//div[@class='price price col-xs-4 col-sm-3 col-middle text-right no-padding']").InnerText;
                else
                    car.Price = item.SelectSingleNode(".//div[@class='price col-xs-4 col-sm-3 text-right no-padding']").InnerText;

                car.Year = year;
                car.Mileage = GetNodeDetails(item, ".//span[@class='mileage']");
                car.Model = modelName;
                car.Engine = GetNodeDetails(item, ".//span[@class='engine-capacity']");
                car.Description = GetNodeDetails(item, ".//div[@class='sellers-comment module']"); ;
                car.URL = hrefValue;

                cars.Add(car);
            }

            return cars;
        }

        private string GetNodeDetails(HtmlNode node, string element)
        {
            if(node == null || string.IsNullOrEmpty(element))
                return "No further description specified.";

            bool nodeExists = node.SelectSingleNode(element) != null;
            if (nodeExists)
                return node.SelectSingleNode(element).InnerText;
            else
                return "No further description specified.";
        }

    }
    
}
