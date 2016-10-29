public interface IIdentifiable<Tkey>
{
    /// <summary>
    /// Gets or sets the primary key for this entity.
    /// </summary>        
    Tkey Id { get; }
}
