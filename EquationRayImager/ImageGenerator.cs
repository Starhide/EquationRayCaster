using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationRayImager
{
    class ImageGenerator
    {

        // Kernel
        static void ImageRender(Index index, Camera cam, int width, int height, ArrayView<Color> output)
        {
            int interations = 50;

            
            int img_x = index % width;
            int img_y = index / width;

            Vector3 ray = cam.ScreenToWorld(width, height, img_x, img_y) - cam.Position;
            ray.Normalize();

            bool done = false;

            var start = cam.Position;
            Vector3 pos = cam.Position;
            float increment = 0.25f;
            float goal = 1;
            double o = 0;
            double po = (2 - Math.Sqrt(pos.X * pos.X + pos.Y * pos.Y)) * (1 - Math.Sqrt(pos.X * pos.X + pos.Y * pos.Y)) + pos.Z * pos.Z;
            //double po = 7 * pos.X * pos.Y / (Math.Pow(Math.E, pos.X * pos.X + pos.Y * pos.Y)) - pos.Z;
            //double po = Math.Sin(pos.X * pos.Y) - pos.Z;
            //double po = Math.Sqrt(Math.Abs(Math.Sin(pos.X) * pos.X + Math.Sin(pos.Y) * pos.Y + Math.Sin(pos.Z) * pos.Z));

            while (!done)
            {
                for (int k = 0; k < interations; k++)
                {
                    pos = start + ray * increment * k;

                    // o = The 3D equation to be rendered
                    o = (2 - Math.Sqrt(pos.X * pos.X + pos.Y * pos.Y)) * (1 - Math.Sqrt(pos.X * pos.X + pos.Y * pos.Y)) + pos.Z * pos.Z; // torus
                    //o = 7 * pos.X * pos.Y / (Math.Pow(Math.E, pos.X * pos.X + pos.Y * pos.Y)) - pos.Z;
                    //o = Math.Sin(pos.X * pos.Y) - pos.Z;
                    //o = Math.Sqrt(Math.Abs(Math.Sin(pos.X) * pos.X + Math.Sin(pos.Y) * pos.Y + Math.Sin(pos.Z) * pos.Z));

                    if (Math.Abs(goal - o) < 0.01)
                    {
                        //output[index] = Vector3.Distance(cam.Position, pos);
                        float xc = Math.Abs(pos.X), yc = Math.Abs(pos.Y), zc = Math.Abs(pos.Z);
                        float s = xc+yc+zc;
                        output[index] = new Color((int)(255.0f * zc / s), (int)(255.0f * yc / s), (int)(255.0f * xc / s));
                        done = true;
                        break;
                    }
                    else
                    {
                        if((o < goal && po > goal) || (o >goal && po < goal))
                        {
                            start = start + ray * increment * (k-1);
                            increment = increment / interations;
                            break;
                        }
                    }

                    po = o;

                    if (k == interations - 1)
                    {
                        output[index] = new Color(0,0,0);
                        done = true;
                    }
                }
            }

        }

        private static Context context;
        private static Accelerator accelerator;
        private static System.Action<Index, Camera,  int, int, ArrayView<Color>> image_kernel;

        /// <summary>
        /// Compile the mandelbrot kernel in ILGPU-CPU or ILGPU-CUDA mode.
        /// </summary>
        /// <param name="withCUDA"></param>
        public static void CompileKernel()
        {
            context = new Context();
            accelerator = new CudaAccelerator(context);

            image_kernel = accelerator.LoadAutoGroupedStreamKernel<
                Index, Camera, int, int, ArrayView<Color>>(ImageRender);
        }

        /// <summary>
        /// Calculate the mandelbrot set on the GPU.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="max_iterations"></param>
        public static Color[] CalcGPU(int width, int height, Camera camera, Texture2D texture2D)
        {
            int num_values = width*height;
            var dev_out = accelerator.Allocate<Color>(num_values);
            Color[] buffer = new Color[num_values];

            // Launch kernel
            image_kernel(num_values, camera, width, height, dev_out.View);
            accelerator.Synchronize();
            dev_out.CopyTo(buffer, 0, 0, num_values);

            

            dev_out.Dispose();
            return buffer;
        }

        /// <summary>
        /// Dispose lightning-context and cuda-context.
        /// </summary>
        public static void Dispose()
        {
            accelerator.Dispose();
            context.Dispose();
        }
    }
}
