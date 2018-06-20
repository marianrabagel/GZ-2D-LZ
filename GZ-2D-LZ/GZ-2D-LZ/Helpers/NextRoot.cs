using System;

namespace G2_2D_LZ.Helpers
{
    public class NextRoot
    {
        public static int GetNextRootX(int x, int colOffset, int geometricTransformation)
        {
            if (geometricTransformation == (int)Constants.GeometricTransformation.VerticalMirror
                || geometricTransformation == (int)Constants.GeometricTransformation.FirstDiagonalMirror)
            {
                return x - colOffset;
            }
            if (geometricTransformation == (int)Constants.GeometricTransformation.Identity
                || geometricTransformation == (int)Constants.GeometricTransformation.HorizontalMirror
                || geometricTransformation == (int)Constants.GeometricTransformation.NoGeometricTransformation)
            {
                return x + colOffset;
            }

            throw new InvalidOperationException("geometric tranformation not set" + nameof(GetNextRootX));
        }

        public static int GetNextRootY(int y, int rowOffset, int geometricTransformation)
        {
            if (geometricTransformation == (int)Constants.GeometricTransformation.HorizontalMirror
                || geometricTransformation == (int)Constants.GeometricTransformation.FirstDiagonalMirror)
            {
                return y - rowOffset;
            }

            if (geometricTransformation == (int)Constants.GeometricTransformation.Identity
                || geometricTransformation == (int)Constants.GeometricTransformation.VerticalMirror
                || geometricTransformation == (int)Constants.GeometricTransformation.NoGeometricTransformation
            )
            {
                return y + rowOffset;
            }

            throw new InvalidOperationException("geometric tranformation not set" + nameof(GetNextRootY));
        }
    }
}
