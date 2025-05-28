using System;

namespace EfCore.FirestoreProvider.Sample.Models
{
    public class Ticket
    {
        public string Id { get; set; } // Document ID
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } // Reference to the User who created the ticket
    }
}
