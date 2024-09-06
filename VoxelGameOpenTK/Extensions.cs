using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGameOpenTK
{
    public static class Extensions
    {
        public static Vector3 Round(this Vector3 vector)
        {
            return new(MathF.Round(vector.X), MathF.Round(vector.Y), MathF.Round(vector.Z));
        }
    }
}
