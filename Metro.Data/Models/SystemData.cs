namespace Metro.Data.Models;

/// <summary>
/// Системные данные для загрузки данных ядра приложения
/// </summary>
internal class SystemData
{
    public TimeSpan CurrentTime { set; get; }
    public int LastCoreId { set; get; }
}