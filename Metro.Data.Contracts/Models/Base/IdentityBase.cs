namespace Metro.Data.Contracts.Models.Base;

/// <summary>
/// Базовый класс с идентификатором
/// </summary>
public abstract class IdentityBase
{
    /// <summary>
    /// Идентификатор объекта
    /// </summary>
    public int Id { get; set; }
}