using UnityEngine;

namespace Effects
{
    [CreateAssetMenu(fileName = "DropletMultiplier", menuName = "Spirit/Effect/Droplet Multiplier")]
    public class DropletMultiplierEffect : SpiritEffect
    {
        public override void Apply(Credits credits, SpiritBuffManager buffManager, float multiplier)
        {
            credits.dropletsMulti += multiplier;
        }

        public override void Remove(Credits credits, SpiritBuffManager buffManager, float multiplier)
        {
            credits.dropletsMulti -= multiplier;
        }
    }
}