using Arch.Core;
using DataForge.Data;
using UnityEngine;

namespace DataForge.Processors
{
    public class STransformProcessor : ComponentProcessor<STransform>
    {
        public override void OnProcess(Entity entity, ref STransform t)
        {
            t.scale = Vector3.one;
        }
    }
}