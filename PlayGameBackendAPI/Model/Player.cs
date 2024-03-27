namespace PlayGameBackendAPI.Model
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public int Points { get; set; } = 10000;
    }
}
