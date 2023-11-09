using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Clone : Skill
{
    [Header("Clone Info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;

    [SerializeField] private bool cloneOnDashStart;
    [SerializeField] private bool cloneOnDashEnd;
    [SerializeField] private bool cloneOnCounterAttack;
    
    [Header("Clone Duplication")]
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;

    
    public void CreateClone(Transform _clonePosition, Vector3 _offset)
    {
        GameObject newClone = Instantiate(clonePrefab);
        
        newClone.GetComponent<Skill_Clone_Controller>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform), canDuplicateClone, chanceToDuplicate);        
    }

    public void CreateCloneOnDashStart()
    {
        if (cloneOnDashStart)
        {
            CreateClone(player.transform, Vector3.zero);
        }
    }

    public void CreateCloneOnDashEnd()
    {
        if (cloneOnDashEnd)
            CreateClone(player.transform, Vector3.zero);
    }

    public void CreateCloneOnCounterAttack(Transform _enemyTransform)
    {
        if (cloneOnCounterAttack)
            StartCoroutine(CreateCloneWithDelay(_enemyTransform, new Vector3(1 * player.facingDir, 0)));
    }

    private IEnumerator CreateCloneWithDelay(Transform _enemyTransform, Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
            CreateClone(_enemyTransform, _offset);            
    }
    
    //public void CreateCloneOnStart(Transform _clonePosition, Vector3 _offset)
    //{
    //    if (cloneOnDashStart)
    //    {
    //        GameObject newClone = Instantiate(clonePrefab);

    //        newClone.GetComponent<Skill_Clone_Controller>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform));
    //    }
    //}


    //public void CreateCloneOnEnd(Transform _clonePosition, Vector3 _offset)
    //{
    //    if (cloneOnDashEnd)
    //    {
    //        GameObject newClone = Instantiate(clonePrefab);

    //        newClone.GetComponent<Skill_Clone_Controller>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform));
    //    }
    //}

    //public void CreateCloneOnCounterAttack(Transform _clonePosition, Vector3 _offset)
    //{
    //    if (cloneOnCounterAttack)
    //        StartCoroutine(CreatecloneWithDelay(_clonePosition, _offset));
    //}

    //private IEnumerator CreatecloneWithDelay(Transform _clonePosition, Vector3 _offset)
    //{
    //    yield return new WaitForSeconds(.3f);
    //    GameObject newClone = Instantiate(clonePrefab);
    //    newClone.GetComponent<Skill_Clone_Controller>().SetupClone(_clonePosition, cloneDuration, canAttack, _offset, FindClosestEnemy(newClone.transform));
    //}
}
