using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Variable
{
    public int level;
    public bool isTrigger;
    public bool isEqualEffect;
    public int animBoolCode;
    public Animator animator;

    public Variable(int level, bool isTrigger, int animBoolCode, bool isEqualEffect = true)
    {
        this.level = level;
        this.isTrigger = isTrigger;
        this.animBoolCode = animBoolCode;
        this.isEqualEffect = isEqualEffect;
    }

    public void Enter()
    {
        if(isTrigger){
            animator.SetTrigger(animBoolCode);
        } else {
            animator.SetBool(animBoolCode, true);
        }
    }

    public void Exit()
    {
        if(!isTrigger)
            animator.SetBool(animBoolCode, false);
    }
}

public class VariableManager
{
    public List<Variable> variables;
    public Animator animator;
    public VariableManager(Animator animator)
    {
        variables = new List<Variable>();
        this.animator = animator;
    }

    public void AddVariable(Variable var)
    {
        var.animator = animator;
        variables.Add(var);
    }

    public void ActivateVar(Variable var)
    {
        if(!variables.Contains(var)){
            Debug.Log("this var not in manager");
            return;
        } 
        
        foreach (Variable variable in variables){
            if(variable.level <= var.level){
                if(variable.level == var.level && !var.isEqualEffect)
                    continue;
                variable.Exit();
            }
        }

        var.Enter();
    }

    public void DeactivateVar(Variable var)
    {
        var.Exit();
    }
}