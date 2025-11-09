using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PhaseDataSequence", menuName = "GameData/PhaseDataSequence")]
public class PhaseDataSequence : ScriptableObject
{
    [SerializeField] private List<PhaseData_B> _phases;

    public List<PhaseData_B> AllPhaseData => _phases;
}
