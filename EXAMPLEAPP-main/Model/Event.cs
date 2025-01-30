namespace ITB2203Application.Model
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SpeakerId { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
		public DateTime Time { get; internal set; }
	}
}
