using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EDFLib
{
    public class Header
    {
        // Field Widths
        public const int BASIC_FIELD_LENGTH = 8;
        public const int LONG_FIELD_LENGTH = 80;
        public const int RESERVED_HEADER_LENGTH = 44;
        public const int NUM_SIGNALS_LENGTH = 4;
        public const int LABEL_FIELD_LENGTH = 16;
        public const int SIGNAL_RESERVED_LENGTH = 32;

        // Header size measurements
        // Header uses 256 bytes for common info and an additional 
        // 256 bytes per signal.
        public const int HEADER_FIXED_SIZE = 256;

        // Version
        public const char VERSION = '0';

        // Date format strings
        public const string DATE_FORMAT = "dd.MM.yy";
        public const string TIME_FORMAT = "hh.mm.ss";

        public string PatientID { get; set; }
        public string RecordingID { get; set; }

        // Recording Start time as a DateTimeOffset.
        // Will be null, if the RecordingStartDate and Time read 
        // from the file are not valid.
        private DateTimeOffset? recordingStartTime;
        public DateTimeOffset? RecordingStartTime
        {
            get
            {
                return recordingStartTime;
            }
            set
            {
                recordingStartTime = value;

                // Only modify the Start Date and Time if value is non-null.
                if (value.HasValue)
                {
                    // EDF+ specification: use 1985 as a cutoff date (for two-digit years).
                    // https://www.edfplus.info/specs/edfplus.html#edfplusandedf
                    if (value.Value.Year <= 2084)
                    {
                        RecordingStartDateString = value.Value.ToString(DATE_FORMAT);
                    }
                    else
                    {
                        // EDF+: After 2084, replace year with 'yy'
                        RecordingStartDateString = value.Value.ToString("dd.MM") + ".yy";
                    }

                    RecordingStartTimeString = value.Value.ToString(TIME_FORMAT);
                }
            }
        }

        public int RecordDurationInSeconds { get; set; }
        public int NumberOfRecords { get; set; }
        public int SignalCount { get; set; }

        public string RecordingStartDateString { get; private set; }

        public string RecordingStartTimeString { get; private set; }

        public List<SignalHeader> SignalHeaders { get; set; } = new List<SignalHeader>();
    }
}
