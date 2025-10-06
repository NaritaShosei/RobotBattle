using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class PhaseData_B : ScriptableObject
{
    public string PhaseName = "フェーズ";

    public abstract UniTask Run(PhaseContext context);
}

// フェーズ間で共有したいデータなど
public class PhaseContext
{

}