
[System.Serializable]
public class Level
{
    public Level(int _number)
    {
        Number              = _number;
        CanPlay             = false;
        MaxCollectableHints = 1;
    }
    
    public int MaxCollectableHints {get; set;}
    public bool CanPlay {get; set;}
    public int Number {get;}
    public bool Completed {get; set;}
}