using System;
using System.Collections.Generic;

namespace AlarmTableGenerator.Api
{
    /// <summary>
    /// The interface that instructs classes on what to hold from it's parsed information
    /// </summary>
    public interface IParsedFile
    {
        string FileName { get; set; }

        /// <summary>
        /// Easy access to know what type of file was parsed
        /// </summary>
        Type FileType { get; }

        /// <summary>
        /// The datetime the file was imported into the system
        /// </summary>
        DateTime FileEntered { get; }  

        /// <summary>
        /// If the file contained errors when parsing
        /// </summary>
        bool ContainsErrors { get; }

        /// <summary>
        /// The list of entries that were generated from parsing the file
        /// </summary>
        List<IEntryInfo> Entries { get; }

        /// <summary>
        /// Determines using the file header if this file is of the class type
        /// </summary>
        /// <param name="headerEntry"></param>
        bool IsMatchingFileType(string[] headerEntry);

        /// <summary>
        /// Tries to add a line from the file into the designated class
        /// </summary>
        /// /// <param name="lineNum"></param>
        /// <param name="entry"></param>
        bool TryAddEntry(int lineNum, string[] entry);
    }
}
