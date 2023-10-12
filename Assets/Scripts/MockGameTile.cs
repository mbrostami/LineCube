public class MockGameTile : GameTile
{
    public int InPort { get; set; }
    public bool Initialized { get; set; }
}

/**
 *
 *
 * switch (type)
        {
            case 1:
                return new int[4] { 1, 2, 3, 4 };
            case 2:
                return new int[4] { 1, 2, 3, 0 };
            case 3:
                return new int[4] { 1, 2, 0, 0 };
            case 4:
                return new int[4] { 1, 0, 3, 0 };
            case 5:
                return new int[4] { 1, 0, 0, 4 };
            case 6:
                return new int[4] { 1, 0, 3, 4 };
            case 7:
                return new int[4] { 0, 2, 3, 4 };
            case 8:
                return new int[4] { 0, 2, 3, 0 };
            case 9:
                return new int[4] { 0, 2, 0, 4 };
            case 10:
                return new int[4] { 0, 0, 3, 4 };
            default:
                return new int[4] { 0, 0, 0, 0 };
        }

    */
    