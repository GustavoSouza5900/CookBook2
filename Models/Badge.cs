namespace CookBook.Models
{
    public class Badge
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconClass { get; set; } = "fas fa-medal"; // Ex: FontAwesome
        public string TriggerEvent { get; set; } = string.Empty; // Ex: "PUBLISH_10_RECIPES"
    }
}