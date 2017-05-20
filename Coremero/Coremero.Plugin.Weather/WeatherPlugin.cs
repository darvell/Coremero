using System;
using System.IO;
using System.Threading.Tasks;
using Coremero.Utilities;
using Coremero.Commands;
using Coremero.Attachments;
using Coremero.Context;
using Coremero.Messages;
using Coremero.Storage;

namespace Coremero.Plugin.Weather
{
    public class WeatherPlugin : IPlugin
    {
        private string DARK_SKIES_APIKEY = "";
        public WeatherPlugin(ICredentialStorage credentialStorage)
        {
            DARK_SKIES_APIKEY = credentialStorage.GetKey("darkskies", "");
        }

        [Command("weather")]
        public async Task<IMessage> GetWeather(IInvocationContext context, string message)
        {
            Weather weather = new Weather(DARK_SKIES_APIKEY);
            WeatherRendererInfo forecast = await weather.GetForecastAsync(message.TrimCommand());
            return Message.Create("", new StreamAttachment(weather.RenderWeatherImage(forecast), "weather.gif"));
        }
    }
}
