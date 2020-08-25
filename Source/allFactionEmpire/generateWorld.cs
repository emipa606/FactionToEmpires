using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace empireMaker
{

    public class generateWorld : WorldComponent
    {

        public generateWorld(World world) : base(world)
        {
        }

        public override void FinalizeInit()
        {
            EmpireMaker.PatchRelation();
        }

    }

}
