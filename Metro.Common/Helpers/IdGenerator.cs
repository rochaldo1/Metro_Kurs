namespace Metro.Common.Helpers;

public static class IdGenerator
{
    private static int _id;

    public static int NextId => ++_id;

    public static int CurrentId => _id;

    public static void SetLastId(int id) => _id = id;
}