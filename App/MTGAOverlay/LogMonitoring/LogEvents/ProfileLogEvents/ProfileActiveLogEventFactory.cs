using MindSculptor.Tools;
using System;
using System.Text.RegularExpressions;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.ProfileLogEvents
{
    internal class ProfileActiveLogEventFactory
    {
        public NullableReference<string> MtgaUserId { get; private set; } = null;
        public NullableReference<string> Name       { get; private set; } = null;
        public int?                      NameId     { get; private set; } = null;

        public bool TryParseAccountLogInMessage(string logMessage, out NullableReference<LogEvent> result)
        {
            Invalidate();
            var regexMatch = Regex.Match(logMessage, @"^\[Accounts - Client\] Successfully logged in to account: ([^#]+)#(.+)$");
            if (regexMatch.Success)
            {
                Name   = regexMatch.Groups[1].Value;
                NameId = Convert.ToInt32(regexMatch.Groups[2].Value);
            }
            result = null;
            return false;
        }

        public bool TryParseAccountUpdateMessage(string logMessage, out NullableReference<LogEvent> result)
        {
            if (Name.HasValue && NameId.HasValue)
            {
                var regexMatch = Regex.Match(logMessage, @"^\[Accounts - AccountClient\]\(1*\d:[0-5]\d:[0-5]\d [AP]M\) Updated account\. DisplayName:([^#]+)#([^,]+), AccountID:([^,]+), [\w\W]*$");
                if (regexMatch.Success)
                {
                    var mtgaUserId = regexMatch.Groups[3].Value;
                    var name       = regexMatch.Groups[1].Value;
                    var nameId     = Convert.ToInt32(regexMatch.Groups[2].Value);

                    if (Name.Value == name && NameId.Value == nameId)
                    {
                        MtgaUserId = mtgaUserId;
                        return ProfileActiveLogEvent.TryCreateFromFactory(this, out result);
                    }
                }
            }
            result = null;
            return false;
        }

        public void Invalidate()
        {
            MtgaUserId  = null;
            Name        = null;
            NameId      = null;
        }
    }
}
