using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    private Stat _stat;
    [SerializeField] private float _scanRange = 8;
    [SerializeField] private float _attackRange = 1;
    void Start()
    {
        WorldObjectType = Define.WorldObject.Monster;
        _stat = gameObject.GetComponent<Stat>();

        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);
    }

    protected override void UpdateDie()
    {
        Debug.Log("Monster. UpdateDie");
    }

    protected override void UpdateMoving()
    {
        //플레이어가 내 사정거리보다 가까우면 공격
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude;
            if (distance <= _attackRange)
            {
                NavMeshAgent nma2 = gameObject.GetOrAddComponent<NavMeshAgent>();
                nma2.SetDestination(transform.position);
                State = Define.State.Skill;
                return;
            }
        }
        
        //목적지 도착하면 정지
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude < 0.1f)
        {
            State = Define.State.Idle;
            return;
        }
        
        //이동
        NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();
        nma.SetDestination(_destPos);
        nma.speed = _stat.MoveSpeed;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
    }

    protected override void UpdateIdle()
    {
        Debug.Log("Monster updateIdle");
        GameObject player = Managers.Game.GetPlayer();
        if (player == null)
            return;

        float distance = (player.transform.position - transform.position).magnitude;
        if (distance <= _scanRange)
        {
            _lockTarget = player;
            State = Define.State.Moving;
            return;
        }
    }

    protected override void UpdateSkill()
    {
        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }
    
    void OnHitEvent()
    {

        Managers.Sound.Play("UnityChan/univ0001", Define.Sound.Effect);
        
        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
            targetStat.OnAttacked(_stat);

            if (targetStat.Hp > 0)
            {
                float distance = (_destPos - transform.position).magnitude;
                if (distance <= _attackRange)
                    State = Define.State.Skill;
                else 
                    State = Define.State.Moving;
            }
            else
            {
                State = Define.State.Idle;   
            }
        }
        else
        {
            State = Define.State.Idle;
        }
        
        
    }
    
    
}
