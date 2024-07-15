
using System.Numerics;

public class _Player
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Power { get; set; }
    public float AttackSpeed { get; set; }
    public float Health { get; set; }    
    public float Defense { get; set; }   
    public float MoveSpeed { get; set; } 
}

public class _Enemy
{
    public int Id { get; set; }
    public string Name { get; set; }
    public float Power { get; set; }
    public float CoolTime { get; set; }
    public float Health { get; set; }
    public float Defense { get; set; }
    public float MoveSpeed { get; set; }
    public float TraceSpeed { get; set; }
    public float ReturnSpeed { get; set; }
    public float DetectionRadius { get; set; }
}

public class _PlayerSkill
{
    public int Id { get; set; }
    public float Power { get; set; }
    public int Delay { get; set; }
    public int CoolTime { get; set; }
    public int Range { get; set; }
}

public class _EnemySkill
{
    public int Id { get; set; }
    public float CoolTime { get; set; }
    public float Power { get; set; }
    public int Projectile_Id { get; set; }
    public float Range { get; set; }
}

public class _EnemyProjectile
{
    public int Id { get; set; }
    public float Scale { get; set; }
    public float Speed { get; set; }
    public float Angle { get; set; }
    public float LifeTime { get; set; }
}