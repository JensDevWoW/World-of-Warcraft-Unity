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
using System.Linq;
using Unity.Services.Authentication;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientNetworkManager : MonoBehaviour
{
    private ClientOpcodeHandler opcodeHandler;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        opcodeHandler = new ClientOpcodeHandler();

        // Register opcode handlers
        opcodeHandler.RegisterHandler(Opcodes.SMSG_SPELL_START,         HandleSpellStart);
        opcodeHandler.RegisterHandler(Opcodes.SMSG_SPELL_GO,            HandleSpellGo);
        opcodeHandler.RegisterHandler(Opcodes.SMSG_SEND_COMBAT_TEXT,    HandleCombatText);
        opcodeHandler.RegisterHandler(Opcodes.SMSG_UPDATE_STAT,         HandleUpdateStat);
        opcodeHandler.RegisterHandler(Opcodes.SMSG_AURA_UPDATE,         HandleAuraUpdate);
        opcodeHandler.RegisterHandler(Opcodes.SMSG_CAST_CANCELED,       HandleCancelCast);
        opcodeHandler.RegisterHandler(Opcodes.SMSG_APPLY_AURA,          HandleApplyAura);
        opcodeHandler.RegisterHandler(Opcodes.SMSG_UPDATE_TARGET,       HandleUpdateTarget);
        opcodeHandler.RegisterHandler(Opcodes.SMSG_SPELL_FAILED,        HandleSpellFailed);
        opcodeHandler.RegisterHandler(Opcodes.SMSG_UPDATE_UNIT_STATE,   HandleUpdateUnitState);
        opcodeHandler.RegisterHandler(Opcodes.SMSG_INIT_BARS,           HandleInitBars);
        opcodeHandler.RegisterHandler(Opcodes.SMSG_UPDATE_CHARGES,      HandleUpdateCharges);
        opcodeHandler.RegisterHandler(Opcodes.SMSG_SPELL_COOLDOWN,      HandleSpellCooldown);
        opcodeHandler.RegisterHandler(Opcodes.SMSG_CHANNELED_START,     HandleStartChannel);
        opcodeHandler.RegisterHandler(Opcodes.SMSG_ACCOUNT_INFO,        HandleAccountInfo);

        //opcodeHandler.RegisterHandler(Opcodes.SMSG_CHANNELED_UPDATE,    HandleUpdateChannel);
        // Register the OpcodeMessage handler on the client
        NetworkClient.RegisterHandler<OpcodeMessage>(OnOpcodeMessageReceived);
    }

    private void OnOpcodeMessageReceived(OpcodeMessage msg)
    {
        // Handle the opcode using the registered handler
        opcodeHandler.HandleOpcode(msg.opcode, new NetworkReader(msg.payload));
    }

    private void HandleAccountInfo(NetworkReader reader)
    {
        Debug.Log("Account data received!");
        int accountId = reader.ReadInt();
        string accountName = reader.ReadString();
        Account currentAccount = new Account { accountName = accountName, Id = accountId };
        // Store account info in a client-side manager
        ClientAccountManager.Instance.SetCurrentAccount(currentAccount);

        SceneManager.LoadScene("CharacterSelectionScene");
    }


    private void HandleSpellCooldown(NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        int spellId = reader.ReadInt();
        float duration = reader.ReadFloat();

        if (identity.netId == NetworkClient.localPlayer.netId)
        {
            UIHandler.Instance.StartCooldown(spellId, duration);
        }
    }

    private void HandleUpdateCharges(NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        int spellId = reader.ReadInt();
        int charges = reader.ReadInt();

        if (identity.netId == NetworkClient.localPlayer.netId)
        {
            UIHandler.Instance.UpdateCharges(spellId, charges);
        }
    }

    private void HandleInitBars(NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();

        if (identity.netId == NetworkClient.localPlayer.netId)
        {
            UIHandler.Instance.InitBars();
        }
    }

    private void HandleSpellStart(NetworkReader reader)
    {
        int castFlags = reader.ReadInt();
        NetworkIdentity casterIdentity = reader.ReadNetworkIdentity();
        NetworkIdentity targetIdentity = reader.ReadNetworkIdentity();
        float castTime = reader.ReadFloat();
        float gcd = reader.ReadFloat();
        int spellId = reader.ReadInt();
        bool animationEnabled = reader.ReadBool();
        bool isSpellQueueSpell = reader.ReadBool();
        Vector3 aoePosition = reader.ReadVector3();
        bool voc = reader.ReadBool();

        Unit caster = casterIdentity.GetComponent<Unit>();

        if (casterIdentity.netId == NetworkClient.localPlayer.netId)
        {
            if (castTime > 0f)
                UIHandler.Instance.StartCast(castTime, GetSpellNameById(spellId));

            if (gcd > 0f)
                UIHandler.Instance.StartGlobalCooldown(gcd);
        }

        if (caster.ToPlayer() != null)
            if (animationEnabled)
                caster.animHandler.animator.SetBool("IsCastingDirected", true);

    }

    private string GetSpellNameById(int spellId)
    {
        SpellInfo spellInfo = SpellDataHandler.Instance.Spells.FirstOrDefault(spell => spell.Id == spellId);
        return spellInfo != null ? spellInfo.Name : "Unknown Spell";
    }

    private void HandleSpellGo(NetworkReader reader)
    {
        // Deserialize each field in the same order as they were serialized
        int castFlags = reader.ReadInt();
        NetworkIdentity casterIdentity = reader.ReadNetworkIdentity();
        NetworkIdentity targetIdentity = reader.ReadNetworkIdentity();
        float castTime = reader.ReadFloat();
        float cooldownTime = reader.ReadFloat();
        int spellId = reader.ReadInt();
        float speed = reader.ReadFloat();
        float spellTime = reader.ReadFloat();
        bool animationEnabled = reader.ReadBool();
        Vector3 aoePosition = reader.ReadVector3();
        int manaCost = reader.ReadInt();
        bool voc = reader.ReadBool();
        bool isToggled = reader.ReadBool();

        Unit caster = casterIdentity.GetComponent<Unit>();
        Unit target = null;

        if (targetIdentity != null)
            target = targetIdentity.GetComponent<Unit>();



        if (caster != null && animationEnabled == true && caster.ToPlayer() != null)
        {
                
            caster.animHandler.SetupSpellParameters(spellId, speed, caster.transform, target?.transform);
            caster.animHandler.animator.SetBool("IsCastingDirected", false);
            caster.animHandler.animator.SetTrigger("CastFinished");
        }

        if (casterIdentity.netId == NetworkClient.localPlayer.netId)
        {
            // We need to tell the client that the cooldown is now set
            if (cooldownTime > 0 && UIHandler.Instance.IsOnCooldown(spellId) == false)
                UIHandler.Instance.StartCooldown(spellId, cooldownTime);
        }

        if (spellTime > 0)
        {
            Transform casterTransform = caster.transform;

            if (target == null)
            {
                //VFXManager.Instance.CastSpell(spellId, speed, casterTransform);
                return;
            }

            Transform targetTransform = null;
            if (target != null)
                targetTransform = target.transform;

            if (!targetTransform)
                Debug.LogError("No transform found for Target!");

            
        }
    }

    private void HandleUpdateStat(NetworkReader reader)
    {
        NetworkIdentity networkIdentity = reader.ReadNetworkIdentity();
        string statChanged = reader.ReadString();
        float statValue = reader.ReadFloat();
        float maxStatValue = reader.ReadFloat();

        Unit sender = networkIdentity.GetComponent<Unit>();

        GameObject canvas = GameObject.FindWithTag("Canvas");

        if (!canvas)
            Debug.LogError("CANNOT FIND CANVAS!");

        if (networkIdentity.netId == NetworkClient.localPlayer.netId)
        {
            if (statChanged == "health")
            {
                UIHandler.Instance.UpdateHealth(statValue, maxStatValue);
            }
        }

        if (statChanged == "health")
        {
            // Check if we are targetting that unit
            if (UIHandler.Instance.GetTarget() == sender)
            {
                // We are targetting that person, adjust his health
                UIHandler.Instance.TargetHealthUpdate(statValue, maxStatValue);
            }
        }

    }

    private void HandleStartChannel(NetworkReader reader)
    {
        NetworkIdentity networkIdentity = reader.ReadNetworkIdentity();
        int spellId = reader.ReadInt();
        float duration = reader.ReadFloat();


        Unit unit = networkIdentity.GetComponent<Unit>();

        if (unit != null)
        {
            // TODO: Handle animation
        }

        if (networkIdentity.netId == NetworkClient.localPlayer.netId)
        {
            UIHandler.Instance.StartChannel(spellId, duration);
        }

        

    }

    private void HandleApplyAura(NetworkReader reader)
    {
        NetworkIdentity casterIdentity = reader.ReadNetworkIdentity();
        NetworkIdentity targetIdentity = reader.ReadNetworkIdentity();
        int auraId = reader.ReadInt();
        float duration = reader.ReadFloat();

        Unit caster = casterIdentity.GetComponent<Unit>();
        Unit target = targetIdentity.GetComponent<Unit>();

        if (casterIdentity.netId == NetworkClient.localPlayer.netId)
        {
            // We need to check if we're targetting the person receiving the debuff still
            if (caster.GetTarget() == target)
            {
                UIHandler.Instance.AddTargetDebuff(auraId, IconManager.GetSpellIcon(target.GetClass(), auraId), duration);
            }
        } // No else here because we might be targetting ourselves and receive a buff/debuff
        if (targetIdentity.netId == NetworkClient.localPlayer.netId)
        {
            // We are the target, apply the aura to our list
            UIHandler.Instance.AddBuff(auraId, IconManager.GetSpellIcon(target.GetClass(), auraId), duration); // TODO: Make ClassHandler
        }
    }

    private void HandleAuraUpdate(NetworkReader reader)
    {
        NetworkIdentity casterIdentity = reader.ReadNetworkIdentity();
        NetworkIdentity targetIdentity = reader.ReadNetworkIdentity();
        int auraId = reader.ReadInt();
        float duration = reader.ReadFloat();
        int stacks = reader.ReadInt();

        Unit caster = casterIdentity.GetComponent<Unit>();
        Unit target = targetIdentity.GetComponent<Unit>();

        if (casterIdentity.netId == NetworkClient.localPlayer.netId)
        {
            // We are caster
        }
        if (targetIdentity.netId == NetworkClient.localPlayer.netId)
        {
            UIHandler.Instance.UpdateAura(auraId, duration, stacks);
        }

        // Need to check if we're targetting the unit
        if (UIHandler.Instance.GetTarget() == target)
        {
            UIHandler.Instance.UpdateTargetAura(auraId, duration, stacks);
        }
    }

    private void HandleCombatText(NetworkReader reader)
    {
        NetworkIdentity casterIdentity = reader.ReadNetworkIdentity();
        NetworkIdentity targetIdentity = reader.ReadNetworkIdentity();
        float damage = reader.ReadFloat();
        bool isPositive = reader.ReadBool();
        bool absorb = reader.ReadBool();

        Unit target = targetIdentity.GetComponent<Unit>();

        if (casterIdentity.netId == NetworkClient.localPlayer.netId)
        {
            Transform transform = targetIdentity.transform;
            if (transform != null)
            {
                if (absorb)
                    DynamicTextManager.CreateText(transform.position, "absorb", DynamicTextManager.defaultData);
                else
                    DynamicTextManager.CreateText(transform.position, damage.ToString(), DynamicTextManager.defaultData);
            }
        }
    }

    private void HandleSpellFailed(NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        int spellId = reader.ReadInt();
        string reason = reader.ReadString();

        if (identity.netId == NetworkClient.localPlayer.netId)
        {
            UIHandler.Instance.HandleSpellFailed(spellId, reason);
        }
    }

    private void HandleUpdateTarget(NetworkReader reader)
    {
        NetworkIdentity sender = reader.ReadNetworkIdentity();
        NetworkIdentity targetIdentity = reader.ReadNetworkIdentity();
        float health = reader.ReadFloat();
        float maxHealth = reader.ReadFloat();
        float mana = reader.ReadFloat();
        float maxMana = reader.ReadFloat();

        if (sender.netId == NetworkClient.localPlayer.netId)
        {
            Unit target = targetIdentity.GetComponent<Unit>();
            // We are the person whose target needs updating
            UIHandler.Instance.UpdateTarget(target);
            UIHandler.Instance.TargetHealthUpdate(health, maxHealth);
            //TODO: Add mana
        }
    }
    private void HandleCancelCast(NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        int spellId = reader.ReadInt();
        Unit caster = identity.GetComponent<Unit>();

        if (identity.netId == NetworkClient.localPlayer.netId)
        {
            // This is working
        }
    }

    private void HandleUpdateUnitState(NetworkReader reader)
    {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        int state = reader.ReadInt();
        bool apply = reader.ReadBool();

        Unit unit = identity.GetComponent<Unit>();

        if (identity.netId == NetworkClient.localPlayer.netId)
        {
            PlayerControls playerControls = identity.GetComponent<PlayerControls>();
            if (!playerControls)
                return;

            if (apply)
                playerControls.AddState(state);
            else
                playerControls.RemoveState(state);
        }
    }
}
