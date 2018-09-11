using Microsoft.Xna.Framework;
using System;

namespace EquationRayImager
{
    internal struct Camera
    {
        public Vector3 Up => ApplyQuaternion(Vector3.Up, Rotation);

        public Vector3 Left => ApplyQuaternion(Vector3.Left, Rotation);

        public Vector3 Forward => ApplyQuaternion(Vector3.Forward, Rotation);

        public Quaternion Rotation;
        public Vector3 Position;
        public float Width;
        public float Height;
        public float Near;

        public Camera(Vector3 pos, Vector3 lookat)
        {
            Vector3 dir = lookat - pos;
            dir.Normalize();
            Vector3 cross = Vector3.Cross(Vector3.Forward, dir);
            cross.Normalize();

            Rotation = Quaternion.CreateFromAxisAngle(cross, Angle(Vector3.Forward, dir));
            Position = pos;

            Width = 1.6f;
            Height = 0.9f;
            Near = 1.0f;
        }

        public Vector3 ScreenToWorld(Vector2 screen, Vector2 pos)
        {
            float x = ((pos.X - screen.X / 2) / (screen.X / 2)) / 2;
            float y = ((pos.Y - screen.Y / 2) / (screen.Y / 2)) / 2;

            Vector3 worldpos = Position + Forward * Near + Left * x + Up * (-y);

            return worldpos;
        }

        public Vector3 ScreenToWorld(float width, float height, float x, float y)
        {
            float sx = ((x - width / 2) / (width / 2)) / 2;
            float sy = ((y - height / 2) / (height / 2)) / 2;

            Vector3 worldpos = Position + Forward * Near + Left * sx + Up * (-sy);

            return worldpos;
        }

        public static Vector3 ApplyQuaternion(Vector3 vector, Quaternion quaternion)
        {
            Quaternion q = quaternion * new Quaternion(vector, 0) * Quaternion.Conjugate(quaternion);
            return new Vector3(q.X, q.Y, q.Z);
        }

        private static float Angle(Vector3 a, Vector3 b)
        {
            a.Normalize();
            b.Normalize();
            return (float)Math.Acos(Vector3.Dot(a, b));
        }
    }
}
