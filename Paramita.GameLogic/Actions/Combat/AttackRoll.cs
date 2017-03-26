﻿using Paramita.GameLogic.Items;
using Paramita.GameLogic.Actors;
using Paramita.GameLogic.Mechanics;

namespace Paramita.GameLogic.Actions
{
    /*
    *  An attack roll consists of a competitive roll of 2d6 (open-ended) between the attacker
    *  and defender. 
    *  
    *  The attacker's attack skill, weapon modifiers, and fatigue penalties are added to its roll.
    *  The defender's defense skill, weapon modifiers, and fatigue penalities are added to its roll.
    *  
    *  A message is displayed on the GameScene's Status Message panel indicated the totals.
    *  
    *  The difference between the two scores is stored for use in further resolution. A positive
    *  result indicates the defender was hit.
    */
    public class AttackRoll
    {
        Dice dice2d6;

        Actor attacker;
        Actor defender;
        Weapon attackWeapon;

        int attackSkill;
        int attackerFatigue;
        int defenseSkill;
        int defenderFatigue;
        int defenderMultAttackedPenalty;

        int attackScore;
        int defendScore;

        public Actor Defender
        {
            get { return defender; }
        }
        public Actor Attacker
        {
            get { return attacker; }
        }
        public Weapon AttackWeaon
        {
            get { return attackWeapon; }
        }
        public int Result
        {
            get { return attackScore - defendScore; }
        }
        public string AttackerReport
        {
            get { return GetAttackerRollReport(); }
        }
        public string DefenderReport
        {
            get { return GetDefenderRollReport(); }
        }
        


        public AttackRoll(Actor attacker, Weapon weapon, Actor defender)
        {
            this.attacker = attacker;
            this.defender = defender;
            attackWeapon = weapon;
            dice2d6 = new Dice(2);

            InitializeAttackRollVariables();

            attackScore = CalcAttackScore();
            defendScore = CalcDefendScore();
        }


        private void InitializeAttackRollVariables()
        {
            attackSkill = attacker.AttackSkill + attackWeapon.AttackModifier;
            attackerFatigue = attacker.FatigueAttPenalty;
            defenseSkill = defender.DefenseSkill;
            defenderFatigue = defender.FatigueDefPenalty;
            defenderMultAttackedPenalty = (defender.TimesAttacked - 1) * 2;
        }

        private int CalcAttackScore()
        {
            return dice2d6.OpenEndedRoll() + attackSkill - attackerFatigue;
        }

        private int CalcDefendScore()
        {
            return dice2d6.OpenEndedRoll() + defenseSkill - defenderFatigue 
                - defenderMultAttackedPenalty;
        }

        private string GetAttackerRollReport()
        {
            return attacker + " rolled " + attackScore + "(attSkill: " + attackSkill
                + ", fatigue: " + attackerFatigue + ")";
        }

        private string GetDefenderRollReport()
        {
            return defender + " rolled " + defendScore + "(defSkill: " + defenseSkill
                + ", fatigue: " + defenderFatigue + ", multAtt: " + defenderMultAttackedPenalty + ")";
        }
    }
}