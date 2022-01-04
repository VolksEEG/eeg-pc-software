using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static EDFLib.Header;

namespace EDFLib
{
    public class EDFWriter : IDisposable
    {
        private readonly ILogger<EDFReader> _logger;
        private Stream _stream;

        public EDFWriter(string filename, ILogger<EDFReader> logger) : this(File.OpenWrite(filename), logger)
        {

        }

        public EDFWriter(Stream stream, ILogger<EDFReader> logger)
        {
            _stream = stream;
            _logger = logger;
        }

        public void Write(EDFFile file)
        {
            WriteHeader(file.Header);

            WriteSignals(file.Signals, file.Header.NumberOfRecords);
        }

        /// <summary>
        /// Write the contents of the provided header to this writer's stream.
        /// </summary>
        public void WriteHeader(Header header)
        {
            // See EDF specification for Header layout:
            // https://www.edfplus.info/specs/edf.html

            // Write version (8 ascii, starting with character '0')
            WriteField(_stream, VERSION.ToString(), BASIC_FIELD_LENGTH);

            // Write Patient identification.
            WriteField(_stream, header.PatientID, LONG_FIELD_LENGTH);

            // Write Record identification
            WriteField(_stream, header.RecordingID, LONG_FIELD_LENGTH);

            // Start date, 8 ascii
            WriteField(_stream, header.RecordingStartDateString, BASIC_FIELD_LENGTH);

            // Start time, 8 ascii
            WriteField(_stream, header.RecordingStartTimeString, BASIC_FIELD_LENGTH);

            // Number of bytes in header record.
            // Value is based on a fixed header length.
            int headerByteCount = HEADER_FIXED_SIZE * (header.SignalCount + 1);
            WriteField(_stream, headerByteCount.ToString(), BASIC_FIELD_LENGTH);

            // 44 reserved ascii characters.
            WriteField(_stream, "RESERVED", RESERVED_HEADER_LENGTH);

            // Number of data records.
            WriteField(_stream, header.NumberOfRecords.ToString(), BASIC_FIELD_LENGTH);

            // Duration of a data record (seconds)
            WriteField(_stream, header.RecordDurationInSeconds.ToString(), BASIC_FIELD_LENGTH);

            // Number of signals in a data record (4 bytes - max 9999)
            WriteField(_stream, header.SignalCount.ToString(), NUM_SIGNALS_LENGTH);

            // Write signal header properties.
            // Label
            foreach (SignalHeader signalHeader in header.SignalHeaders)
            {
                WriteField(_stream, signalHeader.Label, LABEL_FIELD_LENGTH);
            }

            // Transducer type
            foreach (SignalHeader signalHeader in header.SignalHeaders)
            {
                WriteField(_stream, signalHeader.TransducerType, LONG_FIELD_LENGTH);
            }

            // Physical dimension
            foreach (SignalHeader signalHeader in header.SignalHeaders)
            {
                WriteField(_stream, signalHeader.PhysicalDimension, BASIC_FIELD_LENGTH);
            }

            // Physical minimum
            foreach (SignalHeader signalHeader in header.SignalHeaders)
            {
                WriteField(_stream, signalHeader.PhysicalMinimum.ToString(), BASIC_FIELD_LENGTH);
            }

            // Physical maximum
            foreach (SignalHeader signalHeader in header.SignalHeaders)
            {
                WriteField(_stream, signalHeader.PhysicalMaximum.ToString(), BASIC_FIELD_LENGTH);
            }

            // Digital minimum
            foreach (SignalHeader signalHeader in header.SignalHeaders)
            {
                WriteField(_stream, signalHeader.DigitalMinimum.ToString(), BASIC_FIELD_LENGTH);
            }

            // Digital maximum
            foreach (SignalHeader signalHeader in header.SignalHeaders)
            {
                WriteField(_stream, signalHeader.DigitalMaximum.ToString(), BASIC_FIELD_LENGTH);
            }

            // Prefiltering
            foreach (SignalHeader signalHeader in header.SignalHeaders)
            {
                WriteField(_stream, signalHeader.Prefiltering, LONG_FIELD_LENGTH);
            }

            // Number of samples per record for each signal.
            foreach (SignalHeader signalHeader in header.SignalHeaders)
            {
                WriteField(_stream, signalHeader.SampleCountPerRecord.ToString(), BASIC_FIELD_LENGTH);
            }

            // RESERVED bytes.
            foreach (SignalHeader _ in header.SignalHeaders)
            {
                WriteField(_stream, "RESERVED", SIGNAL_RESERVED_LENGTH);
            }
        }

        public void WriteSignals(List<Signal> signals, int numDataRecords)
        {
            IEnumerable<(Signal, int)> indexedSignals = signals.Select(signal => (signal, 0));

            // Write one data record at a time up to numDataRecords
            for (int i = 0; i < numDataRecords; i++)
            {
                indexedSignals = WriteDataRecord(indexedSignals);
            }
        }

        public IEnumerable<(Signal signal, int index)> WriteDataRecord(IEnumerable<(Signal signal, int index)> indexedSignals)
        {
            byte[] shortBytes;

            foreach ((Signal signal, int index) in indexedSignals)
            {
                int sampleCount = signal.SignalHeader.SampleCountPerRecord;

                for (int i = 0; i < sampleCount; i++)
                {
                    shortBytes = BitConverter.GetBytes(signal.Values[index + i]);
                    _stream.Write(shortBytes, 0, 2);
                }
            }

            // Update indexes for each signal we're writing.
            return indexedSignals.Select(indexedSignal =>
            {
                indexedSignal.index += indexedSignal.signal.SignalHeader.SampleCountPerRecord;
                return indexedSignal;
            });
        }

        private void WriteField(Stream stream, string field, int fieldLength)
        {
            // Truncate to field length, if necessary.
            if (field.Length > fieldLength)
            {
                _logger?.LogWarning("Field {field} length {fieldLength} exceeds limit, will be truncated to {limit}.",
                    field, field.Length, fieldLength);
                field = field.Substring(0, fieldLength);
            }

            // Pads field with spaces up to the required field length.
            field += new string(' ', fieldLength - field.Length);
            byte[] fieldBytes = Encoding.ASCII.GetBytes(field);
            Debug.Assert(fieldBytes.Length == fieldLength);
            stream.Write(fieldBytes, 0, fieldBytes.Length);
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Free underlying stream object.
                    _stream.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
