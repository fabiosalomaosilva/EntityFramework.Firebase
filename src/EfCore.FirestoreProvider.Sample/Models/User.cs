using System;

namespace EfCore.FirestoreProvider.Sample.Models
{
    public class User
    {
        public string Id { get; set; } // Document ID
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
