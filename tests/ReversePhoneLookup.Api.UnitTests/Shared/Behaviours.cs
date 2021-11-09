using AutoFixture;

namespace ReversePhoneLookup.Models.UnitTests.Shared
{
    public static class Behaviours
    {
        public static void GenerationDepthBehaviorDepth2(IFixture fixture)
        {
            fixture.Behaviors.Add(new GenerationDepthBehavior(2));
        }
        public static void GenerationDepthBehaviorDepth3(IFixture fixture)
        {
            fixture.Behaviors.Add(new GenerationDepthBehavior(3));
        }
        public static void GenerationDepthBehaviorDepth4(IFixture fixture)
        {
            
        }
    }
}
