using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public abstract class PhaseData_B : ScriptableObject
{
    public string PhaseName = "フェーズ";

    public abstract UniTask Run(PhaseContext context, CancellationToken token);
}