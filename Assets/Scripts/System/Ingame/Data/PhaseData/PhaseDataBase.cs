using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PhaseDataBase", menuName = "GameData/PhaseDataBase")]
public class PhaseDataBase : ScriptableObject
{
    [SerializeField] private List<PhaseData_B> _phases;

    public List<PhaseData_B> AllPhaseData => _phases;
}
