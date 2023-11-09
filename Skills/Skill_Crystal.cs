using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Crystal : Skill
{
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [Header("Crystal Mirage")]
    [SerializeField] private bool cloneReplaceCrystal;

    [Header("Explosive Crystal")]
    [SerializeField] private bool canExplode;

    [Header("Moving Crystal")]
    [SerializeField] private bool canMoveToEnemy;
    [SerializeField] private float moveSpeed;

    [Header("Multi Crystal")]
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int amountOfStacks;
    [SerializeField] private float multiStackCooldown;
    [SerializeField] private float useTimeWindow;
    [SerializeField] private List<GameObject> remainingCrystals = new List<GameObject>();



    public override void UseSkill()
    {
        base.UseSkill();

        if (CanUseMultiCrystal())
            return;

        if (currentCrystal == null)
        {
            currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
            Skill_Crystal_Controller currentCrystalScript = currentCrystal.GetComponent<Skill_Crystal_Controller>();

            currentCrystalScript.SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(currentCrystal.transform));
        }
        else
        {
            if (canMoveToEnemy)
                return; // Disable teleport if the crystal can move toward enemy and explode

            // The next three lines happen irrespective of booleans
            Vector2 playerPos = player.transform.position;
            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;

            if (cloneReplaceCrystal)
            {
                SkillManager.instance.clone.CreateClone(currentCrystal.transform, Vector3.zero);                
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<Skill_Crystal_Controller>()?.FinishCrystal();
            }
        }
    }

    private bool CanUseMultiCrystal()
    {
        if (canUseMultiStacks)
        {
            if (remainingCrystals.Count > 0)
            {
                if (remainingCrystals.Count == amountOfStacks)
                    Invoke("ResetAbility", useTimeWindow);

                cooldown = 0;
                GameObject crystalToSpawn = remainingCrystals[remainingCrystals.Count - 1]; //Choose last crystal in list
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, Quaternion.identity); // Respawn a new crystal

                remainingCrystals.Remove(crystalToSpawn); // Remove crystal from list

                newCrystal.GetComponent<Skill_Crystal_Controller>().SetupCrystal(crystalDuration, canExplode, canMoveToEnemy, moveSpeed, FindClosestEnemy(newCrystal.transform));

                if (remainingCrystals.Count <= 0)
                {
                    // cooldown the skill
                    cooldown = multiStackCooldown;
                    // refill crystals
                    RefillCrystal();
                }
            
                return true;
            }
            // respawn crystal
        }
        return false;
    }

    private void RefillCrystal()
    {
        int amountToAdd = amountOfStacks - remainingCrystals.Count;
        
        for (int i = 0; i < amountToAdd; i++)
        {
            remainingCrystals.Add(crystalPrefab);
        }
    }

    /// <summary>
    /// Resets the multi crystal ability after a set period if all crystals were not used
    /// </summary>
    private void ResetAbility()
    {
        if (cooldownTimer > 0)
            return;

        cooldownTimer = multiStackCooldown;
        RefillCrystal();
    }
}
