namespace StudyFlow.Models
{
    public class Room
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public int Capacity { get; set; }

        // Kan være null fordi beskrivelse av rommet ikke er obligatorisk.
        public string? Description { get; set; }
    }
}