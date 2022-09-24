using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static EDFLib.Header;

namespace EDFLib
{
    public class EDFReader
    {
        private ILogger<EDFReader> _logger;

        public EDFReader(ILogger<EDFReader> logger)
        {
            _logger = logger;
        }

        public async Task<EDFFile> Read(Stream stream)
        {
            // Read header
            Header header = await ReadHeader(stream);

            return new EDFFile()
            {
                Header = header
            };
        }

        public async Task<EDFFile> Read(string path)
        {
            using (Stream stream = File.OpenRead(path))
            {
                return await Read(stream);
            }
        }

        // Attempts to read and return an EDF header from the provided
        // Stream object.
        // If it can't create a valid Header, an exception is thrown.
        // Accepts optional logger as parameter
        public async Task<Header> ReadHeader(Stream stream)
        {
            using (StreamReader streamReader = new StreamReader(stream))
            {
                Header header = new Header();

                // Read version
                string version = await ReadField(streamReader, BASIC_FIELD_LENGTH);

                // Ensure version is zero; if not, throw an exception.
                if (version[0] != VERSION)
                {
                    throw new FormatException($"Invalid version field: \"{version}\"");
                }

                // Patient identification - trim whitespace.
                string patientID = await ReadField(streamReader, LONG_FIELD_LENGTH);
                header.PatientID = patientID.Trim();
                // TODO: separate EDF+ fields?

                // Record identification - trim whitespace.
                string recordingID = await ReadField(streamReader, LONG_FIELD_LENGTH);
                header.RecordingID = recordingID.Trim();
                // TODO: read date from recording ID

                // Start date and start time.
                // Assign to header.
                string startDate = await ReadField(streamReader, BASIC_FIELD_LENGTH);
                //header.RecordingStartDateString = startDate;

                string startTime = await ReadField(streamReader, BASIC_FIELD_LENGTH);
                //header.RecordingStartTimeString = startTime;

                // Parse start date and time to fill in the DateTimeOffset.
                // Could fail.
                // TODO: Determine behavior of TryParseExact for two-digit dates.
                bool validDate = DateTimeOffset.TryParseExact(startDate, DATE_FORMAT, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTimeOffset recordingStartDate);

                if (validDate)
                {
                    bool validTime = DateTimeOffset.TryParseExact(startTime, TIME_FORMAT, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTimeOffset recordingStartTime);

                    if (validTime)
                    {
                        // Note: this will overwrite the strings we read from the file, because those
                        // are also set by the setter of 'RecordingStartTime'.
                        // It shouldn't matter, because this is the date parsed from the file.
                        header.RecordingStartTime = recordingStartDate.Add(recordingStartTime.TimeOfDay);
                    }
                    else
                    {
                        _logger?.LogWarning("Could not parse time from time string: {startTime}", startTime);
                    }
                }
                else
                {
                    _logger?.LogWarning("Could not parse date from date string: {startDate}", startDate);
                }

                // Number of bytes in header record - tells us how many signals
                string headerByteCountString = await ReadField(streamReader, BASIC_FIELD_LENGTH);
                // Allowed to throw an exception if not a number.
                int headerByteCount = int.Parse(headerByteCountString);

                // Read and discard reserved characters to advance stream.
                await ReadField(streamReader, RESERVED_HEADER_LENGTH);

                // Number of data records.
                string numRecordsString = await ReadField(streamReader, BASIC_FIELD_LENGTH);
                int numRecords = int.Parse(numRecordsString);
                header.NumberOfRecords = numRecords;

                // Duration of a data record (seconds)
                string recordDurationString = await ReadField(streamReader, BASIC_FIELD_LENGTH);
                float recordDuration = float.Parse(recordDurationString);
                header.RecordDurationInSeconds = recordDuration;

                // Number of signals in a data record (4 bytes - max 9999)
                string numSignalsString = await ReadField(streamReader, NUM_SIGNALS_LENGTH);
                int numSignals = int.Parse(numSignalsString);
                header.SignalCount = numSignals;

                // Verify that the header byte count makes sense, given the number of signals.
                int calculatedNumSignals = (headerByteCount / HEADER_FIXED_SIZE) - 1;

                if (numSignals != calculatedNumSignals)
                {
                    _logger?.LogWarning("Number of signals {numSignals} doesn't match with header byte count {headerByteCount}," +
                        " this may result in parsing errors.");
                }

                // Attempt to read in numSignals signals.
                // Because signal properties are grouped by property type instead of
                // signal, we need to iterate over the list of signals repeatedly.
                List<SignalHeader> signalHeaders = new List<SignalHeader>();
                for (int signalIndex = 0; signalIndex < numSignals; signalIndex++)
                {
                    SignalHeader signalHeader = new SignalHeader();
                    string label = await ReadField(streamReader, LABEL_FIELD_LENGTH);
                    signalHeader.Label = label.Trim();
                    signalHeaders.Add(signalHeader);
                }

                // Transducer type
                foreach (SignalHeader signalHeader in signalHeaders)
                {
                    string transducerType = await ReadField(streamReader, LONG_FIELD_LENGTH);
                    signalHeader.TransducerType = transducerType.Trim();
                }

                // Physical dimension
                foreach (SignalHeader signalHeader in signalHeaders)
                {
                    string physicalDimension = await ReadField(streamReader, BASIC_FIELD_LENGTH);
                    signalHeader.PhysicalDimension = physicalDimension.Trim();
                }

                // Physical minimum
                foreach (SignalHeader signalHeader in signalHeaders)
                {
                    string physicalMinimum = await ReadField(streamReader, BASIC_FIELD_LENGTH);
                    signalHeader.PhysicalMinimum = double.Parse(physicalMinimum);
                }

                // Physical maximum
                foreach (SignalHeader signalHeader in signalHeaders)
                {
                    string physicalMaximum = await ReadField(streamReader, BASIC_FIELD_LENGTH);
                    signalHeader.PhysicalMaximum = double.Parse(physicalMaximum);
                }

                // Digital minimum
                foreach (SignalHeader signalHeader in signalHeaders)
                {
                    string digitalMinimum = await ReadField(streamReader, BASIC_FIELD_LENGTH);
                    signalHeader.DigitalMinimum = double.Parse(digitalMinimum);
                }

                // Digital maximum
                foreach (SignalHeader signalHeader in signalHeaders)
                {
                    string digitalMaximum = await ReadField(streamReader, BASIC_FIELD_LENGTH);
                    signalHeader.DigitalMaximum = double.Parse(digitalMaximum);
                }

                // Prefiltering
                foreach (SignalHeader signalHeader in signalHeaders)
                {
                    string prefiltering = await ReadField(streamReader, LONG_FIELD_LENGTH);
                    signalHeader.Prefiltering = prefiltering.Trim();
                }

                // Number of samples per record for each signal.
                foreach (SignalHeader signalHeader in signalHeaders)
                {
                    string numSamples = await ReadField(streamReader, BASIC_FIELD_LENGTH);
                    signalHeader.SampleCountPerRecord = int.Parse(numSamples);
                }

                // Read and discard reserved section for each signal, to advance Stream as needed.
                for (int i = 0; i < numSignals; i++)
                {
                    await ReadField(streamReader, SIGNAL_RESERVED_LENGTH);
                }

                // Finally, assign signals.
                header.SignalHeaders = signalHeaders;

                return header;
            }
        }

        // Utility method for reading a fixed number of bytes and returning the result as an ascii string.
        public async Task<string> ReadField(StreamReader streamReader, int length)
        {
            char[] fieldBytes = new char[length];
            int index = 0;

            while (index < length && !streamReader.EndOfStream)
            {
                // Read 8 characters
                int bytesRead = await streamReader.ReadAsync(fieldBytes, index, length);
                index += bytesRead;
            }

            return new string(fieldBytes);
        }
    }
}
