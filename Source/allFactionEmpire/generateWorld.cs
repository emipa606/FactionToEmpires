﻿using RimWorld.Planet;

namespace empireMaker;

public class generateWorld(World world) : WorldComponent(world)
{
    public override void FinalizeInit()
    {
        EmpireMaker.PatchRelation();
    }
}