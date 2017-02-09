﻿namespace Paramita.GameLogic.Items
{
    /*
     * Armors are items used to protect beings from harm, usually from combat damage.
     * Includes protection from magic or the elements as well.
     * 
     * This is an abstract class not intended to be instantiated.
     */
    public abstract class Armor : Item
    {

        private int encumbrance;

        public int Encumbrance { get { return encumbrance; } }

        public Armor(ItemType type) : base(type)
        {
        }
    }
}
