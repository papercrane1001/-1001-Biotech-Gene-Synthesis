using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using UnityEngine;
using Verse;

namespace BiotechGeneSynthesis
{
    public class Dialog_SynthesizeGenepack : Window
    {
        public ThingFilterUI.UIState uiState = new ThingFilterUI.UIState();
        private Vector2 scrollPosition;
        public const float TopAreaHeight = 40f;
        public const float TopButtonHeight = 35f;
        public const float TopButtonWidth = 150f;

        private static ThingFilter genepackGlobalFilter;

        public override Vector2 InitialSize => new Vector2(700f, 700f);

        public override void DoWindowContents(Rect inRect)
        {
            forcePause = true;
            doCloseX = true;
            doCloseButton = true;
            closeOnClickedOutside = false;
            absorbInputAroundWindow = true;

            //throw new NotImplementedException();
            //Rect rect = new Rect()
            if(genepackGlobalFilter == null)
            {
                genepackGlobalFilter = new ThingFilter();
                genepackGlobalFilter.SetAllow(ThingDefOf.Genepack, allow: true);
            }




        }
    }
}
