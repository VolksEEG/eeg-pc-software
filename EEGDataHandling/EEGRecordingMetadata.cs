using System;

namespace EEGDataHandling
{
    /// <summary>
    /// Container class for metadata concerning an EEG recording session,
    /// such as patient ID, location, etc.
    /// Currently just contains whatever is required to support EDF files;
    /// it may be useful to break this class up and/or add more fields, depending
    /// on where else in the application these properties are needed, when they're
    /// entered, etc.
    /// </summary>
    public class EEGRecordingMetadata
    {
        public EEGRecordingMetadata()
        {

        }

        // Note from EDF spec (https://www.edfplus.info/specs/edf.html):
        // "The duration of each data record is recommended to be a whole number of seconds 
        // and its size (number of bytes) is recommended not to exceed 61440."
        public int RecordDurationInSeconds { get; set; } = 1;
        public DateTimeOffset StartTime { get; set; }

        // Note from EDF+ spec (https://www.edfplus.info/specs/edfplus.html#edfplusandedf):
        // 3. The 'local patient identification' field must start with the subfields (subfields do not contain, but are separated by, spaces):
        // - the code by which the patient is known in the hospital administration.
        // - sex(English, so F or M).
        // - birthdate in dd-MMM-yyyy format using the English 3-character abbreviations of the month in capitals. 02-AUG-1951 is OK, while 2-AUG-1951 is not.
        // - the patients name.
        // Any space inside the hospital code or the name of the patient must be replaced by a different character, for instance an underscore.For instance, the 'local patient identification' field could start with: MCH-0234567 F 02-MAY-1951 Haagse_Harry.Subfields whose contents are unknown, not applicable or must be made anonymous are replaced by a single character 'X'. So, if everything is unknown then the 'local patient identification' field would start with: 'X X X X'. Additional subfields may follow the ones described here.
        public string PatientID { get; set; }

        // Notes from EDF+ spec (https://www.edfplus.info/specs/edfplus.html#edfplusandedf):
        // 4. The 'local recording identification' field must start with the subfields (subfields do not contain, but are separated by, spaces):
        // - The text 'Startdate'.
        // - The startdate itself in dd-MMM-yyyy format using the English 3-character abbreviations of the month in capitals.
        // - The hospital administration code of the investigation, i.e.EEG number or PSG number.
        // - A code specifying the responsible investigator or technician.
        // - A code specifying the used equipment. 
        public string RecordingID { get; set; }
    }
}
