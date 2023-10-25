using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStat : Stat
{
    [SerializeField] protected int _exp;
    [SerializeField] protected int _gold;

    public int Exp
    {
        get { return _exp; }
        set
        {
            _exp = value;
            //렙업 체크

            int level = _level;
            while (true)
            {
                Data.Stat stat;
                if (Managers.Data.StatDic.TryGetValue(level + 1, out stat) == false)
                    break;
                if (_exp < stat.totalExp)
                    break;
                    level++;
            }

            if (level != level)
            {
                Debug.Log("Level Up");
                Level = level;
                SetStat(Level);
            }
        }
    }
    public int Gold { get { return _gold; } set { _gold = value; } }
    
    private void Start()
    {
        _level = 1;
        _exp = 0;
        _defense = 5;
        _moveSpeed = 7.0f;
        _gold = 0;

        SetStat(_level);
    }

    private void SetStat(int level)
    {
        Dictionary<int, Data.Stat> dic = Managers.Data.StatDic;
        Data.Stat stat = dic[level];

        _hp = stat.maxHp;
        _maxHp = stat.maxHp;
        _attack = stat.attack;
    }
    

    protected override void OnDead(Stat attacker)
    {
        PlayerStat playerStat = attacker as PlayerStat;
        if (playerStat != null)
        {
            playerStat.Exp += 15;
        }
        Managers.Game.Despawn(gameObject);
    }
    
}
