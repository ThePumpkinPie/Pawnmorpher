﻿// HediffGiver_Mutation.cs modified by Iron Wolf for Pawnmorph on 08/07/2019 10:19 AM
// last updated 08/07/2019  10:19 AM

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Pawnmorph.Utilities;
using RimWorld;
using Verse;

namespace Pawnmorph
{
    public class HediffGiver_Mutation : HediffGiver
    {
        public float mtbDays;
        public Gender gender;
        public int chance = 100;
        public TaleDef tale;
        public ThoughtDef memory;
        public bool ignoreThoughtLimit;
        private readonly Dictionary<Hediff, bool> triggered = new Dictionary<Hediff, bool>();

        /// <summary>
        ///     Clears the triggeredHediff from this giver so it can trigger again on the same hediff.
        /// </summary>
        /// <param name="triggeredHediff">The triggered hediff.</param>
        public void ClearHediff(Hediff triggeredHediff)
        {
            triggered.Remove(triggeredHediff);
        }

        public override void OnIntervalPassed(Pawn pawn, [NotNull] Hediff cause)
        {
            RandUtilities.PushState();

            if (Rand.MTBEventOccurs(mtbDays, 60000f, 30f) && pawn.RaceProps.intelligence == Intelligence.Humanlike)
            {
                MutagenDef mutagen = (cause as Hediff_Morph)?.GetMutagenDef() ?? MutagenDefOf.defaultMutagen;


                if ((gender == pawn.gender || !triggered.TryGetValue(cause) && Rand.RangeInclusive(0, 100) <= chance)
                 && TryApply(pawn, mutagen, null, cause))
                {
                    IntermittentMagicSprayer.ThrowMagicPuffDown(pawn.Position.ToVector3(), pawn.MapHeld);
                    triggered[cause] = true;

                    if (tale != null) TaleRecorder.RecordTale(tale, pawn);

                    if (memory != null) TryAddMemory(pawn);
                }
                else
                {
                    triggered[cause] = true;
                }

                var comp = cause.TryGetComp<HediffComp_Single>();


                if (comp != null)
                {
                    comp.stacks--;
                    if (comp.stacks <= 0)
                        pawn.health.RemoveHediff(cause);
                    else if (triggered.ContainsKey(cause))
                        triggered.Remove(cause); //make sure the next roll can potentially trigger the mutation again if it has the single component 
                }
            }

            RandUtilities.PopState();
        }


        /// <summary>
        ///     Tries the apply the mutation to the given pawn
        /// </summary>
        /// <param name="pawn">The pawn.</param>
        /// <param name="mutagenDef">The mutagen definition. used to determine if it's a valid target or not</param>
        /// <param name="outAddedHediffs">The out added hediffs.</param>
        /// <returns></returns>
        public bool TryApply(Pawn pawn, MutagenDef mutagenDef, List<Hediff> outAddedHediffs = null, Hediff cause = null)
        {
            if (!mutagenDef.CanInfect(pawn)) return false;

            bool added = PawnmorphHediffGiverUtility.TryApply(pawn, hediff, partsToAffect, canAffectAnyLivePart, countToAffect,
                                                              outAddedHediffs);

            return added;
        }

        private void TryAddMemory(Pawn pawn)
        {
            ThoughtHandler thoughts = pawn.needs?.mood?.thoughts;
            if (thoughts == null) return;

            if (!ThoughtUtility.CanGetThought(pawn, memory)) return;

            var counter = 0;
            int max = PMUtilities.GetSettings().maxMutationThoughts;

            if (!ignoreThoughtLimit)
                foreach (Thought_Memory thought in thoughts.memories.Memories)
                {
                    if (MutationUtilities.AllMutationThoughts.Contains(thought.def)) counter++;

                    counter++;
                    if (counter >= max) return; //make sure to only add so many thoughts at once 
                }

            pawn.TryGainMemory(memory);
        }
    }
}