using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects
{
    // The game rules, in order: this list is what defines a match and what each one fires.
    [CreateAssetMenu(fileName = "SpecialMatchConfig", menuName = "Gameplay/SpecialMatchConfig")]
    public class SpecialMatchConfig : ScriptableObject
    {
        [Tooltip("From strongest rule to weakest. With no rules, nothing counts as a match.")]
        [SerializeField] private MatchRule[] _rules;

        [Tooltip("On: every rule a match meets fires, and the list order changes nothing. " +
                 "Off: the first rule to claim a tile wins and the order above decides everything " +
                 "- a rule that matches often sitting near the top starves the ones below it.")]
        [SerializeField] private bool _stackEffects = true;

        public MatchRule[] Rules => _rules;
        public bool StackEffects => _stackEffects;
    }
}
