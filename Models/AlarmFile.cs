using AlarmTableGenerator.Api;
using AlarmTableGenerator.Api.Struct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlarmTableGenerator.Models
{
    public class AlarmFile : IParsedFile
    {
        public string FileName { get; set; }
        public Type FileType => typeof(AlarmFile);
        public DateTime FileEntered { get; private set; }
        public bool ContainsErrors => Entries.Any(x => x.ContainsErrors == true);
        public List<IEntryInfo> Entries { get; set; }

        public AlarmFile()
        {
            FileEntered = DateTime.Now;
            Entries = new List<IEntryInfo>();
        }

        public bool IsMatchingFileType(string[] headerEntry)
        {
            //if the header doesn't have 4 sections or any of the sections aren't exactly as expected then this fails returns
            if (headerEntry.Length < 4 ||
                !String.Equals(headerEntry[0], "TimeStampUTC", StringComparison.InvariantCulture) ||
                !String.Equals(headerEntry[1], "Trip", StringComparison.InvariantCulture) ||
                !String.Equals(headerEntry[2], "Alarm", StringComparison.InvariantCulture) ||
                !String.Equals(headerEntry[3], "Severity", StringComparison.InvariantCulture))
            {
                return false;
            }

            return true;
        }

        public bool TryAddEntry(int lineNum, string[] entry)
        {
            if (entry.Length < 4)
            {
                return false;
            }

            var alarm = new Alarm(lineNum, entry[0], entry[1], entry[2], entry[3]);
            Entries.Add(alarm);
            return true;
        }

        public List<Alarm> TryConvertEntries()
        {
            List<Alarm> alarmList = new List<Alarm>();
            foreach (var entry in Entries)
            {
                alarmList.Add(entry as Alarm);
            }

            return alarmList;
        }

        public List<Error> CompileErrors()
        {
            List<Error> errorList = new List<Error>();
            foreach (var entry in Entries)
            {
                var alarm = entry as Alarm;
                errorList.AddRange(alarm.Errors);
            }

            return errorList;
        }
    }
}
