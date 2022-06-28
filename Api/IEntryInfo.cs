using AlarmTableGenerator.Api.Struct;
using System;
using System.Collections.Generic;

namespace AlarmTableGenerator.Api
{
    /// <summary>
    /// The interface for individual entries in a file
    /// </summary>
    public interface IEntryInfo
    {
        /// <summary>
        /// Timestamp for the entry
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// True if Error list has contents
        /// </summary>
        bool ContainsErrors { get; }

        /// <summary>
        /// The list of errors that were generated during parsing of the file
        /// </summary>
        List<Error> Errors { get; }
    }
}
