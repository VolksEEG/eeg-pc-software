namespace EEGMachine.Interfaces
{
    public interface ITimeRange
    {
        public long StartTime { get; set; }
        public long EndTime { get; }
        public long Duration { get; }
    }
}
