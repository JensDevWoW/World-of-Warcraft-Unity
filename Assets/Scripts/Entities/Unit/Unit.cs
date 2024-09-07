/*
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation; either version 2 of the License, or (at your
 * option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using Mirror;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Android;
using static UnityEngine.UI.CanvasScaler;

public enum Stats
{
    Strength = 2,
    Stamina = 100,
    Intellect = 50,
    Agility = 2
}

public class UnitState
{
    public const int UNIT_STATE_ROOTED = 1;
    public const int UNIT_STATE_DISORIENTED = 2;
    public const int UNIT_STATE_STUNNED = 3;
    public const int UNIT_STATE_CASTING = 4;
}
public class Unit : MonoBehaviour
{
    // Reference to the NetworkIdentity component
    public NetworkIdentity Identity { get; private set; }
    public LocationHandler location {  get; private set; }

    private List<Spell> spellList = new List<Spell>();
    private List<Aura> auraList = new List<Aura>();
    private List<Unit> combatList = new List<Unit>();
    private List<AreaTrigger> areaTriggers = new List<AreaTrigger>();

    // Data
    public string m_name = "Voreli";
    public float m_health = 1000;
    public float m_maxHealth = 1000;
    public float m_mana = 100;
    public float m_maxMana = 100;
    private float m_manaTickTimer = 1;
    private float m_manaTickAmount = 7;
    private float m_manaTickAmountCombat = 3;
    private bool m_isAlive = true;
    private float m_combatTimer = 0;
    private float m_absorbAmount = 0;
    private bool m_isCasting;
    private List<int> unitStates;
    private Vector3 lastPosition;
    private Spell m_channeledSpell = null;
    private bool isMoving;
    private float movementThreshold = 0.01f; // The threshold for detecting movement
    private int m_channeledSpellId = 0;
    public CharacterController charController { get; private set; }
    public Player player { get; private set; }
    public LocationHandler locationHandler { get; protected set; }
    public Unit m_target { get; protected set; }

    public List<UnitSpell> spellBook; // List of spell IDs the Unit knows
    public AnimationHandler animHandler {  get; protected set; }
    public Creature creature {  get; protected set; }
    
    void Start()
    {
        // Get the NetworkIdentity component attached to the same GameObject
        Identity = GetComponent<NetworkIdentity>();
        locationHandler = LocationHandler.Instance;
        animHandler = GetComponent<AnimationHandler>();
        creature = GetComponent<Creature>();
        charController = gameObject.GetComponent<CharacterController>();
        lastPosition = transform.position;

        if (creature != null)
            creature.Init();

        if (Identity == null)
        {
            print("NetworkIdentity component is missing on the Unit's GameObject.");
        }

        if (spellBook == null)
            spellBook = new List<UnitSpell>();

        player = GetComponent<Player>();

        unitStates = new List<int>(); // Need to add the class, gotta wait for it to load, slow pc

        FillKnownSpells();
    }

    public void SendUpdateCharges(int spellId, int stacks)
    {
        NetworkWriter writer = new NetworkWriter();

        writer.WriteNetworkIdentity(Identity);
        writer.WriteInt(spellId);
        writer.WriteInt(stacks);

        OpcodeMessage packet = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_UPDATE_CHARGES,
            payload = writer.ToArray()
        };

        NetworkServer.SendToAll(packet);
    }

    // This is trash, just a test case, eventually need to add in a SpellBook class
    public void FillKnownSpells()
    {
        List<int> spells = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

        foreach (int spell in spells)
        {
            SpellInfo info = SpellContainer.Instance.GetSpellById(spell);
            if (info != null)
            {
                UnitSpell unitSpell = SpellContainer.Instance.Convert(info);
                if (unitSpell != null)
                {
                    spellBook.Add(unitSpell);
                }
            }
        }
    }

    public void SetChanneledSpell(Spell spell)
    {
        m_channeledSpell = spell;
    }

    public void SendStateUpdate(int state, bool apply)
    {
        NetworkWriter writer = new NetworkWriter();

        writer.WriteNetworkIdentity(Identity);
        writer.WriteInt(state);
        writer.WriteBool(apply); // Is it being added or removed;

        OpcodeMessage packet = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_UPDATE_UNIT_STATE,
            payload = writer.ToArray()
        };

        NetworkServer.SendToAll(packet);
    }

    public Spell GetChanneledSpell()
    {
        return m_channeledSpell;
    }

    public void InterruptChanneled()
    {
        Spell currentSpell = GetChanneledSpell();
        if (currentSpell != null)
        {
            currentSpell.Cancel();
            SetChanneledSpellId(0);
        }
    }

    public void AddUnitState(int state)
    {
        if (!unitStates.Contains(state))
        {
            unitStates.Add(state);

            // Send opcode
            SendStateUpdate(state, true);
        }
    }

    public void SetChanneledSpellId(int spellId)
    {
        m_channeledSpellId = spellId;
    }

    public float GetGCDTime()
    {
        Player plr = ToPlayer();
        if (!player)
            return 99f; // Not a player

        return plr.GetGCDTime();
    }
    public void RemoveUnitState(int state)
    {
        if (unitStates.Contains(state))
        {
            unitStates.Remove(state);
            SendStateUpdate(state, false);
        }
    }

    public bool HasUnitState(int state)
    {
        return unitStates.Contains(state);
    }

    public float GetMana()
    {
        return m_mana;
    }

    public void SetMana(float val)
    {
        m_mana = val;
    }

    public List<Spell> GetSpellList()
    {
        return spellList;
    }

    public float GetMaxMana()
    {
        return m_maxMana;
    }

    public void CancelCast(Spell spell)
    {
        foreach (Spell sp in spellList)
        {
            if (sp == spell)
                spellList.Remove(sp);
            StopCasting();
            SendCancelCastOpcode(spell);
        }
    }

    public void CancelLCast(int spellId)
    {
        foreach (Spell sp in spellList)
        {
            if (sp.spellId == spellId)
                spellList.Remove(sp);
            StopCasting();
            SendCancelCastOpcode(sp);
        }
    }

    public void SendCancelCastOpcode(Spell spell)
    {
        NetworkWriter writer = new NetworkWriter();

        writer.WriteNetworkIdentity(Identity);
        writer.WriteInt(spell.m_spellInfo.Id);

        OpcodeMessage packet = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_CAST_CANCELED,
            payload = writer.ToArray()
        };

        NetworkServer.SendToAll(packet);
    }

    public bool IsInCombat()
    {
        return m_combatTimer > 0; 
    }

    public void DropCombat()
    {
        m_combatTimer = 0;
    }

    public void SetAbsorbAmount(float amount)
    {
        m_absorbAmount = amount;
    }

    public float GetAbsorbAmount()
    {
        return m_absorbAmount;
    }

    public void SetInCombat()
    {
        m_combatTimer = 6;
    }

    public int GetMap()
    {
        return 1;
    }

    public float GetCastedSpellTimeLeft()
    {
        foreach (Spell spell in spellList)
        {
            if (spell.GetSpellState() == Spell.SPELL_STATE_PREPARING)
                return spell.GetCastTimeLeft();
        }
        return 0f;
    }

    public void SetInCombatWith(Unit unit)
    {
        if (!IsHostileTo(unit))
            print("ERROR: Unit is not an enemy!");

        SetInCombat();
        unit.SetInCombat();

        // We add eachother to our combat lists so both go into combat, not just one unit
        combatList.Add(unit);
        unit.combatList.Add(this);

    }

    public void SendCooldownOpcode(int spellId, float duration)
    {
        NetworkWriter writer = new NetworkWriter();

        writer.WriteNetworkIdentity(Identity);
        writer.WriteInt(spellId);
        writer.WriteFloat(duration);

        OpcodeMessage packet = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_SPELL_COOLDOWN,
            payload = writer.ToArray()
        };

        NetworkServer.SendToAll(packet);
    }

    public bool IsOnCooldown(int spellId)
    {
        foreach (var spell in spellBook)
        {
            if (spell.SpellInfo.Id == spellId)
                return spell.currentCooldownTime > 0;
        }

        return false;
    }

    public void StartCooldown(int spellId)
    {
        foreach (var spell in spellBook)
        {
            if (spell.SpellInfo.Id == spellId)
                spell.StartCooldown();
        }
    }

    public bool IsCooldownActive(int spellId)
    {
        foreach (var spell in spellBook)
        {
            if (spell.SpellInfo.Id == spellId)
                return spell.IsCooldownActive();    
        }

        return false;
    }

    public float GetRemainingCooldown(int spellId)
    {
        foreach (var spell in spellBook)
        {
            if (spell.SpellInfo.Id == spellId)
                return spell.GetRemainingCooldown();
        }

        return 0f;
    }

    public void DropCharge(int spellId)
    {
        foreach (var spell in spellBook)
        {
            if (spell.SpellInfo.Id == spellId)
            {
                spell.UpdateStacks(spell.Stacks - 1);
                SendUpdateCharges(spellId, spell.Stacks);
            }
        }
    }

    public void AddCharge(int spellId)
    {
        foreach (var spell in spellBook)
        {
            if (spell.SpellInfo.Id == spellId)
                if (spell.Stacks < spell.SpellInfo.Stacks)
                {
                    spell.UpdateStacks(spell.Stacks + 1);
                    SendUpdateCharges(spellId, spell.Stacks);
                    
                    if (spell.Stacks + 1 < spell.SpellInfo.Stacks)
                    {
                        spell.StartCooldown();
                    }
                }
                else
                {
                    spell.ResetCooldown();
                }
        }
    }

    public int GetCharges(int spellId)
    {
        foreach (var spell in spellBook)
        {
            if (spell.SpellInfo.Id == spellId)
                return spell.Stacks;
        }

        return 1000;
    }


    private void UpdateCooldown(UnitSpell spell)
    {
        spell.currentCooldownTime -= Time.deltaTime;

        // Check if the cooldown has completed
        if (spell.currentCooldownTime <= 0)
        {
            spell.currentCooldownTime = 0;
            OnCooldownComplete(spell);
        }
    }

    private void OnCooldownComplete(UnitSpell spell)
    {
        // Check if the spell has stacks to update
        if (spell.HasStacks())
        {
            spell.UpdateStacks(spell.Stacks + 1);
            SendUpdateCharges(spell.SpellInfo.Id, spell.Stacks);

            // Restart cooldown if the spell hasn't reached max stacks
            if (spell.Stacks < spell.SpellInfo.Stacks)
            {
                RestartCooldown(spell);
            }
        }
    }

    // Method to restart the cooldown and send cooldown information
    private void RestartCooldown(UnitSpell spell)
    {
        spell.StartCooldown();
        SendCooldownOpcode(spell.SpellInfo.Id, spell.SpellInfo.Cooldown);
    }


public void Update()
    {
        if (GetHealth() <= 0)
        {
            SetHealth(0);
            m_isAlive = false;
            StopCasting();
            //WipeAuras();
            //if (ToCreature())
              //  ToCreature().Died();
        }



        // We update on Unit function so we can access certain data without needing to backtrack
        foreach (var spell in spellBook)
        {
            // Check if the spell's cooldown is active
            if (spell.currentCooldownTime > 0)
            {
                UpdateCooldown(spell);
            }
        }

        CheckMovement();
        //target stealth update
        /*if (HasTarget())
            if (GetTarget().IsInStealth())
                DropTarget();*/

        // Update Schools

        bool preparing = false;
        for (int i = 0; i < spellList.Count; i++)
        {
            if (spellList[i].GetSpellState() == Spell.SPELL_STATE_PREPARING)
            {
                preparing = true;
                SetCasting();
                break;
            }
        }

        if (!preparing)
            StopCasting();

        // Mana Tick
        if (GetMana() < GetMaxMana())
        {
            if (m_manaTickTimer > 0)
            {
                m_manaTickTimer -= Time.deltaTime;
            }
            else
            {
                if (IsInCombat())
                {
                    // Check if adding combat mana tick amount exceeds max mana
                    if (GetMana() + m_manaTickAmountCombat < GetMaxMana())
                    {
                        SetMana(GetMana() + m_manaTickAmountCombat);
                    }
                    else
                    {
                        SetMana(GetMaxMana());
                    }
                }
                else
                {
                    // Check if adding normal mana tick amount exceeds max mana
                    if (GetMana() + m_manaTickAmount < GetMaxMana())
                    {
                        SetMana(GetMana() + m_manaTickAmount);
                    }
                    else
                    {
                        SetMana(GetMaxMana());
                    }
                }

                // Reset the mana tick timer
                m_manaTickTimer = 1f;
            }
        }

        if (IsInCombat())
        {
            // If there are entries in the combat list
            if (combatList.Count > 0)
            {
                // Iterate through the combat list
                for (int i = combatList.Count - 1; i >= 0; i--)
                {
                    if (!combatList[i].IsAlive())
                    {
                        combatList.RemoveAt(i);
                    }
                }

                // If the combat list is empty after cleaning up
                if (combatList.Count == 0)
                {
                    DropCombat();
                }
            }

            // Decrement the combat timer
            m_combatTimer -= Time.deltaTime;

            // If the combat timer has run out
            if (m_combatTimer <= 0f)
            {
                DropCombat();
                print("Out of combat!");
            }
        }



    }
    public void AddSpellToList(Spell spell)
    {
        spellList.Add(spell);
    }

    public Spell CreateSpellAndPrepare(int spellId, GameObject spellPrefab, GameObject triggerPrefab)
    {
        SpellInfo info = SpellDataHandler.Instance.Spells.FirstOrDefault(spell => spell.Id == spellId);

        // Instantiate the prefab instead of creating a new GameObject
        GameObject spellObject = Instantiate(spellPrefab);

        // Set the name of the cloned object if needed
        spellObject.name = $"{spellId}";

        // Get the Spell component from the instantiated prefab
        Spell newSpell = spellObject.GetComponent<Spell>();

        DontDestroyOnLoad(spellObject);

        newSpell.Initialize(spellId, this, info, triggerPrefab);
        //SpellManager.Instance.AddSpell(newSpell);

        newSpell.prepare();

        AddSpellToList(newSpell);
        return newSpell;

        // Network-related: If you need the spell object to be networked
        //NetworkServer.Spawn(spellObject);
    }

    public Spell CreateSpellAndPrepare(int spellId, GameObject spellPrefab, GameObject triggerPrefab, Vector3 position)
    {
        SpellInfo info = SpellDataHandler.Instance.Spells.FirstOrDefault(spell => spell.Id == spellId);

        // Instantiate the prefab instead of creating a new GameObject
        GameObject spellObject = Instantiate(spellPrefab);

        // Set the name of the cloned object if needed
        spellObject.name = $"{spellId}";

        // Get the Spell component from the instantiated prefab
        Spell newSpell = spellObject.GetComponent<Spell>();

        DontDestroyOnLoad(spellObject);

        newSpell.Initialize(spellId, this, info, triggerPrefab);
        newSpell.position = position;

        newSpell.prepare();

        AddSpellToList(newSpell);
        return newSpell;

        // Network-related: If you need the spell object to be networked
        //NetworkServer.Spawn(spellObject);
    }

    public void CreateAreaTriggerAndActivate(Spell spell, Unit target, int spellId, GameObject triggerPrefab)
    {
        AreaTriggerInfo info = AreaTriggerDataHandler.Instance.AreaTriggers.FirstOrDefault(spell => spell.Id == spellId);

        // Instantiate the prefab instead of creating a new GameObject
        GameObject triggerObject = Instantiate(triggerPrefab);

        // Set the name of the cloned object if needed
        triggerObject.name = $"{spellId}";

        // Get the Spell component from the instantiated prefab
        AreaTrigger newTrigger = triggerObject.GetComponent<AreaTrigger>();

        DontDestroyOnLoad(triggerObject);

        newTrigger.Initialize(spell.caster, spell, info);
        //SpellManager.Instance.AddSpell(newSpell);

        newTrigger.Activate();

        spell.caster.AddTriggerToList(newTrigger);
    }

    public void AddTriggerToList(AreaTrigger newTrigger)
    {
        areaTriggers.Add(newTrigger);
    }
    public float GetHealth()
    {
        return m_health;
    }

    public void SetHealth(float val)
    {
        m_health = val; 
    }

    public float GetMaxHealth()
    {
        return m_maxHealth;
    }

    public void RemoveSpellFromList(Spell spell)
    {
        for (int i = 0; i < spellList.Count; i++)
        {
            if (spellList[i] == spell)
            {
                spellList.RemoveAt(i);
                return;
            }
        }
    }

    // Method to set the unit as casting
    public void SetCasting()
    {
        m_isCasting = true;
    }

    public bool IsCasting()
    {
        return m_isCasting;
    }

    // Method to stop the unit from casting
    public void StopCasting()
    {
        m_isCasting = false;
    }

    // Get the target unit (this can be adapted based on your actual target acquisition logic)
    public Unit GetTarget()
    {
        return m_target;
    }

    public void SetTarget(Unit target)
    {
        this.m_target = target;
        UpdateTargetClient();
    }

    public void UpdateTargetClient()
    {
        if (m_target != null)
        {
            NetworkWriter writer = new NetworkWriter();

            writer.WriteNetworkIdentity(this.Identity);
            writer.WriteNetworkIdentity(m_target.Identity);
            writer.WriteFloat(m_target.GetHealth());
            writer.WriteFloat(m_target.GetMaxHealth());
            writer.WriteFloat(m_mana); // TODO: Mana client-side
            writer.WriteFloat(m_maxMana);

            OpcodeMessage packet = new OpcodeMessage
            {
                opcode = Opcodes.SMSG_UPDATE_TARGET,
                payload = writer.ToArray()
            };

            NetworkServer.SendToAll(packet);
        }
    }

    public bool HasTarget()
    { return m_target != null; }

    // Check if this unit is hostile to the given target
    public bool IsHostileTo(Unit target)
    {
        return target != this; // TODO: Replace with actual hostility logic
    }

    // Convert this Unit to a Player if applicable
    public Player ToPlayer()
    {
        return player;
    }

    public LocationHandler ToLocation()
    {
        return locationHandler;
    }

    // Check if this unit is the local player
    public bool IsLocalPlayer()
    {
        return Identity != null && Identity.isLocalPlayer;
    }

    public bool IsAlive()
    {
        return m_isAlive;
    }

    public virtual Unit ToUnit()
    {
        return this;
    }

    public void InitAura(Aura aura)
    {
        auraList.Add(aura);
    }

    public void DestAura(Aura aura)
    {
        if (auraList.Contains(aura))
            auraList.Remove(aura);
        else
            print("Aura does not exist!");
    }

    public void AddAura(int spellId)
    {
        CastSpell(spellId, this);
    }

    public void InitBars(NetworkIdentity identity)
    {
        NetworkWriter writer = new NetworkWriter();

        writer.WriteNetworkIdentity(identity);

        OpcodeMessage packet = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_INIT_BARS,
            payload = writer.ToArray()
        };

        NetworkServer.SendToAll(packet);
    }

    public void AddAura(Aura aura)
    {
        CastSpell(aura.auraInfo.Id, this);
    }
    public void RemoveAura(int spellId)
    {
        Aura auraToRemove = null;
        foreach (Aura aura in auraList)
        {
            if (aura.auraInfo.Id == spellId)
            {
                auraToRemove = aura;
                break;
            }
        }

        if (auraToRemove != null)
        {
            auraToRemove.Finish();
        }
    }

    public bool HasAuraFrom(Spell spell)
    {
        foreach (Aura aura in auraList)
        {
            if (aura.auraInfo.Id == spell.spellId)
                return true;
        }

        return false;
    }

    public void RemoveAura(Aura aura)
    {
        if (auraList.Contains(aura))
            auraList.Remove(aura);
    }

    public List<Aura> GetAuras()
    {
        return auraList;
    }

    public Spell CastSpell(int spellId, Unit target)
    {
        GameNetworkManager GNM = FindObjectOfType<GameNetworkManager>();
        GameObject spellPrefab = GNM.spellPrefab;
        

        SpellInfo info = SpellDataHandler.Instance.Spells.FirstOrDefault(spell => spell.Id == spellId);

        // Instantiate the prefab instead of creating a new GameObject
        GameObject spellObject = Instantiate(spellPrefab);

        // Set the name of the cloned object if needed
        spellObject.name = $"{spellId}";

        // Get the Spell component from the instantiated prefab
        Spell newSpell = spellObject.GetComponent<Spell>();

        DontDestroyOnLoad(spellObject);

        newSpell.Initialize(spellId, this, info, GNM.triggerPrefab);
        //SpellManager.Instance.AddSpell(newSpell);

        newSpell.prepare();

        return newSpell;
    }
    public void DealDamage(Spell spell, Unit target)
    {
        if (spell == null || target == null)
        {
            if (spell == null)
                print("spellNull");
            else if (target == null)
                print("targetNull");

            return; 
        }

        SpellInfo m_spellInfo = spell.m_spellInfo; // Assuming spell has a spellInfo property
        if (m_spellInfo == null)
            return;

        SpellSchoolMask spellClass = m_spellInfo.SchoolMask;

        SpellType spellType = m_spellInfo.Type;

        if (spellType != SpellType.Damage)
            return;

        // Calculate the damage and check if it's a critical hit
        bool crit;
        float damage = Mathf.Round(SpellDamageBonusDone(target, spell, m_spellInfo, out crit));

        bool absorb = false;

        // target.RemoveAurasWithInterruptFlags(SD.SpellAuraInterruptFlags.Damage, spell);

        float abamt = target.GetAbsorbAmount();
        if (abamt > 0)
        {
            if (abamt >= damage)
            {
                abamt -= damage;
                damage = 0;
                absorb = true;
            }
            else
            {
                damage -= abamt;
                abamt = 0;
            }
            target.SetAbsorbAmount(abamt);
        }

        float newHealth = 0;

        if (!spell.isPositive && damage > 0)
        {
            newHealth = target.GetHealth() - damage;
            if (newHealth < 0)
                newHealth = 0;
            target.SetHealth(newHealth);
            print($"Hit {target.name} with {spell.name}!");
            print($"{newHealth} is his new health!");
        }
        else if (spell.isPositive)
        {
            newHealth = target.GetHealth() + damage;
            if (newHealth > target.GetMaxHealth())
                newHealth = target.GetMaxHealth();
            target.SetHealth(newHealth);
            print($"Hit {target.name} with {spell.name}!");
            print($"{newHealth} is his new health!");
        }
        else
            newHealth = target.GetHealth(); // Damage must be 0 so must be absorb 

        /* TODO: Handle stealth removal
        if (target.IsInStealth())
        {
            target.BreakStealth();
        }*/

        // Send health opcode
        SendHealthUpdate(target, newHealth, spell.isPositive);
        SendCombatText(target, damage, spell.isPositive, absorb);
    }

    private void SendCombatText(Unit target, float damage, bool isPositive, bool absorb)
    {
        if (target == null)
            return;

        NetworkWriter writer = new NetworkWriter();

        writer.WriteNetworkIdentity(Identity);
        writer.WriteNetworkIdentity(target.Identity);
        writer.WriteFloat(damage);
        writer.WriteBool(isPositive);
        writer.WriteBool(absorb);

        OpcodeMessage msg = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_SEND_COMBAT_TEXT,
            payload = writer.ToArray()
        };

        NetworkServer.SendToAll(msg);
    }

    public void SendHealthUpdate(Unit target, float health, bool Positive)
    {
        NetworkWriter writer = new NetworkWriter();

        // Writing data to the NetworkWriter
        writer.WriteNetworkIdentity(target.Identity);
        writer.WriteString("health");
        writer.WriteFloat(health);
        writer.WriteFloat(target.GetMaxHealth());

        OpcodeMessage msg = new OpcodeMessage
        {
            opcode = Opcodes.SMSG_UPDATE_STAT,  // Assuming this opcode is defined
            payload = writer.ToArray()
        };

        NetworkServer.SendToAll(msg);
    }

    private void CheckMovement()
    {
        // Calculate the distance moved since the last frame
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        // If the distance moved is greater than the threshold, consider the unit to be moving
        isMoving = distanceMoved > movementThreshold;

        // Update the last position for the next check
        lastPosition = transform.position;
    }

    // This function returns true if the unit is currently moving
    public bool IsMoving()
    {
        return isMoving;
    }

    public string GetClass()
    {
        return "Warlock"; // TODO: Create class system
    }

    public string GetSpecialization()
    {
        return "Affliction"; // TODO: Create Spec system
    }

    public float SpellDamageBonusDone(Unit victim, Spell spell, SpellInfo m_spellInfo, out bool crit)
    {
        crit = false;
        float doneTotal = 0f;
        // float doneTotalMod = SpellDamagePctDone(spell); // Assuming this method exists
        float doneTotalMod = 1f;                                               
        // float doneAdvertisedBenefit = SpellBaseDamageBonusDone(); // Placeholder for future implementation

        // Check if spell amount is 0
        if (spell.GetAmount() == 0)
            return 0f;

        // TODO: Check for immunities
        // if (victim.m_immunities.ContainsKey(spell.m_school) && victim.m_immunities[spell.m_school])
        //    return 0f;

        // Default damage calculation
        bool modPoints = false;
        float newPoints = 0;
        if (spell.modBasePoints > 0)
        {
            modPoints = true;
            newPoints = spell.modBasePoints;
        }
        if (modPoints)
            doneTotal = UnityEngine.Random.Range(newPoints, newPoints * 1.5f);
        else
            doneTotal = UnityEngine.Random.Range(newPoints, newPoints * 1.5f);

        // Damage modifiers based on damage class
        string damageClass = m_spellInfo.damageClass;
        if (damageClass == "Intellect")
        {
            Stats intellect = Stats.Intellect;
            doneTotal += UnityEngine.Random.Range(doneTotal + ((int)intellect / 8f), doneTotal + ((int)intellect / 7f));
        }
        else if (damageClass == "Strength")
        {
            Stats strength = Stats.Strength;
            doneTotal += UnityEngine.Random.Range(doneTotal + ((int)strength / 10f), doneTotal + ((int)strength / 9f));
        }

        /* TODO: Check for damage-increasing auras
        foreach (Aura aura in GetAuraList())
        {
            if (aura.spell.spellId == 46) // Rune of Power
            {
                doneTotal *= 1.4f;
            }
        }*/

        // Crit Handler
        float critPercentage = 15f; // TODO: Add in dynamic crit chance
        if (spell.HasFlag(SpellFlags.SPELL_FLAG_ALWAYS_CRIT) ||
            HasAura(14)) // Combustion hard check
            //(victim.IsFrozen() && GetClass() == "Mage" && GetSpecialization() == "Frost"))
        {
            doneTotal *= 2f;
            crit = true;
        }
        else
        {
            if (UnityEngine.Random.Range(0, 100) <= critPercentage)
            {
                doneTotal *= 2f;
                crit = true;
            }
        }

        /* TODO: We don't want DoTs to trigger crit procs
        bool isDot = false;
        AuraInfo auraInfo = spell.GetAuraInfo();
        if (auraInfo != null && auraInfo.Periodic)
        {
            isDot = true;
        }*/

        // Handle Crit Procs
        if (crit)
        {
            if (GetClass() == "Mage" && GetSpecialization() == "Fire")
            {
                if (spell.m_spellInfo.SchoolMask == SpellSchoolMask.Fire)
                {
                    const int SPELL_MAGE_HEATING_UP = 11;
                    const int SPELL_MAGE_HOT_STREAK = 12;

                    if (HasAura(SPELL_MAGE_HEATING_UP))
                    {
                        RemoveAura(SPELL_MAGE_HEATING_UP);
                        if (HasAura(SPELL_MAGE_HOT_STREAK))
                        {
                            RemoveAura(SPELL_MAGE_HOT_STREAK);
                        }
                        CastSpell(SPELL_MAGE_HOT_STREAK, this); // Cast Hot Streak
                    }
                    else
                    {
                        CastSpell(SPELL_MAGE_HEATING_UP, this); // Cast Heating Up!
                    }
                }
            }
        }
        else
        {
            if (/*!isDot &&*/ HasAura(21)) // Assuming 21 is SPELL_MAGE_HEATING_UP
            {
                RemoveAura(21);
            }
        }

        float tmpDamage = doneTotal * doneTotalMod;
        /* TODO: if (spell.m_customPCT != null)
        {
            return spell.m_customPCT.Value; // Assuming m_customPCT is nullable
        }*/

        return tmpDamage;
    }

    public bool HasAura(int spellId)
    {
        foreach (Aura aura in auraList)
        {
            if (aura.auraInfo.Id == spellId)
            {
                return true;
            }
        }
        return false;
    }
}
