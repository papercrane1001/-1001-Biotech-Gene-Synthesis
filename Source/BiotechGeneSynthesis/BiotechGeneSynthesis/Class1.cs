using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;







namespace BiotechGeneSynthesis
{
    public class Building_Genomorpher : Building
    {
        public override IEnumerable<Gizmo> GetGizmos()
        {
            yield return new Command_Action
            {
                defaultLabel = "SynthesizeGenepackLabel".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/SynthesizeGenepackIcon"),
                defaultDesc = "SynthesizeGenepackDesc".Translate(),
                action = Synthesize_Genepack_UI_Open
            };
        }

        protected void Synthesize_Genepack_UI_Open()
        {

        }

    }
}