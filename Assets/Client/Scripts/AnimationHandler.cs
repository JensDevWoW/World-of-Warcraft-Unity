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

using UnityEngine;
using Mirror;

public class AnimationHandler : NetworkBehaviour
{
    public Animator animator;
    private NetworkAnimator networkAnimator;
    private GameObject bef;

    public int SpellId { get; private set; }
    public float SpellSpeed { get; private set; }
    public Transform CasterTransform { get; private set; }
    public Transform TargetTransform { get; private set; }

    public void SetupSpellParameters(int spellId, float speed, Transform caster, Transform target)
    {
        SpellId = spellId;
        SpellSpeed = speed;
        CasterTransform = caster;
        TargetTransform = target;
    }

    private void Awake()
    {
        networkAnimator = GetComponent<NetworkAnimator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on the GameObject.");
        }

        if (networkAnimator == null)
        {
            Debug.LogError("NetworkAnimator component not found on the GameObject.");
        }
    }

    // Function to set casting state
    [ClientRpc]
    public void RpcSetCastingDirected(bool isCastingDirected)
    {
        // Set the parameter on the Animator; this will be replicated to all clients
        animator.SetBool("IsCastingDirected", isCastingDirected);
    }

    // Function to update the casting state that can be called on the client
    public void SetCastingDirected(bool isCastingDirected)
    {
        // Set the parameter locally
        animator.SetBool("IsCastingDirected", isCastingDirected);

        // Call the RPC to update on all clients
        RpcSetCastingDirected(isCastingDirected);
    }
}
