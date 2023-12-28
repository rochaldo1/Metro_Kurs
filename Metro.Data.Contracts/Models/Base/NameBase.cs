namespace Metro.Data.Contracts.Models.Base;

/// <summary>
/// Базовый класс с именем
/// </summary>
public abstract class NameBase : IdentityBase
{
    /// <summary>
    /// Наименование
    /// </summary>
    public string Name { get; set; }
}