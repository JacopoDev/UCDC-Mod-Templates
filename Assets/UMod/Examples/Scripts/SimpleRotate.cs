using System;
using UnityEngine;

namespace UMod.Example
{
    public class SimpleRotate : MonoBehaviour
    {
        // Private
        [Flags]
        private enum RotateAxisFlags
        {
            X = 1, 
            Y = 2, 
            Z = 4,
        }

        public enum RotateAxis
        {
            X,
            XY,
            XYZ,
            Y,
            YZ,
            Z,
        }

        // Public
        public float rotateSpeed = 20;
        public RotateAxis rotateAxis = RotateAxis.Y;

        // Methods
        public void Update()
        {
            float rotatePerFrame = rotateSpeed * Time.deltaTime;

            RotateAxisFlags rotateFlags = GetAxisFlags();

            float x = ((rotateFlags & RotateAxisFlags.X) != 0) ? rotatePerFrame : 0;
            float y = ((rotateFlags & RotateAxisFlags.Y) != 0) ? rotatePerFrame : 0;
            float z = ((rotateFlags & RotateAxisFlags.Z) != 0) ? rotatePerFrame : 0;

            Quaternion extraRotation = Quaternion.Euler(x, y, z);

            transform.Rotate(x, y, z, Space.World);
        }

        private RotateAxisFlags GetAxisFlags()
        {
            switch (rotateAxis)
            {
                case RotateAxis.X: return RotateAxisFlags.X;
                case RotateAxis.XY: return RotateAxisFlags.X | RotateAxisFlags.Y;
                case RotateAxis.XYZ: return RotateAxisFlags.X | RotateAxisFlags.Y | RotateAxisFlags.Z;
                case RotateAxis.Y: return RotateAxisFlags.Y;
                case RotateAxis.YZ: return RotateAxisFlags.Y | RotateAxisFlags.Z;
                case RotateAxis.Z: return RotateAxisFlags.Z;
            }

            return 0;
        }
    }
}
