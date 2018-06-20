using System;

namespace G2_2D_LZ.Helpers
{
    public class NextRoot
    {
        public static int GetNextRootX(int x, int colOffset, Constants.GeometricTransformation geometricTransformation)
        {
            if (geometricTransformation == Constants.GeometricTransformation.VerticalMirror
                || geometricTransformation == Constants.GeometricTransformation.FirstDiagonalMirror)
            {
                return x - colOffset;
            }
            if (geometricTransformation == Constants.GeometricTransformation.Identity
                || geometricTransformation == Constants.GeometricTransformation.HorizontalMirror
                || geometricTransformation == Constants.GeometricTransformation.NoGeometricTransformation)
            {
                return x + colOffset;
            }

            throw new InvalidOperationException("geometric tranformation not set" + nameof(GetNextRootX));
        }

        public static int GetNextRootY(int y, int rowOffset, Constants.GeometricTransformation geometricTransformation)
        {
            if (geometricTransformation == Constants.GeometricTransformation.HorizontalMirror
                || geometricTransformation == Constants.GeometricTransformation.FirstDiagonalMirror)
            {
                return y - rowOffset;
            }

            if (geometricTransformation == Constants.GeometricTransformation.Identity
                || geometricTransformation == Constants.GeometricTransformation.VerticalMirror
                || geometricTransformation == Constants.GeometricTransformation.NoGeometricTransformation
            )
            {
                return y + rowOffset;
            }

            throw new InvalidOperationException("geometric tranformation not set" + nameof(GetNextRootY));
        }
    }
}
