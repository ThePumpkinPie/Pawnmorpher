﻿// PMStatDefOf.cs modified by Iron Wolf for Pawnmorph on 12/01/2019 9:01 AM
// last updated 12/01/2019  9:01 AM

using JetBrains.Annotations;
using RimWorld;

namespace Pawnmorph
{
    /// <summary>
    /// static def of class for commonly used stats 
    /// </summary>
    [DefOf]
    public static class PMStatDefOf
    {
        /// <summary>
        /// stat that influences how fast a pawn adapts to new mutations 
        /// </summary>
        /// has a range of [-1,2]
        /// values less then 0 means the pawn gets worse with mutations over time
        /// values greater then 0 mean the pawn gets better with mutations over time
        /// default value is 1 
        [UsedImplicitly(ImplicitUseKindFlags.Assign), NotNull]
        public static StatDef MutationAdaptability;

        static PMStatDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(StatDef));
        }
    }
}