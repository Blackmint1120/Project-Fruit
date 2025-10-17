public static class GameLayers
{
    public static int Fruit => UnityEngine.LayerMask.NameToLayer("Fruit");
    public static int Walls => UnityEngine.LayerMask.NameToLayer("Walls");
    public static int UIBlock => UnityEngine.LayerMask.NameToLayer("UIBlock");

    public static int MaskBlockingFruit =>
        (1 << Walls) | (1 << Fruit); // 필요 시 확장
}