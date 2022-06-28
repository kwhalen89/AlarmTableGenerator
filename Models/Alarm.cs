using AlarmTableGenerator.Api;
using AlarmTableGenerator.Api.Enum;
using AlarmTableGenerator.Api.Struct;
using System;
using System.Globalization;
using System.Collections.Generic;

namespace AlarmTableGenerator.Models
{
    public class Alarm : IEntryInfo
    {
        public int LineNumber { get; private set; }
        public DateTime Timestamp { get; private set; }
        public float? Trip { get; private set; }
        public string AlarmType { get; private set; }
        public SeverityLevel Severity { get; private set; }
        public bool ContainsErrors => Errors.Count > 0;
        public List<Error> Errors { get; private set; }

        public Alarm(int lineNum, string timestamp, string trip, string alarmType, string severity)
        {
            Errors = new List<Error>();

            //timestamp
            try
            {
                //adding a Z will force it to UTC
                if (!timestamp.EndsWith("Z", StringComparison.InvariantCulture))
                {
                    timestamp += "Z";
                }

                Timestamp = DateTime.Parse(timestamp);
            }
            catch (Exception ex)
            {
                Errors.Add(new Error(lineNum, $"First column was unable to convert to datetime, {ex}"));
            }

            //trip
            if (!float.TryParse(trip, out var tripFloat))
            {
                Trip = null;
                Errors.Add(new Error(lineNum, $"Second column was unable to convert to float")); 
            }
            else
            {
                Trip = tripFloat;
            }

            //alarm
            alarmType = alarmType.Trim();
            if (alarmType.Length > 10 || alarmType.Length < 1)
            { 
                AlarmType = "ALM";
                Errors.Add(new Error(lineNum, $"Third column must be 1-10 characters, defaulted entry to ALM"));
            }
            else
                AlarmType = alarmType;

            //severity
            if (!Int32.TryParse(severity, out var severityInt))
            {
                Severity = SeverityLevel.INFO;
                Errors.Add(new Error(lineNum, $"Fourth column must be an integer between 1-15, defaulted entry to INFO"));
            }

            switch (severityInt)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    Severity = SeverityLevel.LOW;
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    Severity = SeverityLevel.MED;
                    break;
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    Severity = SeverityLevel.HIGH;
                    break;
                default:
                    Severity = SeverityLevel.INFO;
                    Errors.Add(new Error(lineNum, $"Fourth column must be an integer between 1-15, defaulted entry to INFO"));
                    break;
            }
        }
    }
}
