namespace instantMessagingClient.Database
{
    //used when sending/receiving messages
    public class MyMessages
    {
        public int Id { get; set; }

        public int IdEnvoyeur { get; set; }

        public string message { get; set; }

        public MyMessages(int idEnvoyeur, string message)
        {
            IdEnvoyeur = idEnvoyeur;
            this.message = message;
        }

        public MyMessages()
        {
        }

        public MyMessages(int id, int idEnvoyeur, string message)
        {
            Id = id;
            IdEnvoyeur = idEnvoyeur;
            this.message = message;
        }
    }
}
