using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Configuration;
using AutoBotFramework.Models;

namespace AutoBotFramework.Forms
{

    [Serializable]
    public enum Model
    {
        Bmw1Series = 1,
        Bmw2Series = 2,
        Bmw3Series = 3,
        //Bmw4Series = 4,
        //M2 = 5,
        //M3 = 6,
        //M4 = 7,
        //M5 = 8
    }

    [Serializable]
    public enum Transmission
    {
        Automatic = 1,
        Manual = 2,
        Any = 3
    }

    [Serializable]
    public enum Fuel
    {
        Petrol = 1,
        Diesal = 2,
        Any = 3
    }

    [Serializable]
    public enum Colour
    {
        Black = 1,
        Blue = 2,
        Gold = 3,
        Grey = 4,
        Red = 5,
        Silver = 6,
        White = 7,
        Any = 8
    }

    [Serializable]
    public class CarForm
    {
        static HttpClient client;
        static List<Car> carsToDisplay = new List<Car>();

        public CarForm()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("http://autobot-api.azurewebsites.net");
            //client.BaseAddress = new Uri("http://localhost:51212");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [Prompt("Please select one of the following executive models we have on special. {||}")]
        public Model Model;

        [Prompt("Which model would you be interested in for the {Model}? (Example 2012). {||}")]
        public int MiniumYear;

        [Prompt("What kind of transmission would you prefer on the {Model}? {||}")]
        public Transmission Transmission;

        [Prompt("Interesting fact : Petrol engines are cheaper to maintain and repair but diesels last longer. What would you prefer for your {Model}? {||}")]
        public Fuel Fuel;

        [Prompt("BMW Approved Used Cars provide peace of mind motoring, whats your prefered mileage? (Example 1000)")]
        public int Mileage;

        [Prompt("What colour would you like your {Model} to be? {||}")]
        public Colour Colour;

        public static IForm<CarForm> BuildForm()
        {
            return new FormBuilder<CarForm>()
                    .Message("Welcome to autobot! Let me try and help you find the perfect BMW deal.")
                    .Field(nameof(Model))
                    .Field(nameof(MiniumYear), validate: ValidateMiniumYear)
                    .Field(nameof(Transmission))
                    .Field(nameof(Fuel))
                    .Field(nameof(Mileage), validate: ValidateMilage)
                    .Field(nameof(Colour))
                    //.Field(nameof(MiniumPrice), validate: ValidateMiniumPrice)
                    //.Field(nameof(Email), validate: ValidateEmailAddress)
                    .Message("Okay...Il be right back with the results.")
                    .OnCompletion(async (context, profileForm) =>
                    {
                        var reply = context.MakeMessage();
                        string request = $"/api/car?model={profileForm.Model}&year={profileForm.MiniumYear}&Milage={profileForm.Mileage}&GearBox={profileForm.Transmission}&Fuel={profileForm.Fuel}&Colour={profileForm.Colour}";

                        var carsFromApi = await GetCarsAsync(request);
                        if (carsFromApi != null && carsFromApi.Count > 0)
                        {
                            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                            reply.Attachments = GetCards(carsFromApi);
                            await context.PostAsync(reply);
                        }
                        else
                        {
                            await context.PostAsync($"I am sorry, we could not find any results for the specified model {profileForm.MiniumYear} {profileForm.Model}");
                        }
                    })
                    .Build();
        }

        #region Validation

        private static Task<ValidateResult> ValidateMiniumYear(CarForm state, object response)
        {
            var result = new ValidateResult { IsValid = true, Value = response };
            int val = Convert.ToInt32(response);
            if (val < 2012 || val > DateTime.Now.Year)
            {
                result.Feedback = $"The only vehicles we have on offer are from  2012 to {DateTime.Now.Year}";
                result.IsValid = false;
            }
            return Task.FromResult(result);
        }

        private static Task<ValidateResult> ValidateMilage(CarForm state, object response)
        {
            var result = new ValidateResult { IsValid = true, Value = response };
            int val = Convert.ToInt32(response);
            if(val >= 1000 && val <= 25000)
            {
                result.Feedback = $"We try to keep low mileage vehicles and they range from 1000kms - 25000kms";
                result.IsValid = false;
            }
            return Task.FromResult(result);
        }

        #endregion

        #region Service

        private static async Task<List<Car>> GetCarsAsync(string path)
        {
            List<Car> car = new List<Car>();
            HttpResponseMessage response = await client.GetAsync(path);
            var r = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<Car>>(r);
            return result;
        }

        #endregion

        #region Cards

        private static List<Attachment> GetCards(List<Car> carsFromApi)
        {
            var result = new List<Attachment>();

            var cars = carsFromApi.Where(c => c.Images.Count > 0)
                .OrderBy(c => c.Mileage)
                .ToList();

            foreach (var car in cars)
            {
                result.Add(
                    GetHeroCard(
                        car.Year + " - " + car.Model,
                        car.Price + " - " + car.Mileage,
                        car.Description,
                        new CardImage(url: car.Images[0].Image),
                        new CardAction(ActionTypes.OpenUrl, "More Info", value: car.URL)
                        )
                    );
            }
            return result;
        }

        private static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            string desc = "";
            if (text.Length > 100)
                desc = text.Substring(0, 99);
            else
                desc = text;

            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = desc,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction }
            };

            return heroCard.ToAttachment();
        }

        #endregion

    }
}