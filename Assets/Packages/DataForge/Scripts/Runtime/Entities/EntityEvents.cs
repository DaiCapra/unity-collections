namespace DataForge.Entities
{
    public static class EntityEvents
    {
        public delegate void EntityHierarchyHandler();

        public static EntityHierarchyHandler EntityCollectionChanged { get; set; }
    }
}