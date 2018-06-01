using System;
using System.Collections.Generic;
using System.Globalization;
using DarkSky.Services;
using System.IO;
using DarkSky.Models;
using System.Threading.Tasks;
using System.Numerics;
using SixLabors.Fonts;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using NodaTime.TimeZones;
using System.Linq;
using NodaTime;
using SixLabors.Primitives;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.Processing.Text;

namespace Coremero.Plugin.Weather
{
    // Sponges weather renderer.
    // Taken from https://github.com/sponge/coreweather

    // TODO: Go hog wild and clean this up to my standard, whatever that is.
    public static class WeatherColors
    {
        public static Rgba32 White = new Rgba32(255, 255, 255);
        public static Rgba32 Black = new Rgba32(0, 0, 0);
        public static Rgba32 Blue = new Rgba32(8, 15, 255);
        public static Rgba32 LightBlue = new Rgba32(121, 112, 255);
        public static Rgba32 DarkBlue = new Rgba32(41, 25, 92);
        public static Rgba32 Orange = new Rgba32(194, 108, 2);
        public static Rgba32 Red = new Rgba32(198, 19, 2);
        public static Rgba32 Teal = new Rgba32(47, 65, 120);
        public static Rgba32 TealAlso = new Rgba32(172, 177, 237);
        public static Rgba32 Yellow = new Rgba32(205, 185, 0);
    }

