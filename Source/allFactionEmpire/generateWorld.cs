using RimWorld.Planet;

namespace empireMaker;

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