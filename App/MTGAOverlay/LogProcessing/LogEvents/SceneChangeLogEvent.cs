using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogProcessing.LogEvents
{
    internal class SceneChangeLogEvent : LogEvent
    {
        public string FromSceneName { get; }
        public string ToSceneName { get; }

        private SceneChangeLogEvent(string fromSceneName, string toSceneName)
        {
            FromSceneName = fromSceneName;
            ToSceneName = toSceneName;
        }

        public static SceneChangeLogEvent FromJson(JsonElement jsonElement)
        {
            var requestElement = jsonElement.GetProperty("request");
            var requestDocument = JsonDocument.Parse(requestElement.GetString());

            var payloadElement = requestDocument.RootElement
                .GetProperty("params")
                .GetProperty("payloadObject");

            var fromSceneName = payloadElement.GetProperty("fromSceneName").GetString();
            var toSceneName = payloadElement.GetProperty("toSceneName").GetString();

            return new SceneChangeLogEvent(fromSceneName, toSceneName);
        }
    }
}
