using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Api.Utils
{
    public static class Helper
    {
        /// <summary>
        /// Extract the image source
        /// </summary>
        /// <param name="image"></param>
        /// <returns>Image src</returns>
        public static string GetImageURL(string image)
        {
            int startPos = image.LastIndexOf("smart-img class='thumb - img' src=") + "smart-img class='thumb - img' src=".Length + 1;
            int length = image.IndexOf("></smart-img>") - startPos;
            string sub = image.Substring(startPos, length).Replace("''", "");
            return sub.TrimEnd().Substring(0, sub.Length - 1);
        }

        /// <summary>
        /// Build up the query url to get results from autotrader.
        /// </summary>
        /// <param name="query">QueryRequest object</param>
        /// <returns>URI</returns>
        public static string BuildURL(QueryRequest query)
        {
            // the url is case sensitive.

            var url = new StringBuilder();

            // base url
            url.Append("http://www.autotrader.co.za/makemodel/make/BMW");

            // append model to the url
            if (query.Model.ToLower().StartsWith("m"))
                url.Append($"/model/{query.Model.Replace(" ", "")}");
            else
            {
                var series = Regex.Replace(query.Model, "[^0-9]", "");
                url.Append($"/model/{series}%20SERIES");
            }

            // append from year to the url
            url.Append($"/year/more-than-{query.Year}");

            // append mileage to the url
            url.Append($"/mileage/more-than-{query.Milage}");

            // append colour to the url
            if (query.Colour.ToLower() != "any")
                url.Append($"/colour/{query.Colour}");

            // append gearbox to the url
            if (query.GearBox.ToLower() != "any")
                url.Append($"/transmission/{query.GearBox}");

            // append fuel to the url
            if (query.Fuel.ToLower() != "any")
                url.Append($"/fueltype/{query.Fuel}");

            url.Append("/search");

            return url.ToString();
        }
    }
}
