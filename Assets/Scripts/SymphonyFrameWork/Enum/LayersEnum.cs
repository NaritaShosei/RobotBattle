using System;

[Flags]
public enum LayersEnum : int
{
    None = 1 << 0,
    Default = 1 << 1,
    TransparentFX = 1 << 2,
    Water = 1 << 3,
    UI = 1 << 4,
    Ground = 1 << 5,
    Player = 1 << 6,
    PlayerBullet = 1 << 7,
    EnemyBullet = 1 << 8,
    Enemy = 1 << 9,
    SmallEnemy = 1 << 10,
}
