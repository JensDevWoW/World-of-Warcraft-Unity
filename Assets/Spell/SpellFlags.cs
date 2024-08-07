using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellFlags
{
    public static class SpellConstants
    {
        public const int SPELL_STATE_PREPARING = 1;
        public const int SPELL_STATE_CASTING = 2;
        public const int SPELL_STATE_FINISHED = 3;
    }

    public enum SpellState
    {
        None,
        Preparing,
        Casting,
        Finished
    }
}