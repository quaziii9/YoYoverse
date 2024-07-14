
public class _Player
{
    public int _playerId { get; set; }
    public string _playerName { get; set; }
    public float _attackPower { get; set; }
    public float _attackSpeed { get; set; }
    public float _playerHealth { get; set; }    
    public float _playerDefense { get; set; }   
    public float _playerMoveSpeed { get; set; } 
    public string _playerType { get; set; } 
}

public class _Enemy
{
    public int _enemyId { get; set; }
    public string _enemyName { get; set; }
    public float _attackPower { get; set; }
    public float _attackCoolTime { get; set; }
    public float _enemyHealth { get; set; }
    public float _enemyDefense { get; set; }
    public float _enemyMoveSpeed { get; set; }
    public int _skillId { get; set; }
}

public enum Target
{
    Single,
    Multi
}

public class _PlayerSkill
{
    public int _skillId { get; set; }
    public string _skillName { get; set; }
    public float _skillPower { get; set; }
    public float _skillCoolTime { get; set; }
   
}