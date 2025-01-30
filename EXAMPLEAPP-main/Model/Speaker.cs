namespace ITB2203Application.Model
{
    public class Speaker
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
		public ICollection<Event> Events { get; set; }
	}
}
