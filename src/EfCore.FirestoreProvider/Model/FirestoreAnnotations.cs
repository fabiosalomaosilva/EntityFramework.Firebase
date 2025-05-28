using System;

namespace EfCore.FirestoreProvider.Model
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class FirestoreCollectionAttribute : Attribute
    {
        public string CollectionName { get; }

        public FirestoreCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FirestoreDocumentIdAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FirestoreIgnoreAttribute : Attribute
    {
    }
}