    public class DrawCommand
    {
        public bool IsRelative { get; set; }
        public int ContentWidth { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Text { get; set; }
        public Font Font { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public Rgba32 Color { get; set; }
        public Image<Rgba32> Image { get; set; }
    }

    public class Location
    {
        public string FormattedAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class WeatherRendererDay
    {
        public ZonedDateTime Date { get; set; }
        public string Day { get; set; }
        public string Icon { get; set; }
        public string Summary { get; set; }
        public double? HiTemp { get; set; }
        public double? LoTemp { get; set; }
    }

    public class WeatherRendererInfo
    {
        public string Address { get; set; }
        public string Unit { get; set; }
        public ZonedDateTime Date { get; set; }
        public double? Temperature { get; set; }
        public double? FeelsLike { get; set; }
        public string Alert { get; set; }
        public List<WeatherRendererDay> Forecast { get; set; } = new List<WeatherRendererDay>();
    }

    internal class Weather
    {
        private string darkSkyApiKey;
        private FontCollection collection;
        private Font mdFont;
        private Font lgFont;
        private Font smFont;
        private Dictionary<string, Image<Rgba32>> images;

        private Dictionary<string, string> weatherDescription = new Dictionary<string, string>()
        {
            ["ClearDay"] = "Clear",
            ["ClearNight"] = "Clear",
            ["Cloudy"] = "Cloudy",
            ["Fog"] = "Fog",
            ["PartlyCloudyDay"] = "Partly\nCloudy",
            ["PartlyCloudyNight"] = "Partly\nCloudy",
            ["Rain"] = "Rain",
            ["Sleet"] = "Sleet",
            ["Snow"] = "Snow",
            ["Wind"] = "Wind"
        };

        public Weather(string darkSkyApiKey, string ResourceDir)
        {
            this.darkSkyApiKey = darkSkyApiKey;

            collection = new FontCollection();
            smFont = new Font(collection.Install(Path.Combine(ResourceDir, "Weather", "Star4000 Small.ttf")), 36);
            mdFont = new Font(collection.Install(Path.Combine(ResourceDir, "Weather", "Star4000.ttf")), 36);
            lgFont = new Font(collection.Install(Path.Combine(ResourceDir, "Weather", "Star4000 Large.ttf")), 32);

            images = new Dictionary<string, Image<Rgba32>>();
            foreach (var image in Directory.GetFiles(Path.Combine(ResourceDir, "Weather"))
                .Where(x => x.Contains(".png") || x.Contains(".gif")))
            {
                var basename = Path.GetFileNameWithoutExtension(image);
                images.Add(basename, Image.Load(image));
            }
        }

        // only get the data here, buddy
        public async Task<WeatherRendererInfo> GetForecastAsync(string query)
        {
            // use google to get address, lat, and lng for a human-entered string
            Location location;
            try
            {
                var requestUri = string.Format(
                    "http://maps.googleapis.com/maps/api/geocode/json?address={0}&sensor=false",
                    Uri.EscapeDataString(query));

                using (var client = new HttpClient())
                {
                    var request = await client.GetAsync(requestUri);
                    var content = await request.Content.ReadAsStringAsync();
                    JObject results = JObject.Parse(content);

                    var address_components = results["results"][0]["address_components"];
                    var loc = results["results"][0]["geometry"]["location"];

                    var locality = (from a in address_components where a["types"][0].Value<string>() == "locality" select a["short_name"].Value<string>()).FirstOrDefault();

                    location = new Location
                    {
                        Latitude = loc["lat"].Value<double>(),
                        Longitude = loc["lng"].Value<double>(),
                        FormattedAddress = locality
                    };
                }
            }
            catch (Exception)
            {
                return null;
            }

            // request darksky without minutely/hourly, and use location to determine units
            var WeatherService = new DarkSkyService(darkSkyApiKey);
            DarkSkyResponse forecast = await WeatherService.GetForecast(location.Latitude, location.Longitude,
                new DarkSkyService.OptionalParameters
                {
                    DataBlocksToExclude = new List<ExclusionBlock> { ExclusionBlock.Minutely, ExclusionBlock.Hourly, },
                    MeasurementUnits = "auto"
                });

            var timezone = DateTimeZoneProviders.Tzdb[forecast.Response.TimeZone];
            var myTime = SystemClock.Instance.GetCurrentInstant();
            var info = new WeatherRendererInfo()
            {
                Address = location.FormattedAddress,
                Unit = forecast.Response.Flags.Units == "us" ? "F" : "C",
                Date = myTime.InZone(timezone),
                Temperature = forecast.Response.Currently.Temperature,
                FeelsLike = forecast.Response.Currently.ApparentTemperature,
                Alert = forecast.Response.Alerts?[0].Title
            };

            int counter = 0;
            foreach (var day in forecast.Response.Daily.Data.Take(4))
            {
                var dayRender = new WeatherRendererDay()
                {
                    HiTemp = day.TemperatureHigh,
                    LoTemp = day.TemperatureLow,
                    Summary = weatherDescription.ContainsKey(day.Icon.ToString())
                        ? weatherDescription[day.Icon.ToString()]
                        : day.Icon.ToString().Replace("-", ""),
                    Icon = String.Join("-", Regex.Split(day.Icon.ToString(), @"(?<!^)(?=[A-Z])")).ToLower(),
                    Date = info.Date.Plus(Duration.FromDays(counter))
                };

                info.Forecast.Add(dayRender);
                counter++;
            }
            return info;
        }

        public MemoryStream RenderWeatherImage(WeatherRendererInfo info)
        {
            MemoryStream output = new MemoryStream();

            using (Image<Rgba32> image = images["background"].Clone())
            {
                var now = info.Date;

                var topDateLine = now.ToString("h:mm:ss tt", CultureInfo.CurrentCulture).ToUpper();
                var bottomDateLine = now.ToString("ddd MMM d", CultureInfo.CurrentCulture).ToUpper();
                var tickerLine = info.Alert != null ? info.Alert + "\n" : "";
                tickerLine +=
                    $"Temp: {(int)info.Temperature}°{info.Unit}   Feels Like: {(int)info.FeelsLike}°{info.Unit}";

                if (info.Alert != null)
                {
                    image.Mutate(i => i.Fill<Rgba32>(WeatherColors.Red, new Rectangle(0, 480, image.Width, 96)));
                }

                // everything except the forecast
                var cmds = new List<DrawCommand>
                {
                    new DrawCommand()
                    {
                        Text = info.Address,
                        Font = mdFont,
                        X = 150,
                        Y = 8,
                        Color = WeatherColors.White
                    },
                    new DrawCommand()
                    {
                        Text = "Extended Forecast",
                        Font = mdFont,
                        IsRelative = true,
                        X = 0,
                        Y = 36,
                        Color = WeatherColors.Yellow
                    },
                    new DrawCommand() {Text = topDateLine, Font = smFont, X = 695, Y = 10, Color = WeatherColors.White},
                    new DrawCommand()
                    {
                        Text = bottomDateLine,
                        Font = smFont,
                        IsRelative = true,
                        X = 0,
                        Y = 25,
                        Color = WeatherColors.White
                    },
                    new DrawCommand() {Text = tickerLine, Font = mdFont, X = 5, Y = 485, Color = WeatherColors.White},
                };

                int bx = 15, by = 90;
                foreach (var day in info.Forecast)
                {
                    cmds.AddRange(new List<DrawCommand>
                    {
                        new DrawCommand() {X = bx, Y = by},

                        // day of week, icon, summary
                        new DrawCommand()
                        {
                            Text = day.Date.ToString("ddd", CultureInfo.CurrentCulture).ToUpper(),
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            IsRelative = true,
                            X = 100,
                            Y = 30,
                            Font = mdFont,
                            Color = WeatherColors.Yellow
                        },
                        new DrawCommand()
                        {
                            Image = images.ContainsKey(day.Icon) ? images[day.Icon] : images["clear-day"],
                            HorizontalAlignment = HorizontalAlignment.Center,
                            ContentWidth = 190,
                            IsRelative = true,
                            X = -90,
                            Y = 40
                        },
                        new DrawCommand()
                        {
                            Text = day.Summary,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            IsRelative = true,
                            X = 90,
                            Y = 150,
                            Font = mdFont,
                            Color = WeatherColors.White
                        },

                        // low temperature
                        new DrawCommand()
                        {
                            Text = "Lo",
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            IsRelative = true,
                            X = -50,
                            Y = 85,
                            Font = mdFont,
                            Color = WeatherColors.TealAlso
                        },
                        new DrawCommand()
                        {
                            Text = day.LoTemp != null ? Math.Round(day.LoTemp.Value, 0).ToString() : "",
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            IsRelative = true,
                            X = 0,
                            Y = 45,
                            Font = lgFont,
                            Color = WeatherColors.White
                        },

                        // high temperature
                        new DrawCommand()
                        {
                            Text = "Hi",
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            IsRelative = true,
                            X = 100,
                            Y = -50,
                            Font = mdFont,
                            Color = WeatherColors.Yellow
                        },
                        new DrawCommand()
                        {
                            Text = day.HiTemp != null ? Math.Round(day.HiTemp.Value, 0).ToString() : "",
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            IsRelative = true,
                            X = 0,
                            Y = 45,
                            Font = lgFont,
                            Color = WeatherColors.White
                        },
                    });

                    bx += 244;
                }

                int x = 0, y = 0;
                foreach (var cmd in cmds)
                {
                    // reset the position if it's not relative
                    if (cmd.IsRelative)
                    {
                        x += cmd.X;
                        y += cmd.Y;
                    }
                    else
                    {
                        x = cmd.X;
                        y = cmd.Y;
                    }

                    // if we have a text object, use textalignment, color, and text fields
                    if (cmd.Text != null)
                    {
                        var textOpts = new TextGraphicsOptions(false) { HorizontalAlignment = cmd.HorizontalAlignment, VerticalAlignment = cmd.VerticalAlignment };
                        image.Mutate(i => i.DrawText(textOpts, cmd.Text, cmd.Font, WeatherColors.Black, new Vector2(x + 2, y + 2)));
                        image.Mutate(i => i.DrawText(textOpts, cmd.Text, cmd.Font, cmd.Color, new Vector2(x, y)));
                    }

                    // if we have an image object, use the image field
                    if (cmd.Image != null)
                    {
                        Point pos = new Point(x, y);
                        if (cmd.ContentWidth > 0 && cmd.HorizontalAlignment == HorizontalAlignment.Center)
                        {
                            pos.X += (cmd.ContentWidth - cmd.Image.Width) / 2;
                        }

                        image.Mutate(i => i.DrawImage(cmd.Image, PixelBlenderMode.Normal, 1.0f, pos));
                    }
                }
                image.SaveAsGif(output);
            }
            output.Seek(0, SeekOrigin.Begin);
            return output;
        }
    }
}