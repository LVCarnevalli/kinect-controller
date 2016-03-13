// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Kinect;

namespace Commons
{
    internal static class SkeletalCommon
    {
        /**
         *  Obter e transformar a posição (mouseX,mouseY,z) de uma parte do esqueleto.
         *  No caso, usaremos as posições da mão esquerda e mão direita.
         */
        public static Joint ScaleTo(this Joint joint, int width, int height, float skeletonMaxX, float skeletonMaxY)
        {
            // Obter o esqueleto
            Microsoft.Kinect.SkeletonPoint pos = new SkeletonPoint()
            {
                // Obter posição horizontal
                X = Scale(width, skeletonMaxX, joint.Position.X),
                // Obter posição vertical
                Y = Scale(height, skeletonMaxY, -joint.Position.Y),
                // Obter posição de profundidade
                Z = joint.Position.Z
            };
            // Definir posição do membro
            joint.Position = pos;
            // Retornar valores das posições do membro
            return joint;
        }

        /**
         *  Executar o método ScaleTo com os valores máximos para o esqueleto como padrão.
         */
        public static Joint ScaleTo(this Joint joint, int width, int height)
        {
            return ScaleTo(joint, width, height, 1.0f, 1.0f);
        }

        /**
         *  Método responsável por calcular e obter a posição do esqueleto.
         */
        private static float Scale(int maxPixel, float maxSkeleton, float position)
        {
            float value = ((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));
            if (value > maxPixel)
                return maxPixel;
            if (value < 0)
                return 0;
            return value;
        }
    }
}