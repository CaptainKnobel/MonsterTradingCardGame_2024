using MonsterTradingCardGame_2024.Enums;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Services.Business_Logic
{
    public class BattleService
    {
        public int DetermineWinner(Card card1, Card card2)
        {
            if (card1 is MonsterCard monster1 && card2 is MonsterCard monster2)
            {
                return CompareMonsters(monster1, monster2);
            }
            else if (card1 is SpellCard spell1 && card2 is SpellCard spell2)
            {
                return CompareSpells(spell1, spell2);
            }
            else if (card1 is MonsterCard monsterCard && card2 is SpellCard spellCard)
            {
                return CompareMonsterWithSpell(monsterCard, spellCard);
            }
            else if (card1 is SpellCard spellCard1 && card2 is MonsterCard monsterCard1)
            {
                return -CompareMonsterWithSpell(monsterCard1, spellCard1);
            }


            // Default: Compare damage values
            return card1.Damage.CompareTo(card2.Damage);
        }

        private int CompareMonsters(MonsterCard monster1, MonsterCard monster2)
        {
            // Spielregel: Drachen schlagen Goblins automatisch
            if (monster1.MonsterSpecies == Species.Dragon && monster2.MonsterSpecies == Species.Goblin)
                return 1;
            if (monster2.MonsterSpecies == Species.Dragon && monster1.MonsterSpecies == Species.Goblin)
                return -1;

            // Default: Schaden vergleichen
            return monster1.Damage.CompareTo(monster2.Damage);
        }

        private int CompareSpells(SpellCard spell1, SpellCard spell2)
        {
            // Spielregel: Wasser schlägt Feuer
            if (spell1.ElementType == Element.Water && spell2.ElementType == Element.Fire)
                return 1;
            if (spell2.ElementType == Element.Water && spell1.ElementType == Element.Fire)
                return -1;

            // Default: Schaden vergleichen
            return spell1.Damage.CompareTo(spell2.Damage);
        }

        private int CompareMonsterWithSpell(MonsterCard monster, SpellCard spell)
        {
            // Spielregel: Zauber dominieren basierend auf Element-Typ
            if (spell.ElementType == Element.Fire && monster.ElementType == Element.Earth)
                return 1;
            if (spell.ElementType == Element.Water && monster.ElementType == Element.Fire)
                return 1;

            // Monster dominieren basierend auf Element-Typ
            if (monster.ElementType == Element.Earth && spell.ElementType == Element.Fire)
                return -1;
            if (monster.ElementType == Element.Fire && spell.ElementType == Element.Water)
                return -1;

            // Default: Schaden vergleichen
            return monster.Damage.CompareTo(spell.Damage);
        }
    }
}
