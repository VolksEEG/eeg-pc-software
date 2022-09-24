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

        public float RecordDurationInSeconds { get; set; }
        public int NumberOfRecords { get; set; }
        public int SignalCount { get; set; }

        public string RecordingStartDateString { get; private set; }

        public string RecordingStartTimeString { get; private set; }

        public List<SignalHeader> SignalHeaders { get; set; } = new List<SignalHeader>();

        /// <summary>
        /// Write the contents of this header to the provided stream.
        /// </summary>
        /// <param name="stream"></param>
        /// TODO: move into a 'writer' class
        public void Write(Stream stream)
        {
            // See EDF specification for Header layout:
            // https://www.edfplus.info/specs/edf.html

            // Write version (8 ascii, starting with character '0')
            WriteField(stream, VERSION.ToString(), BASIC_FIELD_LENGTH);

            // Write Patient identification.
            WriteField(stream, PatientID, LONG_FIELD_LENGTH);

            // Write Record identification
            WriteField(stream, RecordingID, LONG_FIELD_LENGTH);

            // Start date, 8 ascii
            WriteField(stream, RecordingStartDateString, BASIC_FIELD_LENGTH);

            // Start time, 8 ascii
            WriteField(stream, RecordingStartTimeString, BASIC_FIELD_LENGTH);

            // Number of bytes in header record.
            // Value is based on a fixed header length.
            int headerByteCount = HEADER_FIXED_SIZE * (SignalCount + 1);
            WriteField(stream, headerByteCount.ToString(), BASIC_FIELD_LENGTH);

            // 44 reserved ascii characters.
            WriteField(stream, "RESERVED", RESERVED_HEADER_LENGTH);

            // Number of data records.
            WriteField(stream, NumberOfRecords.ToString(), BASIC_FIELD_LENGTH);

            // Duration of a data record (seconds)
            WriteField(stream, RecordDurationInSeconds.ToString(), BASIC_FIELD_LENGTH);

            // Number of signals in a data record (4 bytes - max 9999)
            WriteField(stream, SignalCount.ToString(), NUM_SIGNALS_LENGTH);

            // Write signal header properties.
            // Label
            foreach (SignalHeader signalHeader in SignalHeaders)
            {
                WriteField(stream, signalHeader.Label, LABEL_FIELD_LENGTH);
            }

            // Transducer type
            foreach (SignalHeader signalHeader in SignalHeaders)
            {
                WriteField(stream, signalHeader.TransducerType, LONG_FIELD_LENGTH);
            }

            // Physical dimension
            foreach (SignalHeader signalHeader in SignalHeaders)
            {
                WriteField(stream, signalHeader.PhysicalDimension, BASIC_FIELD_LENGTH);
            }

            // Physical minimum
            foreach (SignalHeader signalHeader in SignalHeaders)
            {
                WriteField(stream, signalHeader.PhysicalMinimum.ToString(), BASIC_FIELD_LENGTH);
            }

            // Physical maximum
            foreach (SignalHeader signalHeader in SignalHeaders)
            {
                WriteField(stream, signalHeader.PhysicalMaximum.ToString(), BASIC_FIELD_LENGTH);
            }

            // Digital minimum
            foreach (SignalHeader signalHeader in SignalHeaders)
            {
                WriteField(stream, signalHeader.DigitalMinimum.ToString(), BASIC_FIELD_LENGTH);
            }

            // Digital maximum
            foreach (SignalHeader signalHeader in SignalHeaders)
            {
                WriteField(stream, signalHeader.DigitalMaximum.ToString(), BASIC_FIELD_LENGTH);
            }

            // Prefiltering
            foreach (SignalHeader signalHeader in SignalHeaders)
            {
                WriteField(stream, signalHeader.Prefiltering, LONG_FIELD_LENGTH);
            }

            // Number of samples per record for each signal.
            foreach (SignalHeader signalHeader in SignalHeaders)
            {
                WriteField(stream, signalHeader.SampleCountPerRecord.ToString(), BASIC_FIELD_LENGTH);
            }

            // RESERVED bytes.
            foreach (SignalHeader _ in SignalHeaders)
            {
                WriteField(stream, "RESERVED", SIGNAL_RESERVED_LENGTH);
            }
        }

        private void WriteField(Stream stream, string field, int fieldLength)
        {
            // Truncate to field length, if necessary.
            if (field.Length > fieldLength)
            {
                field = field.Substring(0, fieldLength);
            }

            // Pads field with spaces up to the required field length.
            field += new string(' ', fieldLength - field.Length);
            byte[] fieldBytes = Encoding.ASCII.GetBytes(field);
            Debug.Assert(fieldBytes.Length == fieldLength);
            stream.Write(fieldBytes, 0, fieldBytes.Length);
        }
    }
}
