namespace Host.Models
{
    public class ScheduleModel
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }       
        public string From { get; set; }
        public string To { get; set; }
    }
}
