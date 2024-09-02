using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationData
{
    #region ParameterNames
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string moveParameterName = "Move";
    [SerializeField] private string attackParameterName = "Attack";
    [SerializeField] private string dieParameterName = "Die";
    [SerializeField] private string hitParameterName = "Hit";
    [SerializeField] private string failParameterName = "Fail";
    [SerializeField] private string skillParameterName = "Skill";
    [SerializeField] private string eggBreakParameterName = "Break";
    [SerializeField] private string eggHatchEffectParameterName = "Hatch";
    [SerializeField] private string eggHatchAutoModeParameterName = "AutoMode";
    #endregion

    #region ParameterHashs
    public int IdleParameterHash { get; private set; }
    public int MoveParameterHash { get; private set; }
    public int AttackParameterHash { get; private set; }
    public int DieParameterHash { get; private set; }
    public int HitParameterHash { get; private set; }
    public int FailParameterHash { get; private set; }
    public int SkillParameterHash { get; private set; }
    public int EggBreakParameterHash { get; private set; }
    public int EggHatchParameterHash { get; private set; }
    public int EggHatchAutoModeParameterHash { get; private set; }
    #endregion

    public void Initialize()
    {
        #region 전투 Anim
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        MoveParameterHash = Animator.StringToHash(moveParameterName);
        AttackParameterHash = Animator.StringToHash(attackParameterName);
        DieParameterHash = Animator.StringToHash(dieParameterName);
        HitParameterHash = Animator.StringToHash(hitParameterName);
        FailParameterHash = Animator.StringToHash(failParameterName);
        SkillParameterHash = Animator.StringToHash(skillParameterName);
        #endregion

        #region Egg Anim
        EggBreakParameterHash = Animator.StringToHash(eggBreakParameterName);
        EggHatchParameterHash = Animator.StringToHash(eggHatchEffectParameterName);
        EggHatchAutoModeParameterHash = Animator.StringToHash(eggHatchAutoModeParameterName);
        #endregion
    }
}
